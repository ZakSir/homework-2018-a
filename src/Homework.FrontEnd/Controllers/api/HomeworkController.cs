// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Homework.FrontEnd.Controllers.api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Homework;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    /// <summary>
    /// Controller for homework apis
    /// </summary>
    public class HomeworkController : Controller
    {
        /// <summary>
        /// Characters to split the type list. Created here for perf.
        /// </summary>
        private static readonly char[] modelSplitCharacters = new char[] { ',' };

        /// <summary>
        /// Static reference to the test objects that have been created. Lazy for perf.
        /// </summary>
        private static Lazy<Indexed> testObjects = new Lazy<Indexed>(() => new Indexed(new Models.TestObjects()));

        /// <summary>
        /// Lock oject for creating the service object.
        /// </summary>
        private static object modelServiceLockObject = new object();

        /// <summary>
        /// Model service backing store. 
        /// </summary>
        private static Homework.ProblemA.ModelService modelService;

        /// <summary>
        /// Initialization specifier for model service. Volatile for correctness.
        /// </summary>
        private static volatile bool isInitialized = false;

        /// <summary>
        /// Gets the Model service from the backing store and initializes it if required.
        /// </summary>
        protected static Homework.ProblemA.ModelService ModelService
        {
            get
            {
                Telemetry.Info("Getting model Service", "31b0f832-3edd-4d8c-a91e-952645075a05");

                Init();

                return modelService;
            }
        }


        [HttpGet]
        [Route("api/homework/similarities")]
        public IActionResult GetSimilarities([FromQuery]string A, [FromQuery] string B)
        {
            if (string.IsNullOrWhiteSpace(A))
            {
                this.BadRequest($"An object for {nameof(A)} must be specified");
            }

            if (string.IsNullOrWhiteSpace(B))
            {
                this.BadRequest($"An object for {nameof(B)} must be specified");
            }

            string aName = '/' + A;
            string bName = '/' + B;

            if (!testObjects.Value.ContainsKey(aName))
            {
                this.BadRequest($"Object not available in {nameof(A)}. please use /api/homework/testobjects");
            }

            if (!testObjects.Value.ContainsKey(bName))
            {
                this.BadRequest($"Object not available in {nameof(B)}. please use /api/homework/testobjects");
            }

            IEnumerable<Homework.ProblemA.ObjectProperty> result = ModelService.GetMatchingPropertyNames(testObjects.Value[aName], testObjects.Value[bName]);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets a differential list between objects 
        /// </summary>
        /// <param name="typeList">The comma delimited list of types. These type reference values are available from /api/homework/testobjects.</param>
        /// <param name="isHuman">Is a human reading the direct result of this Api?</param>
        /// <param name="selfReference">Perform differentials against the same object.</param>
        /// <returns>A list of differential comparisons.</returns>
        [HttpGet]
        [Route("api/homework/differentials")]
        public IActionResult GetDifferentials([FromQuery]string typeList, [FromQuery]bool isHuman = false, [FromQuery]bool selfReference = true)
        {
            if (string.IsNullOrWhiteSpace(typeList))
            {
                return this.BadRequest("A list of test objects must be specified.");
            }

            string[] parts = typeList.Split(modelSplitCharacters);

            foreach (string part in parts)
            {
                if (!testObjects.Value.ContainsKey("/" + part))
                {
                    // missing model
                    return this.BadRequest($"The test object '{part}' does not exist");
                }
            }

            List<ProblemA.ObjectDifferential> diffs = new List<ProblemA.ObjectDifferential>();

            foreach (string parta in parts)
            {
                foreach (string partb in parts)
                {
                    if (parta == partb && !selfReference)
                    {
                        continue;
                    }

                    string _a = "/" + parta;
                    string _b = "/" + partb;

                    object obja = testObjects.Value[_a];
                    object objb = testObjects.Value[_b];

                    ProblemA.ObjectDifferential od = ModelService.GetDifferential(obja, objb);

                    diffs.Add(od);
                }
            }

            if (isHuman)
            {
                return this.Ok(JsonConvert.SerializeObject(diffs, Formatting.Indented));
            }

            return this.Ok(diffs);
        }

        /// <summary>
        /// Gets the properties of an object by its name.
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <returns>The indexed object.</returns>
        [HttpGet]
        [Route("api/homework/object/{name}")]
        public IActionResult GetObjectByName(string name)
        {
            name = '/' + name;
            if (!testObjects.Value.ContainsKey(name))
            {
                this.BadRequest("Object not available. please use /api/homework/testobjects");
            }

            Indexed indexedSubObject = new Indexed(testObjects.Value[name], true);

            return this.Ok(indexedSubObject.ToFlatDictionary());
        }

        /// <summary>
        /// Gets the cryptographic hash of an object by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/homework/hashobject/{name}")]
        public IActionResult GetObjectHashByName(string name)
        {
            name = '/' + name;
            if (!testObjects.Value.ContainsKey(name))
            {
                this.BadRequest("Object not available. please use /api/homework/testobjects");
            }

            string hash = ModelService.GetCryptographicHashCode(testObjects.Value[name], "md5");

            return this.Ok(hash);
        }

        /// <summary>
        /// Gets a list of available test object references. This list can be used with /api/homework/object/{Name}.
        /// </summary>
        /// <returns>The list of available test object references.</returns>
        [HttpGet]
        [Route("api/homework/testobjects")]
        public IActionResult GetTestObjects()
        {
            return this.Ok(this.GetFirstLevelProperties(testObjects.Value));
        }

        /// <summary>
        /// Initializes the Model Service. 
        /// </summary>
        private static void Init()
        {
            if (isInitialized)
            {
                return;
            }

            lock (modelServiceLockObject)
            {
                if (isInitialized)
                {
                    return;
                }

                Telemetry.Info("Initializing Model Service", "a27e9f91-8cf2-4d66-81ff-960236f59981");

                modelService = new ProblemA.ModelService();
                isInitialized = true;

                Telemetry.Info("Finished Init of Model Service", "6aff2a8b-d209-43b2-9e49-cf867e77b21e");
            }
        }

        /// <summary>
        /// Gets the first level of properties on any POCO object.
        /// </summary>
        /// <param name="o">The object to index.</param>
        /// <returns>A collection of property names.</returns>
        private IEnumerable<string> GetFirstLevelProperties(object o)
        {
            Indexed ix = o as Indexed ?? new Indexed(o, true);

            IEnumerable<string> firstLevel = testObjects.Value.ToFlatDictionary().Keys.Select(s => s.TrimStart('/')).Where(s => !s.Contains('/'));

            return firstLevel;
        }
    }
}

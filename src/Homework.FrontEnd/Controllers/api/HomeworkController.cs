using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Homework;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Homework.FrontEnd.Controllers.api
{
    public class HomeworkController : Controller
    {
        private static readonly char[] modelSplitCharacters = new char[] { ',' };
        private static Lazy<Indexed> testObjects = new Lazy<Indexed>(() => new Indexed(new Models.TestObjects()));

        private static object modelServiceLockObject = new object();

        private static Homework.ProblemA.ModelService modelService;

        private static volatile bool isInitialized = false;

        protected static Homework.ProblemA.ModelService ModelService {
            get{
                Telemetry.Client.TrackTrace();
                Init();

                return modelService;
            }
        }

        private static void Init()
        {
            if(isInitialized)
            {
                return;
            }

            lock(modelServiceLockObject)
            {
                if(isInitialized)
                {
                    return;
                }

                modelService = new ProblemA.ModelService();
                isInitialized = true;
            }
        }

        [HttpGet]
        [Route("api/homework/similarities")]
        public IActionResult GetSimilarities([FromQuery]string A, [FromQuery] string B)
        {
            if (string.IsNullOrWhiteSpace(A))
            {
                BadRequest($"An object for {nameof(A)} must be specified");
            }

            if (String.IsNullOrWhiteSpace(B))
            {
                BadRequest($"An object for {nameof(B)} must be specified");
            }

            string aName = '/' + A;
            string bName = '/' + B;

            if (!testObjects.Value.ContainsKey(aName))
            {
                BadRequest($"Object not available in {nameof(A)}. please use /api/homework/testobjects");
            }

            if (!testObjects.Value.ContainsKey(bName))
            {
                BadRequest($"Object not available in {nameof(B)}. please use /api/homework/testobjects");
            }

            IEnumerable<Homework.ProblemA.ObjectProperty> result = ModelService.GetMatchingPropertyNames(testObjects.Value[aName], testObjects.Value[bName]);

            return Ok(result);
        }

        // GET: api/values
        [HttpGet]
        [Route("api/homework/differentials")]
        public IActionResult GetDifferentials([FromQuery]string typeList, [FromQuery]bool isHuman = false)
        {
            if(string.IsNullOrWhiteSpace(typeList))
            {
                return BadRequest("A list of test objects must be specified.");
            }
            
            string[] parts = typeList.Split(modelSplitCharacters);

            foreach(string part in parts){
                if(!testObjects.Value.ContainsKey("/" + part))
                {
                    // missing model
                    return BadRequest($"The test object '{part}' does not exist");
                }
            }

            List<ProblemA.ObjectDifferential> diffs = new List<ProblemA.ObjectDifferential>();

            foreach(string parta in parts){
                foreach(string partb in parts){
                    string _a = "/" + parta;
                    string _b = "/" + partb;

                    object obja = testObjects.Value[_a];
                    object objb = testObjects.Value[_b];

                    ProblemA.ObjectDifferential od = ModelService.GetDifferential(obja, objb);

                    diffs.Add(od);
                }
            }

            if(isHuman)
            {
                return Ok(JsonConvert.SerializeObject(diffs, Formatting.Indented));
            }

            return Ok(diffs);
        }

        [HttpGet]
        [Route("api/homework/object/{name}")]
        public IActionResult GetObjectByName(string name)
        {
            name = '/' + name;
            if (!testObjects.Value.ContainsKey(name))
            {
                BadRequest("Object not available. please use /api/homework/testobjects");
            }

            Indexed indexedSubObject = new Indexed(testObjects.Value[name], true);

            return Ok(indexedSubObject.ToFlatDictionary());
        }

        [HttpGet]
        [Route("api/homework/hashobject/{name}")]
        public IActionResult GetObjectHashByName(string name)
        {
            name = '/' + name;
            if (!testObjects.Value.ContainsKey(name))
            {
                BadRequest("Object not available. please use /api/homework/testobjects");
            }

            string hash = ModelService.GetCryptographicHashCode(testObjects.Value[name], "md5");

            return Ok(hash);
        }


        [HttpGet]
        [Route("api/homework/testobjects")]
        public IActionResult GetTestObjects()
        {
            return Ok(GetFirstLevelProperties(testObjects.Value));
        }

        private IEnumerable<string> GetFirstLevelProperties(object o)
        {
            Indexed ix = o as Indexed ?? new Indexed(o, true);

            IEnumerable<string> firstLevel = testObjects.Value.ToFlatDictionary().Keys.Select(s => s.TrimStart('/')).Where(s => !s.Contains('/'));

            return firstLevel;
        }

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}

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

        // GET: api/values
        [HttpGet]
        [Route("api/homework/differentials")]
        public IActionResult GetDifferentials([FromQuery]string typeList, [FromQuery]bool isHuman)
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
        [Route("api/homework/testobjects")]
        public IActionResult GetTestObjects()
        {
            var firstLevel = testObjects.Value.ToFlatDictionary().Keys.Select(s => s.TrimStart('/')).Where(s => !s.Contains('/'));

            return Ok(firstLevel);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

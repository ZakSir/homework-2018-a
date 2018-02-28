using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;

namespace Homework.FrontEnd.Controllers.api
{
    [Produces("application/json")]
    public class SportsController : Controller
    {
        private static readonly object InitializationLockObject = new object();

        private static DocumentClient client;

        private static bool isInitialized = false;

        private readonly IConfiguration config;

        public SportsController(IConfiguration config)
        {
            this.config = config;

            Init(config);
        }

        private static void Init(IConfiguration config)
        {
            if (isInitialized)
            {
                return;
            }

            lock (InitializationLockObject)
            {
                if (isInitialized)
                {
                    return;
                }

                string endpoint = config["sports:db:endpoint"];
                string


                Dictionary<string, string> logProperties = new Dictionary<string, string>()
                {
                    [nameof(endpoint)] = endpoint
                };
                
                client = new DocumentClient(endpoint, )
            }
        }

        [HttpGet]
        [Route("api/homework/sports/mascot/{team}")]
        public IActionResult GetMascot(string team)
        {
            return this.Ok("foo");
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#if DEBUG
namespace Homework.FrontEnd.Controllers.api
{

    public class TestController : Controller
    {
        [HttpPost]
        [Route("api/test")]
        public IActionResult TestPost()
        {
            Telemetry.Info("Test Post Reached", "36758c34-80d4-4a05-90dd-869dfe931fef");
            return this.Ok(true);
        }
    }
}
#endif
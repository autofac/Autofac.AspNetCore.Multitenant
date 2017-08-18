using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sandbox.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        public ValuesController(IDependency dependency)
        {
            this.Dependency = dependency;
        }

        [HttpGet]
        public string Get()
        {
            return this.Dependency.Id;
        }

        public IDependency Dependency { get; set; }
    }
}

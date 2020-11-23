using Microsoft.AspNetCore.Mvc;

namespace Sandbox.Shared
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        public ValuesController(IDependency dependency)
        {
            Dependency = dependency;
        }

        [HttpGet]
        public string Get()
        {
            return Dependency.Id;
        }

        public IDependency Dependency { get; set; }
    }
}

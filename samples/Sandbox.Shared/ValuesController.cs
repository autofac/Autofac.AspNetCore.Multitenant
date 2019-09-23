using Microsoft.AspNetCore.Mvc;

namespace Sandbox.Shared.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
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

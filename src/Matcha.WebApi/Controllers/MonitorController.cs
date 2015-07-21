using System.Web.Http;
using System.Web.Http.Description;

namespace Matcha.WebApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MonitorController : ApiController
    {
        /// <summary>
        /// Monitoring Endpoint. This is intended for internal use - keep site alive, ensure we are logging and monitoring can see we are up
        /// </summary>
        /// <returns>A summary of the current state of the system</returns>
        [Route("monitor")]
        public IHttpActionResult Get()
        {
            var assemblyVersion = GetType().Assembly.GetName().Version;
            return Ok(new
            {
                AssemblyVersion = assemblyVersion
                //TODO DB version
            });
        }
    }
}

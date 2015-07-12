using System.Web.Http;

namespace Matcha.WebApi.Controllers
{
    public class MonitorController : ApiController
    {
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

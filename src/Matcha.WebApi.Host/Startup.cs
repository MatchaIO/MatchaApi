using Matcha.WebApi.Config;
using Matcha.WebApi.Host;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace Matcha.WebApi.Host
{

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = WebApiConfig.Register();
            app.UseWebApi(config);
            ContainerConfig.SetUpAutofac(config);
        }
    }
}
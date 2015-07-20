using Matcha.WebApi.Config;
using Matcha.WebApi.Host;
using Microsoft.Owin;
using Owin;
using Swashbuckle.Application;

[assembly: OwinStartup(typeof(Startup))]
namespace Matcha.WebApi.Host
{

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = WebApiConfig.Register();
            app.UseWebApi(config);
            config
                .EnableSwagger(swaggerConfig =>
                {
                    swaggerConfig.SingleApiVersion("v1", "Matcha.IO");
                    swaggerConfig.IncludeXmlComments("Matcha.WebApi.xml");
                })
                .EnableSwaggerUi();
            ContainerConfig.SetUpAutofac(config);
        }
    }
}
using Autofac;
using Autofac.Integration.WebApi;
using Matcha.WebApi.Controllers;

namespace Matcha.WebApi.Config
{
    public class WebApiAutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register the Web API controllers.
            builder.RegisterApiControllers(typeof(MonitorController).Assembly);

            // For registering filter attibutes see : https://code.google.com/p/autofac/wiki/WebApiIntegration#Filters_without_attributesSee 
        }
    }
}
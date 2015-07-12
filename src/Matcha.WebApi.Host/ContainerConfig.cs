using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Matcha.WebApi.Config;

namespace Matcha.WebApi.Host
{
    public static class ContainerConfig
    {
        /// <summary>
        /// https://code.google.com/p/autofac/wiki/WebApiIntegration
        /// </summary>
        /// <param name="config"></param>
        public static IContainer SetUpAutofac(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();
            
            builder.RegisterModule(new WebApiAutofacModule());
            
            var container = builder.Build();
            // Configure Web API with the dependency resolver.
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            return container;
        }
    }
}
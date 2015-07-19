using System.Linq;
using Autofac;
using Autofac.Integration.WebApi;
using Matcha.WebApi.Controllers;
using Matcha.WebApi.Domain.Events;
using Matcha.WebApi.Handlers;

namespace Matcha.WebApi.Config
{
    public class WebApiAutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register the Web API controllers.
            var assembly = typeof(MonitorController).Assembly;
            var publicInstances = assembly.GetTypes().Where(t => t.IsPublic && !t.IsAbstract).ToArray();
             builder.RegisterApiControllers(assembly);
            builder.RegisterTypes(publicInstances.Where(t => t.Namespace.Contains("Handlers")).ToArray())
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            builder.RegisterTypes(publicInstances.Where(t => t.Name.Contains("Repository")).ToArray())
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<SqlEventPublisher>().AsImplementedInterfaces().SingleInstance();
            // For registering filter attibutes see : https://code.google.com/p/autofac/wiki/WebApiIntegration#Filters_without_attributesSee 
        }
    }
}
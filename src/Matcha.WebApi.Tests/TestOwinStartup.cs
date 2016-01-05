using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Matcha.WebApi.Config;
using Matcha.WebApi.Domain.Models;
using Matcha.WebApi.Tests.EventStoreImpl;
using Owin;

namespace Matcha.WebApi.Tests
{
    public class TestOwinStartup
    {
        public static IContainer Container;

        public void Configuration(IAppBuilder app)
        {
            var config = WebApiConfig.Register();
            app.UseWebApi(config);
            Container = SetUpAutofac(config);
        }

        //Copied from the host
        private static IContainer SetUpAutofac(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new WebApiAutofacModule());
            //builder.RegisterModule(new NHibernateImpl.SqliteFileStorageNHibernateModule(typeof(Lead).Assembly));
            builder.RegisterModule(new EventStoreModule());

            var container = builder.Build();
            // Configure Web API with the dependency resolver.
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            return container;
        }
    }
}
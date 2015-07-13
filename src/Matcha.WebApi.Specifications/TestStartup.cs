using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Autofac;
using Autofac.Integration.WebApi;
using Matcha.WebApi.Config;
using Matcha.WebApi.Domain.Models;
using Matcha.WebApi.Specifications.Annotations;
using Owin;

namespace Matcha.WebApi.Specifications
{
    [UsedImplicitly]
    public class TestStartup
    {
        public static IContainer Container;

        public void Configuration(IAppBuilder app)
        {
            var config = WebApiConfig.Register();
            app.UseWebApi(config);

            config.Services.Replace(typeof(IExceptionHandler), new TestExceptionHandler());
            Container = SetUpAutofac(config);
        }

        //Copied from the host
        private static IContainer SetUpAutofac(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new WebApiAutofacModule());
            builder.RegisterModule(new SqliteFileStorageNHibernateModule(typeof(Lead).Assembly));

            var container = builder.Build();
            // Configure Web API with the dependency resolver.
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            return container;
        }

        class TestExceptionHandler : ExceptionHandler
        {
            public override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
            {
                return base.HandleAsync(context, cancellationToken);
            }

            public override bool ShouldHandle(ExceptionHandlerContext context)
            {
                return base.ShouldHandle(context);
            }

            public override void Handle(ExceptionHandlerContext context)
            {
                base.Handle(context);
            }
        }
    }
}
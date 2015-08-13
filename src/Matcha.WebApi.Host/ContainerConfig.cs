using System;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Matcha.WebApi.Config;
using Matcha.WebApi.Domain.Models;
using Matcha.WebApi.Tests; //TODO... naughty
using NHibernate;
using NHibernate.Tool.hbm2ddl;

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
            builder.RegisterModule(new SqliteFileStorageNHibernateModule(typeof(Lead).Assembly));//TODO this is not prod code - its alsoa copied file for a test project... :/

            
            var container = builder.Build();
            // Configure Web API with the dependency resolver.
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            //Create the database, again not prod ready code, but great for local!!!! :D
            new SchemaExport(container.Resolve<NHibernate.Cfg.Configuration>())
                .Execute(
                    useStdOut: false,
                    execute: true,
                    justDrop: false,
                    connection: container.Resolve<ISession>().Connection,
                    exportOutput: Console.Out
                );

            return container;
        }
    }
}
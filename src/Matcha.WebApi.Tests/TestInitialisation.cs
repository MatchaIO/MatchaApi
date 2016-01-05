using System;
using Autofac;
using Microsoft.Owin.Testing;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Matcha.WebApi.Tests
{
    public static class TestInitialisation
    {
        private static readonly object Gate = new object();
        public static WebClientProxy GetWebClientProxy()
        {
            lock (Gate)
            {
                var server = TestServer.Create<TestOwinStartup>();

                var container = TestOwinStartup.Container;
                //var configuration = container.Resolve<NHibernate.Cfg.Configuration>();

                //new SchemaExport(configuration).Execute(
                //    useStdOut: false,
                //    execute: true,
                //    justDrop: false,
                //    connection: container.Resolve<ISession>().Connection,
                //    exportOutput: Console.Out
                //    );

                return new WebClientProxy(server.HttpClient);
            }
        }
    }
}
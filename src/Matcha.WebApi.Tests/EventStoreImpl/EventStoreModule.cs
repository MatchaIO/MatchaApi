using System.Linq;
using System.Net;
using Autofac;
using EventStore.ClientAPI;
using Matcha.WebApi.Domain.DataAccess.EventStoreImpl;

namespace Matcha.WebApi.Tests.EventStoreImpl
{
    public class EventStoreModule : Autofac.Module
    {
        private static readonly IPEndPoint IntegrationTestTcpEndPoint = new IPEndPoint(IPAddress.Loopback, 1113);

        protected override void Load(ContainerBuilder builder)
        {
            var assembly = typeof(Matcha.WebApi.Domain.DataAccess.EventStoreImpl.LeadRepository).Assembly;
            var publicInstances = assembly.GetTypes().Where(t => t.IsPublic && !t.IsAbstract).ToArray();
            var array = publicInstances.Where(t => t.Name.Contains("Repository") && t.Namespace.Contains("EventStore")).ToArray();
            builder.RegisterTypes(array)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.Register<IEventStoreConnection>(ctx =>
             {
                 var connection = EventStoreConnection.Create(IntegrationTestTcpEndPoint);
                 connection.ConnectAsync().Wait	();
                 return connection;
             }).SingleInstance();//Reconsider?
            builder.RegisterType<GetEventStoreRepository>().AsImplementedInterfaces().SingleInstance();//Reconsider??

            builder.RegisterType<NoOpEventPublisher>().AsImplementedInterfaces().SingleInstance();
        }

    }
}

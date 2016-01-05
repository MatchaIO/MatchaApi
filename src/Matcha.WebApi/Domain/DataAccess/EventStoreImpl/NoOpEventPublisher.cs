using Matcha.WebApi.Domain.Events;
using Matcha.WebApi.Handlers;

namespace Matcha.WebApi.Domain.DataAccess.EventStoreImpl
{
    public class NoOpEventPublisher : IEventPublisher
    {
        public void Publish(Event eventToPublish)
        {
            //No Op
        }
    }
}
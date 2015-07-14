using System.Collections.Generic;
using System.Linq;

namespace Matcha.WebApi.Handlers
{
    public class InMemoryEventPublisher : IEventPublisher, IEventRepository
    {
        private readonly List<Event> _publishedEvents = new List<Event>();

        public List<Event> PublishedEvents
        {
            get { return _publishedEvents; }
        }

        public void Publish(Event eventToPublish)
        {
            _publishedEvents.Add(eventToPublish);
        }

        public IEnumerable<Event> EventsOfType(string eventType)
        {
            return _publishedEvents.Where(e => e.GetType().Name == eventType);
        }
    }
}
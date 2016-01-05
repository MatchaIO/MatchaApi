using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using EventStore.ClientAPI;
using Matcha.WebApi.Domain.DataAccess.EventStoreImpl;

namespace Matcha.WebApi.Controllers
{
    public class EventsController : ApiController
    {
        private readonly IEventRepository _eventRepository;

        public EventsController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        /// <summary>
        /// Get all events by Event type
        /// </summary>
        /// <param name="eventType">The full type name of the event</param>
        /// <returns>An enumerable of Events</returns>
        [Route("api/Events/ByType/{eventType}")]
        public IEnumerable<ResolvedEvent> Get(string eventType)
        {
            var eventTypeName = eventType.Split(new[] { '.' }).Last();//Currently we pass the fully qual name over, but the current impl of (our) event store code uses just the name
            var lastEvents = _eventRepository.GetLastEvents();
            var filteredEvents = lastEvents.Where(e => e.Event.EventType==eventTypeName).OrderByDescending(e=>e.Event.Created).ToArray();
            return filteredEvents;
        }
        /// <summary>
        /// Gets all events
        /// </summary>
        /// <returns>All the Events</returns>
        [Route("api/Events/")]
        public ResolvedEvent[] GetAll()
        {
            return _eventRepository.GetLastEvents();//TODO - this is retarded - dont send the whole event store over the wire - use the event store itself
        }
    }
}
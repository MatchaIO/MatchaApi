using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Matcha.WebApi.Domain.Events;
using Matcha.WebApi.Handlers;

namespace Matcha.WebApi.Controllers
{
    public class EventsController : ApiController
    {
        private readonly IEventRepository _eventRepository;

        public EventsController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        [Route("api/Events/ByType/{eventType}")]
        public IEnumerable<Event> Get(string eventType)
        {
            var results =  _eventRepository.EventsOfType(eventType).ToArray();//TODO - this is retarded - dont send the whole event store over the wire
            return results;
        }
        [Route("api/Events/")]
        public IEnumerable<Event> GetAll()
        {
            return _eventRepository.GetAll();//TODO - this is retarded - dont send the whole event store over the wire
        }
    }
}
using System.Collections.Generic;
using System.Web.Http;
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
            return _eventRepository.EventsOfType(eventType);//TODO - this is retarded - dont send the whole event store over the wire
        }
        [Route("api/Events/")]
        public IEnumerable<Event> GetAll()
        {
            return _eventRepository.GetAll();//TODO - this is retarded - dont send the whole event store over the wire
        }
    }
}
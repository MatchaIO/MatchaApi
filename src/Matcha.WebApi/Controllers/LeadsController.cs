using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Matcha.WebApi.Handlers;
using Matcha.WebApi.Messages.Commands;
using Matcha.WebApi.Messages.Events;
using Matcha.WebApi.Messages.Projections;
using Matcha.WebApi.Messages.Queries;

namespace Matcha.WebApi.Controllers
{
    public class LeadsController : ApiController
    {
        private readonly ICommandHandler<CreateLeadCommand, Guid> _createLead;
        private readonly IQueryHandler<GetLeadById, LeadDetail> _getLead;

        public LeadsController(
            ICommandHandler<CreateLeadCommand, Guid> createLead,
            IQueryHandler<GetLeadById, LeadDetail> getLead)
        {
            _createLead = createLead;
            _getLead = getLead;
        }

        /// <summary>
        /// Gets a lead by opportunityId. Restricted to users with sales permssions 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/leads/{id}")]
        public LeadDetail Get(Guid id)
        {
            return _getLead.Handle(new GetLeadById(id));
        }

        /// <summary>
        /// Create the bare minimum lead required for Sales to contact the prospective client
        /// Raises <see cref="LeadCreated"/>. 
        /// </summary>
        /// <param name="lead"></param>
        [Route("api/leads")]
        public HttpResponseMessage Post([FromBody]CreateLeadCommand lead)
        {
            var newId = _createLead.Handle(lead);
            var response = Request.CreateResponse(HttpStatusCode.Created, newId);
            //Assuming we are following std rest resourcing (ie POST to /X/ and GET from /X/{id})
            response.Headers.Location = Request.RequestUri.Combine(newId);
            return response;
        }
    }
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
    }
}
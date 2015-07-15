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
        private readonly ICommandHandler<UpdateLeadCommand, Guid> _updateLead;
        private readonly IQueryHandler<GetLeadById, LeadDetail> _getLead;
        private readonly IQueryHandler<GetLeads, IEnumerable<LeadDetail>> _getLeads;
        
        public LeadsController(
            ICommandHandler<CreateLeadCommand, Guid> createLead,
            ICommandHandler<UpdateLeadCommand, Guid> updateLead,
            IQueryHandler<GetLeadById, LeadDetail> getLead, 
            IQueryHandler<GetLeads, IEnumerable<LeadDetail>> getLeads)
        {
            _createLead = createLead;
            _getLead = getLead;
            _getLeads = getLeads;
            _updateLead = updateLead;
        }

        /// <summary>
        /// Gets all leads. Restricted to users with sales permssions 
        /// </summary>
        /// <returns></returns>
        [Route("api/leads/")]
        public IEnumerable<LeadDetail> Get()
        {
            return _getLeads.Handle(new GetLeads());//TODO should at least consider batching?
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

        /// <summary>
        /// Modifies the sales lead required for Sales to contact the prospective client
        /// Raises <see cref="LeadUpdated"/>. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lead"></param>
        [Route("api/leads/{id}")]
        public HttpResponseMessage Put(Guid id, [FromBody]UpdateLeadCommand lead)
        {
            lead.Id = id;
            _updateLead.Handle(lead);
            var response = Request.CreateResponse(HttpStatusCode.Accepted, id);
            response.Headers.Location = Request.RequestUri;
            return response;
        }
    }
}
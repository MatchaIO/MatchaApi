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
        private readonly ICommandHandler<CreateLeadCommand, LeadDetail> _createLead;
        private readonly ICommandHandler<UpdateLeadCommand, LeadDetail> _updateLead;
        private readonly ICommandHandler<DeleteLeadCommand, Guid> _deleteLead;
        private readonly IQueryHandler<GetLeadById, LeadDetail> _getLead;
        private readonly IQueryHandler<GetLeads, IEnumerable<LeadDetail>> _getLeads;

        public LeadsController(
            ICommandHandler<CreateLeadCommand, LeadDetail> createLead,
            ICommandHandler<UpdateLeadCommand, LeadDetail> updateLead,
            ICommandHandler<DeleteLeadCommand, Guid> deleteLead,
            IQueryHandler<GetLeadById, LeadDetail> getLead,
            IQueryHandler<GetLeads, IEnumerable<LeadDetail>> getLeads)
        {
            _createLead = createLead;
            _getLead = getLead;
            _getLeads = getLeads;
            _deleteLead = deleteLead;
            _updateLead = updateLead;
        }

        /// <summary>
        /// Gets all leads. Restricted to users with sales permssions 
        /// </summary>
        /// <returns>All the current Leads</returns>
        [Route("api/leads/")]
        public IEnumerable<LeadDetail> Get()
        {
            return _getLeads.Handle(new GetLeads());//TODO should at least consider batching?
        }

        /// <summary>
        /// Gets a lead by Id. Restricted to users with sales permssions
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The lead details</returns>
        [Route("api/leads/{id}")]
        public LeadDetail Get(Guid id)
        {
            return _getLead.Handle(new GetLeadById(id));
        }

        /// <summary>
        /// Create the bare minimum lead required for Sales to contact the prospective client
        /// Raises <see cref="LeadCreated"/>. 
        /// </summary>
        /// <param name="lead">A create lead command</param>
        [Route("api/leads")]
        public HttpResponseMessage Post([FromBody]CreateLeadCommand lead)
        {
            var newLead = _createLead.Handle(lead);
            var response = Request.CreateResponse(HttpStatusCode.Created, newLead);
            //Assuming we are following std rest resourcing (ie POST to /X/ and GET from /X/{id})
            response.Headers.Location = Request.RequestUri.Combine(newLead.Id);
            return response;
        }

        /// <summary>
        /// Modifies the sales lead required for Sales to contact the prospective client
        /// Raises <see cref="LeadUpdated"/>. 
        /// </summary>
        /// <param name="id">Ther lead Id</param>
        /// <param name="lead">The update lead command</param>
        [Route("api/leads/{id}")]
        public HttpResponseMessage Put(Guid id, [FromBody]UpdateLeadCommand lead)
        {
            lead.Id = id;
            var updatedLead = _updateLead.Handle(lead);
            var response = Request.CreateResponse(HttpStatusCode.OK, updatedLead);
            response.Headers.Location = Request.RequestUri;
            return response;
        }

        /// <summary>
        /// Withdraws the sales lead 
        /// Raises <see cref="LeadDeleted"/>. 
        /// </summary>
        /// <param name="id">The lead Id</param>
        [Route("api/leads/{id}")]
        public HttpResponseMessage Delete(Guid id)
        {
            var cmd = new DeleteLeadCommand { Id = id };
            _deleteLead.Handle(cmd);
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
using System;
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
    public class OpportunitiesController : ApiController
    {
        private readonly ICommandHandler<CreateOpportunityCommand, OpportunityDetail> _createOpportunity;
        private readonly IQueryHandler<GetOpportunityById, OpportunityDetail> _getOpportunity;

        public OpportunitiesController(ICommandHandler<CreateOpportunityCommand, OpportunityDetail> createOpportunity, IQueryHandler<GetOpportunityById, OpportunityDetail> getOpportunity)
        {
            _createOpportunity = createOpportunity;
            _getOpportunity = getOpportunity;
        }

        /// <summary>
        /// Gets a opportunity by opportunity Id. Restricted to users with sales or talent permssions 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/opportunities/{id}")]
        public OpportunityDetail Get(Guid id)
        {
            return _getOpportunity.Handle(new GetOpportunityById(id));
        }

        /// <summary>
        /// Create an opportunity that can now be accessed by the talent. This can also be used to vet existing leads, in which those leads will no longer be visible to sales users.
        /// Restricted to sales users
        /// Raises <see cref="OpportunityCreated"/>. 
        /// Optionally Raises <see cref="LeadVetted"/>. 
        /// </summary>
        /// <param name="command">The create opportunity command</param>
        [Route("api/opportunities")]
        public HttpResponseMessage Post([FromBody]CreateOpportunityCommand command)
        {
            var newOpportunity = _createOpportunity.Handle(command);
            var response = Request.CreateResponse(HttpStatusCode.Created, newOpportunity);
            //Assuming we are following std rest resourcing (ie POST to /X/ and GET from /X/{id})
            response.Headers.Location = Request.RequestUri.Combine(newOpportunity.Id);
            return response;
        }
    }
}
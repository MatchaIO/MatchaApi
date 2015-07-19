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

        ///// <summary>
        ///// Gets all current opportunities. Restricted to users with sales permssions 
        ///// </summary>
        ///// <returns></returns>
        //[Route("api/opportunities/")]
        //public IEnumerable<LeadDetail> Get()
        //{
        //    return _getOpportunities.Handle(new GetOpportunities());//TODO should at least consider batching?
        //}

        /// <summary>
        /// Gets a opportunity by opportunityId. Restricted to users with sales permssions 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/opportunities/{id}")]
        public OpportunityDetail Get(Guid id)
        {
            return _getOpportunity.Handle(new GetOpportunityById(id));
        }

        /// <summary>
        /// Create the bare minimum lead required for Sales to contact the prospective client
        /// Raises <see cref="OpportunityCreated"/>. 
        /// </summary>
        /// <param name="command"></param>
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
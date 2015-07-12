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
        /// Create the bare minimum lead required for Sales to contact the prospective client
        /// Raises <see cref="LeadCreated"/>. 
        /// </summary>
        /// <param name="lead"></param>
        public HttpResponseMessage Post([FromBody]CreateLeadCommand lead)
        {
            var newId = _createLead.Handle(lead);
            var response = Request.CreateResponse(HttpStatusCode.Created, newId);
                //Assuming we are following std rest resourcing (ie POST to /X/ and GET from /X/{id})
            response.Headers.Location = Request.RequestUri.Combine(newId);
            return response;
        }
    }
    public static class UriExtensions
    {
        public static Uri Combine(this Uri baseUri, object path)
        {
            var baseUrlAsString = baseUri.AbsoluteUri.TrimEnd('/') + "/";
            return new Uri(baseUrlAsString + path.ToString().TrimStart('/'));
        }
    }
}
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Matcha.WebApi.Handlers;
using Matcha.WebApi.Messages.Commands;
using Matcha.WebApi.Messages.Dtos;
using Matcha.WebApi.Messages.Events;
using Matcha.WebApi.Messages.Queries;

namespace Matcha.WebApi.Controllers
{
    public class TalentController : ApiController
    {
        private readonly ICommandHandler<CreateTalentProfileCommand, TalentProfile> _createTalentProfile;
        private readonly IQueryHandler<GetTalentById, TalentProfile> _getTalent;

        public TalentController(ICommandHandler<CreateTalentProfileCommand, TalentProfile> createTalentProfile, IQueryHandler<GetTalentById, TalentProfile> getTalent)
        {
            _createTalentProfile = createTalentProfile;
            _getTalent = getTalent;
        }

        /// <summary>
        /// Gets a lead by Id. Restricted to users with sales permssions
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The lead details</returns>
        [Route("api/talent/{id}")]
        public TalentProfile Get(Guid id)
        {
            return _getTalent.Handle(new GetTalentById(id));
        }

        /// <summary>
        /// Create a talent profile
        /// Raises <see cref="TalentProfileCreated"/>. 
        /// </summary>
        /// <param name="talentProfile">A create lead command</param>
        [Route("api/talent")]
        public HttpResponseMessage Post([FromBody]CreateTalentProfileCommand talentProfile)
        {
            var newTalent = _createTalentProfile.Handle(talentProfile);
            var response = Request.CreateResponse(HttpStatusCode.Created, newTalent);
            //Assuming we are following std rest resourcing (ie POST to /X/ and GET from /X/{id})
            response.Headers.Location = Request.RequestUri.Combine(newTalent.Id);
            return response;
        }
    }
}
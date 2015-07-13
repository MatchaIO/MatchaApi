using System;
using System.Linq;
using ExpectedObjects;
using Matcha.WebApi.Messages.Commands;
using Matcha.WebApi.Messages.Events;
using Matcha.WebApi.Messages.Projections;
using Ploeh.AutoFixture;
using TechTalk.SpecFlow;
using Xunit;

namespace Matcha.WebApi.Specifications.LeadFeatures
{
    [Binding]
    public class ManageLeadsSteps
    {
        private readonly MatchaFixtureContext _matcha;
        private Uri _lastReponseHeaderLocation;
        private Guid _lastCreatedId { get { return Guid.Parse(_lastReponseHeaderLocation.ToString().Split('/').Last()); } }
        private CreateLeadCommand _cmd;
        private static readonly Fixture Auto = new Fixture();

        public ManageLeadsSteps()
        {
            _matcha = new MatchaFixtureContext();
        }

        [Given(@"an anonymous user using the api")]
        public void GivenAnAnonymousUserUsingTheApi()
        {
            _matcha.SetCurrentUser(Users.Anon);
        }

        [When(@"they submit their contact details")]
        public void WhenTheySubmitTheirContactDetails()
        {
            _cmd = Auto.Create<CreateLeadCommand>();
            _lastReponseHeaderLocation = _matcha.Post("/api/leads", _cmd);
        }

        [Then(@"the lead id is returned")]
        public void ThenTheLeadIdIsReturned()
        {
            Assert.NotEqual(default(Guid), _lastCreatedId);
        }

        [Then(@"a SalesAdmin user can retrieve the lead")]
        public void ThenASalesAdminUserCanRetrieveTheLead()
        {
            _matcha.SetCurrentUser(Users.SalesAdmin);
            var lead = _matcha.Get<LeadDetail>(_lastReponseHeaderLocation);
            lead.ToExpectedObject()
                .ShouldMatch(
                new
                {
                    Id = _lastCreatedId,
                    _cmd.ContactDetails
                });
        }

        [Then(@"LeadCreated event is raised")]
        public void ThenLeadCreatedEventIsRaised()
        {
            var createdEvent = _matcha.GetLastEventOfType<LeadCreated>();
            createdEvent.LeadDetail
                .ToExpectedObject()
                .ShouldMatch(new
                {
                    Id = _lastCreatedId,
                    _cmd.ContactDetails
                });
        }
    }
}

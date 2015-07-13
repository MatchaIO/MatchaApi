using ExpectedObjects;
using Matcha.WebApi.Messages.Commands;
using Matcha.WebApi.Messages.Events;
using Matcha.WebApi.Messages.Projections;
using Ploeh.AutoFixture;
using TechTalk.SpecFlow;

namespace Matcha.WebApi.Specifications.LeadFeatures
{
    [Binding]
    public class ManageLeadsSteps
    {
        private readonly MatchaFixtureContext _matcha;
        private CreateLeadCommand _cmd;
        private static readonly Fixture Auto = new Fixture();

        public ManageLeadsSteps()
        {
            _matcha = new MatchaFixtureContext();
        }

        [When(@"they submit their contact details")]
        public void WhenTheySubmitTheirContactDetails()
        {
            _cmd = Auto.Create<CreateLeadCommand>();
            _matcha.Post("/api/leads", _cmd);
        }

        [Then(@"a SalesAdmin user can retrieve the lead")]
        public void ThenASalesAdminUserCanRetrieveTheLead()
        {
            ScenarioContext.Current.SetCurrentUser(Users.SalesAdmin);
            var lead = _matcha.Get<LeadDetail>(ScenarioContext.Current.GetLastPostResponseHeaderLocation());
            lead.ToExpectedObject()
                .ShouldMatch(
                new
                {
                    Id = ScenarioContext.Current.GetLastPostResponseAggregateId(),
                    _cmd.ContactDetails
                });
        }

        [Then(@"LeadCreated event is raised")]
        public void ThenLeadCreatedEventIsRaised()
        {
            ScenarioContext.Current.SetCurrentUser(Users.EventSubscriber);
            var createdEvent = _matcha.GetLastEventOfType<LeadCreated>();
            createdEvent.LeadDetail
                .ToExpectedObject()
                .ShouldMatch(new
                {
                    Id = ScenarioContext.Current.GetLastPostResponseAggregateId(),
                    _cmd.ContactDetails
                });
        }
    }
}

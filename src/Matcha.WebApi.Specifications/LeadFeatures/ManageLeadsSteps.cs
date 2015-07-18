using System;
using System.Net.Http;
using ExpectedObjects;
using Matcha.WebApi.Messages.Commands;
using Matcha.WebApi.Messages.Dtos;
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
        private CreateLeadCommand _createLeadCmd;
        private UpdateLeadCommand _updateLeadCmd;
        private static readonly Fixture Auto = new Fixture();

        public ManageLeadsSteps()
        {
            _matcha = new MatchaFixtureContext();
        }

        [Given(@"they have submited their contact details")]
        [When(@"they submit their contact details")]
        [Given(@"a valid Lead exists")]
        public void WhenTheySubmitTheirContactDetails()
        {
            _createLeadCmd = Auto.Create<CreateLeadCommand>();
            _matcha.Post("/api/leads", _createLeadCmd);
        }

        [When(@"they submit their contact details with no name")]
        public void WhenTheySubmitTheirContactDetailsWithNoName()
        {
            _createLeadCmd = Auto.Build<CreateLeadCommand>().Create();
            _createLeadCmd.ContactDetails.ContactName = null;
            _createLeadCmd.ContactDetails.OrganiastionName = null;
            _matcha.Post("/api/leads", _createLeadCmd);
        }

        [When(@"they submit their contact details with no contact")]
        public void WhenTheySubmitTheirContactDetailsWithNoContact()
        {
            _createLeadCmd = Auto.Build<CreateLeadCommand>().Create();
            _createLeadCmd.ContactDetails.Contacts = new Contact[] { };
            _matcha.Post("/api/leads", _createLeadCmd);
        }

        [When(@"they modify their contact details")]
        public void WhenTheyModifyTheirContactDetails()
        {
            var leadId = ScenarioContext.Current.GetLastPostResponseAggregateId();
            _updateLeadCmd = Auto.Build<UpdateLeadCommand>().With(cmd => cmd.Id, leadId).Create();
            _matcha.Put(ScenarioContext.Current.GetLastPostResponseHeaderLocation(), _updateLeadCmd);
        }

        [When(@"they modify their contact details with no name")]
        public void WhenTheyModifyTheirContactDetailsWithNoName()
        {
            var leadId = ScenarioContext.Current.GetLastPostResponseAggregateId();
            _updateLeadCmd = Auto.Build<UpdateLeadCommand>().With(cmd => cmd.Id, leadId).Create();
            _updateLeadCmd.ContactDetails.ContactName = null;
            _updateLeadCmd.ContactDetails.OrganiastionName = null;
            _matcha.Put(ScenarioContext.Current.GetLastPostResponseHeaderLocation(), _updateLeadCmd);
        }
        
        [When(@"they modify their contact details with no contact")]
        public void WhenTheyModifyTheirContactDetailsWithNoContact()
        {
            var leadId = ScenarioContext.Current.GetLastPostResponseAggregateId();
            _updateLeadCmd = Auto.Build<UpdateLeadCommand>().With(cmd => cmd.Id, leadId).Create();
            _updateLeadCmd.ContactDetails.Contacts = new Contact[]{};
            _matcha.Put(ScenarioContext.Current.GetLastPostResponseHeaderLocation(), _updateLeadCmd);
        }


        [When(@"a delete command is made with an invalid id,")]
        public void WhenADeleteCommandIsMadeWithAnInvalidId()
        {
            _matcha.Delete("/api/leads/" + Guid.NewGuid());
        }


        [Then(@"a SalesAdmin user can retrieve the lead")]
        public void ThenASalesAdminUserCanRetrieveTheLead()
        {
            ScenarioContext.Current.SetCurrentUser(Users.SalesAdmin);
            var lead = _matcha.Get<LeadDetail>(ScenarioContext.Current.GetLastPostResponseHeaderLocation());
            var contactDetails = _updateLeadCmd != null ? _updateLeadCmd.ContactDetails : _createLeadCmd.ContactDetails;
            lead.ToExpectedObject()
                .ShouldMatch(
                new
                {
                    Id = ScenarioContext.Current.GetLastPostResponseAggregateId(),
                    ContactDetails = contactDetails
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
                    _createLeadCmd.ContactDetails
                });
        }

        [Then(@"LeadUpdated event is raised")]
        public void ThenLeadUpdatedEventIsRaised()
        {
            ScenarioContext.Current.SetCurrentUser(Users.EventSubscriber);
            var createdEvent = _matcha.GetLastEventOfType<LeadUpdated>();
            createdEvent.LeadDetail
                .ToExpectedObject()
                .ShouldMatch(new
                {
                    Id = ScenarioContext.Current.GetLastPostResponseAggregateId(),
                    _updateLeadCmd.ContactDetails
                });
        }

        [Then(@"no (.*) are created")]
        public void NoEntityIsCreated(string entitytype)
        {
            var response = _matcha.HttpClient.GetAsync("api/" + entitytype).Result; 
            response.EnsureSuccessStatusCode();
            ScenarioContext.Current.SetLastResponse(response);
            Assert.Empty(response.Content.ReadAsAsync<object[]>().Result);
        }

        [Then(@"no events are raised")]
        public void AndNoEventIsRaised()
        {
            Assert.Null(_matcha.GetLastEventOfType<LeadUpdated>());
            Assert.Null(_matcha.GetLastEventOfType<LeadCreated>());
        }

        [Then(@"no UpdateLead event is raised")]
        public void AndNoUpdateLeadEventIsRaised()
        {
            Assert.Null(_matcha.GetLastEventOfType<LeadUpdated>());
        }
        
        [Then(@"no DeleteLead event is raised")]
        public void ThenNoDeleteLeadEventIsRaised()
        {
            Assert.Null(_matcha.GetLastEventOfType<LeadDeleted>());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private dynamic _opportunityProposal;
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
            _createLeadCmd = CreateLeadCmd();
            _matcha.Post("/api/leads", _createLeadCmd);
        }

        [When(@"they submit a fullly populated lead")]
        public void WhenTheySubmitAFullyPopulatedLead()
        {
            _opportunityProposal = new { Title = Guid.NewGuid().ToString(), Description = Guid.NewGuid().ToString() };
            _createLeadCmd = CreateLeadCmd();
            _createLeadCmd.OpportunityProposal = _opportunityProposal;
            _matcha.Post("/api/leads", _createLeadCmd);
        }

        [Given(@"a Lead has been previously deleted")]
        public void GivenLeadHasBeenPreviouslyDeleted()
        {
            _createLeadCmd = CreateLeadCmd();
            _matcha.Post("/api/leads", _createLeadCmd);
            _matcha.Delete(ScenarioContext.Current.GetLastResponseHeaderLocation(HttpMethod.Post));
            _matcha.IgnoreAllPriorEvents();
        }

        [When(@"they submit their contact details with no name")]
        public void WhenTheySubmitTheirContactDetailsWithNoName()
        {
            _createLeadCmd = CreateLeadCmd();
            _createLeadCmd.ContactDetails.ContactName = null;
            _createLeadCmd.ContactDetails.OrganiastionName = null;
            _matcha.Post("/api/leads", _createLeadCmd);
        }

        [When(@"they submit their contact details with no contact")]
        public void WhenTheySubmitTheirContactDetailsWithNoContact()
        {
            _createLeadCmd = CreateLeadCmd();
            _createLeadCmd.ContactDetails.Contacts = new Contact[] { };
            _matcha.Post("/api/leads", _createLeadCmd);
        }

        [When(@"an update command is made with the lead id")]
        public void WhenTheyModifyTheirContactDetails()
        {
            var leadId = ScenarioContext.Current.GetLastResponseAggregateId(HttpMethod.Post, Resources.Leads);
            _updateLeadCmd = Auto.Build<UpdateLeadCommand>().With(cmd => cmd.Id, leadId).Create();
            _updateLeadCmd.OpportunityProposal = new { Title = Guid.NewGuid().ToString(), Description = Guid.NewGuid().ToString() };
            _matcha.Put(ScenarioContext.Current.GetLastResponseHeaderLocation(HttpMethod.Post), _updateLeadCmd);
        }

        [When(@"they modify their contact details with no name")]
        public void WhenTheyModifyTheirContactDetailsWithNoName()
        {
            var leadId = ScenarioContext.Current.GetLastResponseAggregateId(HttpMethod.Post, Resources.Leads);
            _updateLeadCmd = Auto.Build<UpdateLeadCommand>().With(cmd => cmd.Id, leadId).Create();
            _updateLeadCmd.ContactDetails.ContactName = null;
            _updateLeadCmd.ContactDetails.OrganiastionName = null;
            _updateLeadCmd.OpportunityProposal = new { Title = Guid.NewGuid().ToString(), Description = Guid.NewGuid().ToString() };
            _matcha.Put(ScenarioContext.Current.GetLastResponseHeaderLocation(HttpMethod.Post), _updateLeadCmd);
        }

        [When(@"they modify their contact details with no contact")]
        public void WhenTheyModifyTheirContactDetailsWithNoContact()
        {
            var leadId = ScenarioContext.Current.GetLastResponseAggregateId(HttpMethod.Post, Resources.Leads);
            _updateLeadCmd = Auto.Build<UpdateLeadCommand>().With(cmd => cmd.Id, leadId).Create();
            _updateLeadCmd.ContactDetails.Contacts = new Contact[] { };
            _updateLeadCmd.OpportunityProposal = new { Title = Guid.NewGuid().ToString(), Description = Guid.NewGuid().ToString() };
            _matcha.Put(ScenarioContext.Current.GetLastResponseHeaderLocation(HttpMethod.Post), _updateLeadCmd);
        }

        [When(@"a delete command is made with an invalid id")]
        public void WhenADeleteCommandIsMadeWithAnInvalidId()
        {
            _matcha.Delete("/api/leads/" + Guid.NewGuid());
        }

        [When(@"an update command is made with an invalid id")]
        public void WhenAnUpdateCommandIsMadeWithAnInvalidId()
        {
            _matcha.Put("/api/leads/" + Guid.NewGuid(), Auto.Create<UpdateLeadCommand>());
        }

        [When(@"a delete command is made with the lead id")]
        public void WhenADeleteCommandIsMadeWithTheLeadId()
        {
            _matcha.Delete(ScenarioContext.Current.GetLastResponseHeaderLocation(HttpMethod.Post));
        }

        [When(@"the lead is vetted")]
        public void WhenTheLeadIsVetted()
        {
            var lead = _matcha.Get<LeadDetail>(ScenarioContext.Current.GetLastResponseHeaderLocation(HttpMethod.Post));
            _matcha.Post("/api/opportunities", new CreateOpportunityCommand { ContactDetails = lead.ContactDetails, LeadId = lead.Id });
        }

        [Then(@"a SalesAdmin user can retrieve the lead")]
        public void ThenASalesAdminUserCanRetrieveTheLead()
        {
            ScenarioContext.Current.SetCurrentUser(Users.SalesAdmin);
            var lead = _matcha.Get<LeadDetail>(ScenarioContext.Current.GetLastResponseHeaderLocation(HttpMethod.Post));
            var contactDetails = _updateLeadCmd != null ? _updateLeadCmd.ContactDetails : _createLeadCmd.ContactDetails;
            var op = _updateLeadCmd != null ? _updateLeadCmd.OpportunityProposal : _createLeadCmd.OpportunityProposal;
            new
            {
                Id = ScenarioContext.Current.GetLastResponseAggregateId(HttpMethod.Post, Resources.Leads),
                ContactDetails = contactDetails,
            }
                .ToExpectedObject()
                .ShouldMatch(lead);

            AssertDynamicEqual(op, lead.OpportunityProposal);
        }

        [Then(@"LeadCreated event is raised")]
        public void ThenLeadCreatedEventIsRaised()
        {
            ScenarioContext.Current.SetCurrentUser(Users.EventSubscriber);
            var createdEvent = _matcha.GetLastEventOfType<LeadCreated>();
            new
            {
                Id = ScenarioContext.Current.GetLastResponseAggregateId(HttpMethod.Post, Resources.Leads),
                _createLeadCmd.ContactDetails,
            }
                .ToExpectedObject()
                .ShouldMatch(createdEvent.Payload);
            
            AssertDynamicEqual(_createLeadCmd.OpportunityProposal, createdEvent.Payload.OpportunityProposal);
        }

        [Then(@"LeadUpdated event is raised")]
        public void ThenLeadUpdatedEventIsRaised()
        {
            ScenarioContext.Current.SetCurrentUser(Users.EventSubscriber);
            var updated = _matcha.GetLastEventOfType<LeadUpdated>();
            new {
                    Id = ScenarioContext.Current.GetLastResponseAggregateId(HttpMethod.Post, Resources.Leads),
                    _updateLeadCmd.ContactDetails,
                }
                .ToExpectedObject()
                .ShouldMatch(updated.Payload);

            AssertDynamicEqual(_updateLeadCmd.OpportunityProposal, updated.Payload.OpportunityProposal);
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

        [Then(@"no LeadUpdated event is raised")]
        public void AndNoUpdateLeadEventIsRaised()
        {
            Assert.Null(_matcha.GetLastEventOfType<LeadUpdated>());
        }

        [Then(@"no LeadDeleted event is raised")]
        public void ThenNoDeleteLeadEventIsRaised()
        {
            var deletedId = ScenarioContext.Current.GetLastResponseAggregateId(HttpMethod.Post, Resources.Leads);
            var deletesEvents = _matcha.GetEventsOfType<LeadDeleted>();
            Assert.Empty(deletesEvents);
        }

        [Then(@"the lead can not be retrieved by id")]
        public void ThenTheLeadCanNoLongerBeRetrievedById()
        {
            var response = _matcha.Get(ScenarioContext.Current.GetLastResponseHeaderLocation(HttpMethod.Post, Resources.Leads));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Then(@"the opportunity can be retrieved by id")]
        public void ThenTheOpportunityCanBeRetrievedById()
        {
            var response = _matcha.Get<OpportunityDetail>(ScenarioContext.Current.GetLastResponseHeaderLocation(HttpMethod.Post));
            Assert.NotNull(response);
            _createLeadCmd.ContactDetails.ToExpectedObject().ShouldMatch(response.ContactDetails);
        }

        [Then(@"the lead does not appear in the lead list")]
        public void ThenTheLeadDoesNotAppearInTheLeadList()
        {
            var deletedId = ScenarioContext.Current.GetLastResponseAggregateId(HttpMethod.Post, Resources.Leads);
            var leadIds = _matcha.Get<IEnumerable<LeadDetail>>("api/leads").Select(x => x.Id);
            Assert.DoesNotContain(deletedId, leadIds);
        }

        [Then(@"the opportunity details are on the lead")]
        public void TheOpportunityDetailsAreOnTheLead()
        {
            var response = _matcha.Get<LeadDetail>(ScenarioContext.Current.GetLastResponseHeaderLocation(HttpMethod.Post, Resources.Leads));
            AssertDynamicEqual(_opportunityProposal, response.OpportunityProposal);
        }

        [Then(@"LeadDeleted event is raised")]
        public void ThenLeadDeletedEventIsRaised()
        {
            var deleteEvent = _matcha.GetLastEventOfType<LeadDeleted>();
            Assert.NotNull(deleteEvent);
            Assert.Equal(ScenarioContext.Current.GetLastResponseAggregateId(HttpMethod.Post, Resources.Leads), deleteEvent.AggregateId);
        }

        [Then(@"LeadVetted event is raised")]
        public void ThenLeadVettedEventIsRaised()
        {
            var leadVetted = _matcha.GetLastEventOfType<LeadVetted>();
            Assert.NotNull(leadVetted);
            Assert.Equal(ScenarioContext.Current.GetLastResponseAggregateId(HttpMethod.Post, Resources.Opportunities), leadVetted.Payload.OpportunityId);
        }

        [Then(@"OpportunityCreated event is raised")]
        public void ThenOpportunityCreatedEventIsRaised()
        {
            var opportunityCreated = _matcha.GetLastEventOfType<OpportunityCreated>();
            Assert.NotNull(opportunityCreated);
            Assert.Equal(ScenarioContext.Current.GetLastResponseAggregateId(HttpMethod.Post, Resources.Opportunities), opportunityCreated.OpportunityDetail.Id);
            _createLeadCmd.ContactDetails.ToExpectedObject().ShouldEqual(opportunityCreated.OpportunityDetail.ContactDetails);
        }

        private static CreateLeadCommand CreateLeadCmd()
        {
            var cmd = Auto.Create<CreateLeadCommand>();
            cmd.OpportunityProposal = new { Title = Guid.NewGuid().ToString(), Description = Guid.NewGuid().ToString() };
            return cmd;
        }
        private void AssertDynamicEqual(object a, object b)
        {
            Assert.Equal(Newtonsoft.Json.JsonConvert.SerializeObject(a), Newtonsoft.Json.JsonConvert.SerializeObject(b));
        }
    }
}

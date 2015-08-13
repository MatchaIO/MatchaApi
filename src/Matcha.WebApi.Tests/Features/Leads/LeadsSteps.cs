using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Autofac;
using ExpectedObjects;
using Matcha.WebApi.Messages.Commands;
using Matcha.WebApi.Messages.Dtos;
using Matcha.WebApi.Messages.Events;
using Matcha.WebApi.Messages.Projections;
using Microsoft.Owin.Testing;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Ploeh.AutoFixture;
using Xunit;

namespace Matcha.WebApi.Tests.Features.Leads
{
    public partial class LeadScenarios
    {
        private readonly WebClientProxy _proxy;
        private CreateLeadCommand _createLeadCmd;
        private UpdateLeadCommand _updateLeadCmd;
        private dynamic _opportunityProposal;
        private static readonly Fixture Auto = new Fixture();

        public LeadScenarios()
        {
            _proxy = TestInitialisation.GetWebClientProxy();
        }

        void AnAnonymousUserUsingTheApi()
        {
            _proxy.SetCurrentUser(Users.Anon);
        }
        void AnSalesUserUsingTheApi()
        {
            _proxy.SetCurrentUser(Users.SalesAdmin);
        }

        void TheySubmitTheirContactDetails()
        {
            _createLeadCmd = CreateLeadCmd();
            _proxy.Post("/api/leads", _createLeadCmd);
        }

        void TheNewIdIsReturned()
        {
            var lastCreatedId = _proxy.GetLastResponseAggregateId(HttpMethod.Post);
            Assert.NotEqual(default(Guid), lastCreatedId);
        }


        void AStatusCodeIsReturned(HttpStatusCode code)
        {
            Assert.Equal(code, _proxy.GetLastResponseStatusCode());
        }
        void ASalesadminUserCanRetrieveTheLead()
        {
            _proxy.SetCurrentUser(Users.SalesAdmin);
            var lead = _proxy.Get<LeadDetail>(_proxy.GetLastResponseHeaderLocation(HttpMethod.Post));
            var contactDetails = _updateLeadCmd != null ? _updateLeadCmd.ContactDetails : _createLeadCmd.ContactDetails;
            var op = _updateLeadCmd != null ? _updateLeadCmd.OpportunityProposal : _createLeadCmd.OpportunityProposal;
            new
            {
                Id = _proxy.GetLastResponseAggregateId(HttpMethod.Post, Resources.Leads),
                ContactDetails = contactDetails,
            }
                .ToExpectedObject()
                .ShouldMatch(lead);

            AssertDynamicEqual(op, lead.OpportunityProposal);
        }

        void LeadcreatedEventIsRaised()
        {
            _proxy.SetCurrentUser(Users.EventSubscriber);
            var createdEvent = _proxy.GetLastEventOfType<LeadCreated>();
            new
            {
                Id = _proxy.GetLastResponseAggregateId(HttpMethod.Post, Resources.Leads),
                _createLeadCmd.ContactDetails,
            }
                .ToExpectedObject()
                .ShouldMatch(createdEvent.Payload);

            AssertDynamicEqual(_createLeadCmd.OpportunityProposal, createdEvent.Payload.OpportunityProposal);
        }

        static CreateLeadCommand CreateLeadCmd()
        {
            var cmd = Auto.Create<CreateLeadCommand>();
            cmd.OpportunityProposal = new { Title = Guid.NewGuid().ToString(), Description = Guid.NewGuid().ToString() };
            return cmd;
        }

        void AssertDynamicEqual(object a, object b)
        {
            Assert.Equal(Newtonsoft.Json.JsonConvert.SerializeObject(a), Newtonsoft.Json.JsonConvert.SerializeObject(b));
        }
        void TheyHaveSubmitedTheirContactDetails()
        {
            _createLeadCmd = CreateLeadCmd();
            _proxy.Post("/api/leads", _createLeadCmd);
        }
        void TheySubmitAFulllyPopulatedLead()
        {
            _opportunityProposal = new { Title = Guid.NewGuid().ToString(), Description = Guid.NewGuid().ToString() };
            _createLeadCmd = CreateLeadCmd();
            _createLeadCmd.OpportunityProposal = _opportunityProposal;
            _proxy.Post("/api/leads", _createLeadCmd);
        }
        void TheOpportunityDetailsAreOnTheLead()
        {
            var response = _proxy.Get<LeadDetail>(_proxy.GetLastResponseHeaderLocation(HttpMethod.Post, Resources.Leads));
            AssertDynamicEqual(_opportunityProposal, response.OpportunityProposal);
        }

        void AnUpdateCommandIsMadeWithTheLeadId()
        {
            var leadId = _proxy.GetLastResponseAggregateId(HttpMethod.Post, Resources.Leads);
            _updateLeadCmd = Auto.Build<UpdateLeadCommand>().With(cmd => cmd.Id, leadId).Create();
            _updateLeadCmd.OpportunityProposal = new { Title = Guid.NewGuid().ToString(), Description = Guid.NewGuid().ToString() };
            _proxy.Put(_proxy.GetLastResponseHeaderLocation(HttpMethod.Post), _updateLeadCmd);
        }

        void AValidLeadExists()
        {
            _createLeadCmd = CreateLeadCmd();
            _proxy.Post("/api/leads", _createLeadCmd);
        }
        void TheySubmitTheirContactDetailsWithNoName()
        {
            _createLeadCmd = CreateLeadCmd();
            _createLeadCmd.ContactDetails.ContactName = null;
            _createLeadCmd.ContactDetails.OrganiastionName = null;
            _proxy.Post("/api/leads", _createLeadCmd);
        }
        void NoLeadsAreCreated()
        {
            NoEntityIsCreated("Leads");
        }
        private void NoEntityIsCreated(string entitytype)
        {
            var response = _proxy.Get("api/" + entitytype);
            response.EnsureSuccessStatusCode();
            _proxy.SetLastResponse(response);
            Assert.Empty(response.Content.ReadAsAsync<object[]>().Result);
        }

        void TheySubmitTheirContactDetailsWithNoContact()
        {
            _createLeadCmd = CreateLeadCmd();
            _createLeadCmd.ContactDetails.Contacts = new Contact[] { };
            _proxy.Post("/api/leads", _createLeadCmd);
        }

        void TheyModifyTheirContactDetailsWithNoContact()
        {
            var leadId = _proxy.GetLastResponseAggregateId(HttpMethod.Post, Resources.Leads);
            _updateLeadCmd = Auto.Build<UpdateLeadCommand>().With(cmd => cmd.Id, leadId).Create();
            _updateLeadCmd.ContactDetails.Contacts = new Contact[] { };
            _updateLeadCmd.OpportunityProposal = new { Title = Guid.NewGuid().ToString(), Description = Guid.NewGuid().ToString() };
            _proxy.Put(_proxy.GetLastResponseHeaderLocation(HttpMethod.Post), _updateLeadCmd);
        }

        void TheyModifyTheirContactDetailsWithNoName()
        {
            var leadId = _proxy.GetLastResponseAggregateId(HttpMethod.Post, Resources.Leads);
            _updateLeadCmd = Auto.Build<UpdateLeadCommand>().With(cmd => cmd.Id, leadId).Create();
            _updateLeadCmd.ContactDetails.ContactName = null;
            _updateLeadCmd.ContactDetails.OrganiastionName = null;
            _updateLeadCmd.OpportunityProposal = new { Title = Guid.NewGuid().ToString(), Description = Guid.NewGuid().ToString() };
            _proxy.Put(_proxy.GetLastResponseHeaderLocation(HttpMethod.Post), _updateLeadCmd);
        }

        void AnUpdateCommandIsMadeWithAnInvalidId()
        {
            _proxy.Put("/api/leads/" + Guid.NewGuid(), Auto.Create<UpdateLeadCommand>());

        }

        void ADeleteCommandIsMadeWithAnInvalidId()
        {
            _proxy.Delete("/api/leads/" + Guid.NewGuid());

        }
        void ALeadHasBeenPreviouslyDeleted()
        {
            _createLeadCmd = CreateLeadCmd();
            _proxy.Post("/api/leads", _createLeadCmd);
            _proxy.Delete(_proxy.GetLastResponseHeaderLocation(HttpMethod.Post));
            _proxy.IgnoreAllPriorEvents();
        }

        void ADeleteCommandIsMadeWithTheLeadId()
        {
            _proxy.Delete(_proxy.GetLastResponseHeaderLocation(HttpMethod.Post));
        }

        void TheLeadCanNotBeRetrievedById()
        {
            var response = _proxy.Get(_proxy.GetLastResponseHeaderLocation(HttpMethod.Post, Resources.Leads));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        void TheLeadDoesNotAppearInTheLeadList()
        {
            var deletedId = _proxy.GetLastResponseAggregateId(HttpMethod.Post, Resources.Leads);
            var leadIds = _proxy.Get<IEnumerable<LeadDetail>>("api/leads").Select(x => x.Id);
            Assert.DoesNotContain(deletedId, leadIds);
        }

        void TheLeadIsVetted()
        {
            var lead = _proxy.Get<LeadDetail>(_proxy.GetLastResponseHeaderLocation(HttpMethod.Post));
            _proxy.Post("/api/opportunities", new CreateOpportunityCommand { ContactDetails = lead.ContactDetails, LeadId = lead.Id });
        }

        void TheOpportunityCanBeRetrievedById()
        {
            var response = _proxy.Get<OpportunityDetail>(_proxy.GetLastResponseHeaderLocation(HttpMethod.Post));
            Assert.NotNull(response);
            _createLeadCmd.ContactDetails.ToExpectedObject().ShouldMatch(response.ContactDetails);
        }


        void NoEventsAreRaised()
        {
            Assert.Null(_proxy.GetLastEventOfType<LeadUpdated>());
            Assert.Null(_proxy.GetLastEventOfType<LeadCreated>());
        }

        void LeaddeletedEventIsRaised()
        {
            var deleteEvent = _proxy.GetLastEventOfType<LeadDeleted>();
            Assert.NotNull(deleteEvent);
            Assert.Equal(_proxy.GetLastResponseAggregateId(HttpMethod.Post, Resources.Leads), deleteEvent.AggregateId);
        }

        void LeadupdatedEventIsRaised()
        {
            _proxy.SetCurrentUser(Users.EventSubscriber);
            var updated = _proxy.GetLastEventOfType<LeadUpdated>();
            new
            {
                Id = _proxy.GetLastResponseAggregateId(HttpMethod.Post, Resources.Leads),
                _updateLeadCmd.ContactDetails,
            }
                .ToExpectedObject()
                .ShouldMatch(updated.Payload);

            AssertDynamicEqual(_updateLeadCmd.OpportunityProposal, updated.Payload.OpportunityProposal);
        }

        void NoLeadupdatedEventIsRaised()
        {
            Assert.Null(_proxy.GetLastEventOfType<LeadUpdated>());
        }

        void NoLeaddeletedEventIsRaised()
        {
            Assert.Null(_proxy.GetLastEventOfType<LeadDeleted>());
        }

        void LeadvettedEventIsRaised()
        {
            var leadVetted = _proxy.GetLastEventOfType<LeadVetted>();
            Assert.NotNull(leadVetted);
            Assert.Equal(_proxy.GetLastResponseAggregateId(HttpMethod.Post, Resources.Opportunities), leadVetted.Payload.OpportunityId);
        }

        void OpportunitycreatedEventIsRaised()
        {
            var opportunityCreated = _proxy.GetLastEventOfType<OpportunityCreated>();
            Assert.NotNull(opportunityCreated);
            Assert.Equal(_proxy.GetLastResponseAggregateId(HttpMethod.Post, Resources.Opportunities), opportunityCreated.OpportunityDetail.Id);
            _createLeadCmd.ContactDetails.ToExpectedObject().ShouldEqual(opportunityCreated.OpportunityDetail.ContactDetails);
        }
    }
}
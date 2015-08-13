using System.Net;
using TestStack.BDDfy;
using Xunit;

namespace Matcha.WebApi.Tests.Features.Leads
{
    public partial class LeadScenarios
    {
        [Fact]
        public void CreateALead()
        {
            this.Given(s => s.AnAnonymousUserUsingTheApi())
            .When(s => s.TheySubmitTheirContactDetails())
            .Then(s => s.TheNewIdIsReturned())
            .And(s => s.AStatusCodeIsReturned(HttpStatusCode.Created))
            .And(s => s.ASalesadminUserCanRetrieveTheLead())
            .And(s => s.LeadcreatedEventIsRaised())
            .BDDfy();
        }

        [Fact]
        public void CreateALeadWithOpportunityDetails()
        {
            this.Given(s => s.AnAnonymousUserUsingTheApi())
            .When(s => s.TheySubmitAFulllyPopulatedLead())
            .Then(s => s.TheNewIdIsReturned())
            .And(s => s.AStatusCodeIsReturned(HttpStatusCode.Created))
            .And(s => s.ASalesadminUserCanRetrieveTheLead())
            .And(s => s.TheOpportunityDetailsAreOnTheLead())
            .And(s => s.LeadcreatedEventIsRaised())
            .BDDfy();
        }

        [Fact]
        public void ContinueToAddToALead()
        {
            this.Given(s => s.AnAnonymousUserUsingTheApi())
            .And(s => s.TheyHaveSubmitedTheirContactDetails())
            .When(s => s.AnUpdateCommandIsMadeWithTheLeadId())
            .Then(s => s.ASalesadminUserCanRetrieveTheLead())
            .And(s => s.AStatusCodeIsReturned(HttpStatusCode.OK))
            .And(s => s.LeadupdatedEventIsRaised())
            .BDDfy();
        }

        [Fact]
        public void SalesUserUpdateALead()
        {
            this.Given(s => s.AnSalesUserUsingTheApi())
            .And(s => s.AValidLeadExists())
            .When(s => s.AnUpdateCommandIsMadeWithTheLeadId())
            .Then(s => s.AStatusCodeIsReturned(HttpStatusCode.OK))
            .And(s => s.LeadupdatedEventIsRaised())
            .And(s => s.ASalesadminUserCanRetrieveTheLead())
            .BDDfy();
        }

        [Fact]
        public void AttemptToCreateALeadWithoutAName()
        {
            this.Given(s => s.AnAnonymousUserUsingTheApi())
            .When(s => s.TheySubmitTheirContactDetailsWithNoName())
            .Then(s => s.AStatusCodeIsReturned(HttpStatusCode.BadRequest))
            .And(s => s.NoLeadsAreCreated())
            .And(s => s.NoEventsAreRaised())
            .BDDfy();
        }

        [Fact]
        public void AttemptToCreateALeadWithoutAContact()
        {
            this.Given(s => s.AnAnonymousUserUsingTheApi())
            .When(s => s.TheySubmitTheirContactDetailsWithNoContact())
            .Then(s => s.AStatusCodeIsReturned(HttpStatusCode.BadRequest))
            .And(s => s.NoLeadsAreCreated())
            .And(s => s.NoEventsAreRaised())
            .BDDfy();
        }

        [Fact]
        public void AttemptToUpdateALeadWithoutAContact()
        {
            this.Given(s => s.AnAnonymousUserUsingTheApi())
            .And(s => s.TheyHaveSubmitedTheirContactDetails())
            .When(s => s.TheyModifyTheirContactDetailsWithNoContact())
            .Then(s => s.AStatusCodeIsReturned(HttpStatusCode.BadRequest))
            .And(s => s.NoLeadupdatedEventIsRaised())
            .BDDfy();
        }

        [Fact]
        public void AttemptToUpdateALeadWithoutAName()
        {
            this.Given(s => s.AnAnonymousUserUsingTheApi())
            .And(s => s.TheyHaveSubmitedTheirContactDetails())
            .When(s => s.TheyModifyTheirContactDetailsWithNoName())
            .Then(s => s.AStatusCodeIsReturned(HttpStatusCode.BadRequest))
            .And(s => s.NoLeadupdatedEventIsRaised())
            .BDDfy();
        }

        [Fact]
        public void AttemptToUpdateAnInvalidLead()
        {
            this.Given(s => s.AnSalesUserUsingTheApi())
            .And(s => s.AValidLeadExists())
            .When(s => s.AnUpdateCommandIsMadeWithAnInvalidId())
            .Then(s => s.AStatusCodeIsReturned(HttpStatusCode.NotFound))
            .And(s => s.NoLeadupdatedEventIsRaised())
            .BDDfy();
        }

        [Fact]
        public void DeleteANonExistantLead()
        {
            this.Given(s => s.AnSalesUserUsingTheApi())
            .And(s => s.AValidLeadExists())
            .When(s => s.ADeleteCommandIsMadeWithAnInvalidId())
            .Then(s => s.AStatusCodeIsReturned(HttpStatusCode.NotFound))
            .And(s => s.NoLeaddeletedEventIsRaised())
            .And(s => s.ASalesadminUserCanRetrieveTheLead())
            .BDDfy();
        }

        [Fact]
        public void DeleteADeletedLead()
        {
            this.Given(s => s.AnSalesUserUsingTheApi())
            .And(s => s.ALeadHasBeenPreviouslyDeleted())
            .When(s => s.ADeleteCommandIsMadeWithTheLeadId())
            .Then(s => s.AStatusCodeIsReturned(HttpStatusCode.NotFound))
            .And(s => s.NoLeaddeletedEventIsRaised())
            .And(s => s.TheLeadCanNotBeRetrievedById())
            .BDDfy();
        }

        [Fact]
        public void DeleteALead()
        {
            this.Given(s => s.AnSalesUserUsingTheApi())
            .And(s => s.AValidLeadExists())
            .When(s => s.ADeleteCommandIsMadeWithTheLeadId())
            .Then(s => s.AStatusCodeIsReturned(HttpStatusCode.NoContent))
            .And(s => s.TheLeadCanNotBeRetrievedById())
            .And(s => s.TheLeadDoesNotAppearInTheLeadList())
            .And(s => s.LeaddeletedEventIsRaised())
            .BDDfy();
        }

        [Fact]
        public void VettingALead()
        {
            this.Given(s => s.AnSalesUserUsingTheApi())
            .And(s => s.AValidLeadExists())
            .When(s => s.TheLeadIsVetted())
            .Then(s => s.AStatusCodeIsReturned(HttpStatusCode.Created))
            .And(s => s.TheLeadCanNotBeRetrievedById())
            .And(s => s.TheLeadDoesNotAppearInTheLeadList())
            .And(s => s.TheOpportunityCanBeRetrievedById())
            .And(s => s.LeadvettedEventIsRaised())
            .And(s => s.OpportunitycreatedEventIsRaised())
            .BDDfy();
        }
    }
}
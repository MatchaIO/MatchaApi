using System;
using TechTalk.SpecFlow;
using Xunit;

namespace Matcha.WebApi.Specifications.LeadFeatures
{
    [Binding]
    public class ControllerSteps
    {
        [Given(@"an anonymous user using the api")]
        public void GivenAnAnonymousUserUsingTheApi()
        {
            ScenarioContext.Current.SetCurrentUser(Users.Anon);
        }

        [Then(@"the new Id is returned")]
        public void ThenTheIdIsReturned()
        {
            var lastCreatedId = ScenarioContext.Current.GetLastPostResponseAggregateId();
            Assert.NotEqual(default(Guid), lastCreatedId);
        }
    }
}
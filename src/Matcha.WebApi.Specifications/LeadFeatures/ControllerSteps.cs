using System;
using System.Net;
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

        [Then(@"a (.*) is returned")]
        public void ThenTheStatusIsReturned(HttpStatusCode code)
        {
            Assert.Equal(code, ScenarioContext.Current.GetLastResponseStatusCode());
        }
    }
}
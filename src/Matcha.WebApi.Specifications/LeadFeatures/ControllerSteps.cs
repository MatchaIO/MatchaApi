using System;
using System.Net;
using TechTalk.SpecFlow;
using Xunit;

namespace Matcha.WebApi.Specifications.LeadFeatures
{
    [Binding]
    public class ControllerSteps
    {
        [Given(@"an (.*) user using the api")]
        public void GivenAUserUsingTheApi(string userAsString)
        {
            Users user; 
            switch (userAsString.ToLowerInvariant())
            {
                case "sales":
                    user = Users.SalesAdmin;
                    break;
                case "anonymous":
                    user = Users.Anon;
                    break;
                case "eventsubscriber":
                    user = Users.EventSubscriber;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            ScenarioContext.Current.SetCurrentUser(user);
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
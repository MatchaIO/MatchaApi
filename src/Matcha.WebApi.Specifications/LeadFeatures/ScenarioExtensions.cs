using System;
using System.Linq;
using System.Net.Http.Headers;
using TechTalk.SpecFlow;

namespace Matcha.WebApi.Specifications.LeadFeatures
{
    public static class ScenarioExtensions
    {
        public static void SetLastPostHeaders(this ScenarioContext context, HttpResponseHeaders headers)
        {
            context.Set<HttpResponseHeaders>(headers);
        }

        public static Guid GetLastPostResponseAggregateId(this ScenarioContext context)
        {
            var lastHeader = context.Get<HttpResponseHeaders>();
            return Guid.Parse(lastHeader.Location.ToString().Split('/').Last());
        }
        public static string GetLastPostResponseHeaderLocation(this ScenarioContext context)
        {
            var lastHeader = context.Get<HttpResponseHeaders>();
            return lastHeader.Location.PathAndQuery;
        }

        public static void SetCurrentUser(this ScenarioContext context, Users user)
        {
            context.Set(user);
        }
        public static Users GetCurrentUser(this ScenarioContext context)
        {
            return context.Get<Users>();
        }
    }
}
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using TechTalk.SpecFlow;

namespace Matcha.WebApi.Specifications.LeadFeatures
{
    public static class ScenarioExtensions
    {
        public static void SetLastResponse(this ScenarioContext context, HttpResponseMessage response)
        {
            context.Set(response);
        }
        public static HttpStatusCode GetLastResponseStatusCode(this ScenarioContext context)
        {
            return context.Get<HttpResponseMessage>().StatusCode;
        }
        public static void SetLastPostHeaders(this ScenarioContext context, HttpResponseHeaders headers)
        {
            context.Set(headers);
        }
        public static void EnsureLastReponseSuccessStatusCode(this ScenarioContext context)
        {
            context.Get<HttpResponseMessage>().EnsureSuccessStatusCode();
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
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using TechTalk.SpecFlow;

namespace Matcha.WebApi.Specifications.LeadFeatures
{
    public static class ScenarioExtensions
    {
        public static void SetLastResponse(this ScenarioContext context, HttpResponseMessage response)
        {
            context.Set(response, response.RequestMessage.Method.Method);
            context.Set(response, "LAST");
        }

        public static HttpStatusCode GetLastResponseStatusCode(this ScenarioContext context)
        {
            return context.Get<HttpResponseMessage>("LAST").StatusCode;
        }

        public static HttpStatusCode GetLastResponseStatusCode(this ScenarioContext context, HttpMethod method)
        {
            return context.Get<HttpResponseMessage>(method.Method).StatusCode;
        }

        public static string GetLastResponseHeaderLocation(this ScenarioContext context, HttpMethod method)
        {
            var lastHeader = context.Get<HttpResponseMessage>(method.Method).Headers;
            return lastHeader.Location.PathAndQuery;
        }

        public static Guid GetLastResponseAggregateId(this ScenarioContext context, HttpMethod method)
        {
            var lastHeader = context.Get<HttpResponseMessage>(method.Method).Headers;
            return Guid.Parse(lastHeader.Location.ToString().Split('/').Last());
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
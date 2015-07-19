using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using TechTalk.SpecFlow;

namespace Matcha.WebApi.Specifications.LeadFeatures
{
    public enum Resources
    {
        Leads,
        Opportunities
    }

    public static class ScenarioExtensions
    {
        public static void SetLastResponse(this ScenarioContext context, HttpResponseMessage response)
        {
            List<HttpResponseMessage> x;
            if (!context.TryGetValue(out x))
            {
                context.Set(new List<HttpResponseMessage>());
            }
            context.Get<List<HttpResponseMessage>>().Add(response);
        }

        public static HttpStatusCode GetLastResponseStatusCode(this ScenarioContext context)
        {
            return context.Get<List<HttpResponseMessage>>().Last().StatusCode;
        }

        public static string GetLastResponseHeaderLocation(this ScenarioContext context, HttpMethod method, Resources? resource = null)
        {
            var lastHeader = context.GetLastHeaders(method, resource);
            return lastHeader.Location.PathAndQuery;
        }

        public static Guid GetLastResponseAggregateId(this ScenarioContext context, HttpMethod method, Resources? resource = null)
        {
            var lastHeader = context.GetLastHeaders(method, resource);
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

        private static HttpResponseHeaders GetLastHeaders(this ScenarioContext context, HttpMethod method, Resources? resource = null)
        {
            var query = context.Get<List<HttpResponseMessage>>()
                .Where(x => x.RequestMessage.Method == method);
            if (resource != null)
            {
                query = query.Where(x => GetResource(x) == resource);
            }
            return query.Last().Headers;
        }

        private static Resources GetResource(HttpResponseMessage response)
        {
            var uri = response.RequestMessage.RequestUri;
            var resourceAsString = uri.PathAndQuery.Split('/').Where(s => !string.IsNullOrWhiteSpace(s)).Skip(1).First();
            return (Resources)Enum.Parse(typeof(Resources), resourceAsString, ignoreCase: true);
        }
    }
}
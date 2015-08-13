using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Matcha.WebApi.Domain.Events;

namespace Matcha.WebApi.Tests
{
    public class WebClientProxy
    {
        private readonly HttpClient _httpClient;
        private Guid[] _eventIdsToIgnore = new Guid[0];
        private readonly List<HttpResponseMessage> _responses = new List<HttpResponseMessage>();

        public WebClientProxy(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void SetCurrentUser(Users anon)
        {
            //throw new NotImplementedException();
        }

        public Guid GetLastResponseAggregateId(HttpMethod method, Resources? resource = null)
        {
            var lastHeader = GetLastHeaders(method, resource);
            return Guid.Parse(lastHeader.Location.ToString().Split('/').Last());
        }


        public HttpStatusCode GetLastResponseStatusCode()
        {
            return _responses.Last().StatusCode;
        }

        public void Post<TRequest>(string urlPath, TRequest request)
        {
            var postResponse = _httpClient.PostAsJsonAsync(
                urlPath,
                request).Result;
            SetLastResponse(postResponse);
        }

        public void Put<TRequest>(string urlPath, TRequest request)
        {
            var putResponse = _httpClient.PutAsJsonAsync(
                urlPath,
                request).Result;
            SetLastResponse(putResponse);
        }

        public void Delete(string urlPath)
        {
            var putResponse = _httpClient.DeleteAsync(urlPath).Result;
            SetLastResponse(putResponse);
        }

        public T Get<T>(Uri urlPath)
        {
            return Get<T>(urlPath.PathAndQuery);
        }

        public T Get<T>(string urlPath)
        {
            var response = _httpClient.GetAsync(urlPath).Result;
            response.EnsureSuccessStatusCode();
            SetLastResponse(response);
            return response.Content.ReadAsAsync<T>().Result;
        }

        public HttpResponseMessage Get(string urlPath)
        {
            return _httpClient.GetAsync(urlPath).Result;
        }

        public T GetLastEventOfType<T>() where T : Event
        {
            return GetEventsOfType<T>().LastOrDefault();
        }

        public IEnumerable<T> GetEventsOfType<T>() where T : Event
        {
            var requestUri = "api/Events/ByType/" + typeof(T).FullName;
            var getResponse = _httpClient.GetAsync(requestUri).Result;
            getResponse.EnsureSuccessStatusCode();
            var result = getResponse.Content.ReadAsAsync<T[]>().Result;
            return result.Where(e => !_eventIdsToIgnore.Contains(e.EventId));
        }

        public void IgnoreAllPriorEvents()
        {
            var getResponse = _httpClient.GetAsync("api/Events/").Result;
            getResponse.EnsureSuccessStatusCode();
            _eventIdsToIgnore = getResponse.Content.ReadAsAsync<Event[]>().Result.Select(e => e.EventId).ToArray();
        }

        internal void SetLastResponse(HttpResponseMessage response)
        {
            _responses.Add(response);
        }
        private HttpResponseHeaders GetLastHeaders(HttpMethod method, Resources? resource = null)
        {
            var query = _responses.Where(x => x.RequestMessage.Method == method);
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

        public string GetLastResponseHeaderLocation(HttpMethod method, Resources? resource = null)
        {
            var lastHeader = GetLastHeaders(method, resource);
            return lastHeader.Location.PathAndQuery;
        }
    }
}
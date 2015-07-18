using System;
using System.Linq;
using System.Net.Http;
using Autofac;
using Matcha.WebApi.Handlers;
using Matcha.WebApi.Specifications.LeadFeatures;
using Microsoft.Owin.Testing;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using TechTalk.SpecFlow;

namespace Matcha.WebApi.Specifications
{
    public class MatchaFixtureContext
    {
        private readonly TestServer _server;

        internal HttpClient HttpClient
        {
            get
            {
                var currentuser = ScenarioContext.Current.GetCurrentUser();//TODO - set user
                var httpClient = _server.HttpClient;
                return httpClient;
            }
        }

        public MatchaFixtureContext()
        {
            _server = TestServer.Create<TestStartup>();

            var container = TestStartup.Container;
            var configuration = container.Resolve<NHibernate.Cfg.Configuration>();

            new SchemaExport(configuration).Execute(
                useStdOut: false,
                execute: true,
                justDrop: false,
                connection: container.Resolve<ISession>().Connection,
                exportOutput: null //Console.Out
                );
        }

        public void Post<TRequest>(string urlPath, TRequest request)
        {
            var postResponse = HttpClient.PostAsJsonAsync(
               urlPath,
               request).Result;
            ScenarioContext.Current.SetLastResponse(postResponse);
        }

        public void Put<TRequest>(string urlPath, TRequest request)
        {
            var putResponse = HttpClient.PutAsJsonAsync(
                urlPath,
                request).Result;
            ScenarioContext.Current.SetLastResponse(putResponse);
        }
        
        public void Delete(string urlPath)
        {
            var putResponse = HttpClient.DeleteAsync(urlPath).Result;
            ScenarioContext.Current.SetLastResponse(putResponse);
        }

        public T Get<T>(Uri urlPath)
        {
            return Get<T>(urlPath.PathAndQuery);
        }

        public T Get<T>(string urlPath)
        {
            var response = HttpClient.GetAsync(urlPath).Result;
            response.EnsureSuccessStatusCode();
            ScenarioContext.Current.SetLastResponse(response);
            return response.Content.ReadAsAsync<T>().Result;
        }
            
        public HttpResponseMessage Get(string urlPath)
        {
            return HttpClient.GetAsync(urlPath).Result;
        }



        public T GetLastEventOfType<T>() where T : Event
        {
            var requestUri = "api/Events/ByType/" + typeof(T).Name;
            var getResponse = HttpClient.GetAsync(requestUri).Result;
            getResponse.EnsureSuccessStatusCode();
            return getResponse.Content.ReadAsAsync<T[]>().Result.LastOrDefault();
        }

        
    }
}
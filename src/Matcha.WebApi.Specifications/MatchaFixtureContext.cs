using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Autofac;
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
            var currentuser = ScenarioContext.Current.GetCurrentUser();//TODO
            var httpClient = _server.HttpClient;
            var postResponse = httpClient.PostAsJsonAsync(
                urlPath,
                request).Result;
            postResponse.EnsureSuccessStatusCode();
            ScenarioContext.Current.SetLastPostHeaders(postResponse.Headers); 
        }

        public T Get<T>(Uri urlPath)
        {
            return Get<T>(urlPath.PathAndQuery);
        }

        public T Get<T>(string urlPath)
        {
            var currentuser = ScenarioContext.Current.GetCurrentUser();//TODO
            var httpClient = _server.HttpClient;
            var response = httpClient.GetAsync(urlPath).Result;
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsAsync<T>().Result;
        }

        public T GetLastEventOfType<T>()
        {
            var currentuser = ScenarioContext.Current.GetCurrentUser();//TODO
            var requestUri = "api/Events/ByType/" + typeof(T).Name;
            var getResponse = _server.HttpClient.GetAsync(requestUri).Result;
            getResponse.EnsureSuccessStatusCode();
            return getResponse.Content.ReadAsAsync<T[]>().Result.Last();
        }
    }
}
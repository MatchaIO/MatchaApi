using System;
using System.Linq;
using System.Net.Http;
using Autofac;
using Matcha.WebApi.Handlers;
using Matcha.WebApi.Messages.Commands;
using Microsoft.Owin.Testing;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Matcha.WebApi.Specifications
{
    public class MatchaFixtureContext
    {
        private readonly TestServer _server;
        private readonly IContainer _container;
        private Users _currentUser;

        public MatchaFixtureContext()
        {
            _server = TestServer.Create<TestStartup>();

            _container = TestStartup.Container;
            _container.Resolve<ICommandHandler<CreateLeadCommand, Guid>>().ToString();
            var configuration = _container.Resolve<NHibernate.Cfg.Configuration>();

            new SchemaExport(configuration).Execute(
                useStdOut: false,
                execute: true,
                justDrop: false,
                connection: _container.Resolve<ISession>().Connection,
                exportOutput: null //Console.Out
                );
        }

        public Uri Post<TRequest>(string urlPath, TRequest request)
        {
            var httpClient = _server.HttpClient;
            var postResponse = httpClient.PostAsJsonAsync(
                urlPath,
                request).Result;
            postResponse.EnsureSuccessStatusCode();
            return postResponse.Headers.Location;
        }

        public T Get<T>(Uri urlPath)
        {
            var httpClient = _server.HttpClient;
            var response = httpClient.GetAsync(urlPath.PathAndQuery).Result;
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsAsync<T>().Result;
        }

        public void SetCurrentUser(Users user)
        {
            _currentUser = user;
        }

        public T GetLastEventOfType<T>()
        {
            var requestUri = "api/Events/ByType/" + typeof(T).Name;
            var getResponse = _server.HttpClient.GetAsync(requestUri).Result;
            getResponse.EnsureSuccessStatusCode();
            return getResponse.Content.ReadAsAsync<T[]>().Result.Last();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Matcha.WebApi.Handlers;
using NHibernate;
using NHibernate.Linq;

namespace Matcha.WebApi.Domain.Events
{
    public class SqlEventPublisher : IEventPublisher, IEventRepository
    {
        private readonly ISession _session;
        private static readonly string EventName = typeof (Event).FullName;
        public SqlEventPublisher(ISession session)
        {
            _session = session;
        }

        public void Publish(Event eventToPublish)
        {
            _session.Save(EventName, eventToPublish);
            _session.Flush();
        }

        public IEnumerable<Event> EventsOfType(string eventType)
        {
            return _session.Query<Event>().Where(e => e.EventType == eventType);
        }

        public IEnumerable<Event> GetAll()
        {
            return _session.Query<Event>().ToList();
        }
    }
}
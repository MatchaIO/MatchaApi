using System;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Matcha.WebApi.Domain.Events
{
    public class Event
    {
        protected Event()
        {
            EventId = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
            EventType = GetType().FullName;
        }
        public virtual Guid EventId { get; set; }
        public virtual DateTime Timestamp { get; set; }
        public virtual Guid? AggregateId { get; set; }
        public virtual string EventType { get; set; }
        public virtual string EventPayloadType { get; set; }
        public virtual string EventPayloadAsJson { get; set; }
    }

    public class Event<TPayload> : Event where TPayload : class
    {
        public Event()
        {
            EventPayloadType = typeof(TPayload).FullName;
        }
        public virtual TPayload Payload
        {
            get
            {
                return string.IsNullOrWhiteSpace(EventPayloadAsJson) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<TPayload>(EventPayloadAsJson);
            }
            set
            {
                EventPayloadAsJson = value == null ? null : Newtonsoft.Json.JsonConvert.SerializeObject(value);
            }
        }
    }

    public class EventMap : ClassMapping<Event>
    {
        public EventMap()
        {
            Id(x => x.EventId, m => m.Generator(Generators.Assigned));
            Property(x => x.AggregateId);
            Property(x => x.Timestamp);
            Property(x => x.EventType);
            Property(x => x.EventPayloadType);
            Property(x => x.EventPayloadAsJson);            
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Text;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Matcha.WebApi.Domain.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Matcha.WebApi.Domain.DataAccess.EventStoreImpl
{
    public class LeadRepository : ILeadRepository
    {
        private readonly IEventRepository _eventRepository;
        private static readonly Func<Lead, bool> IsValid = l => !l.IsDeleted && !l.IsVetted;

        public LeadRepository(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public Lead GetLeadById(Guid id)
        {
            var lead = _eventRepository.GetById<Lead>(id);
            if (lead == null || !IsValid(lead))
                throw EntityNotFoundException.From<Lead>(id);
            return lead;
        }

        public IEnumerable<Lead> GetAllCurrentLeads()
        {
            throw new NotImplementedException("Projections not yet working");
            //return _session.Query<Lead>().Where(IsValid);
        }

        public void Store(Lead lead)
        {
            _eventRepository.Save(lead, Guid.NewGuid(), _ => { });
        }
    }

    public interface IEventRepository
    {
        TAggregate GetById<TAggregate>(Guid id) where TAggregate : class, IEventBasedAggregate;
        TAggregate GetById<TAggregate>(Guid id, int version) where TAggregate : class, IEventBasedAggregate;
        void Save(IEventBasedAggregate aggregate, Guid commitId, Action<IDictionary<string, object>> updateHeaders);
        ResolvedEvent[] GetLastEvents(int count = 20);
    }

    public class AggregateVersionException : Exception
    {
        public readonly Guid Id;
        public readonly Type Type;
        public readonly int AggregateVersion;
        public readonly int RequestedVersion;

        public AggregateVersionException(Guid id, Type type, int aggregateVersion, int requestedVersion)
            : base(string.Format("Requested version {2} of aggregate '{0}' (type {1}) - aggregate version is {3}", id, type.Name, requestedVersion, aggregateVersion))
        {
            Id = id;
            Type = type;
            AggregateVersion = aggregateVersion;
            RequestedVersion = requestedVersion;
        }
    }

    public class AggregateDeletedException : Exception
    {
        public readonly Guid Id;
        public readonly Type Type;

        public AggregateDeletedException(Guid id, Type type)
            : base(string.Format("Aggregate '{0}' (type {1}) was deleted.", id, type.Name))
        {
            Id = id;
            Type = type;
        }
    }

    public class AggregateNotFoundException : Exception
    {
        public readonly Guid Id;
        public readonly Type Type;

        public AggregateNotFoundException(Guid id, Type type)
            : base(string.Format("Aggregate '{0}' (type {1}) was not found.", id, type.Name))
        {
            Id = id;
            Type = type;
        }
    }
    public class HandlerForDomainEventNotFoundException : Exception
    {
        public HandlerForDomainEventNotFoundException()
        {
        }

        public HandlerForDomainEventNotFoundException(string message)
            : base(message)
        {
        }

        public HandlerForDomainEventNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public HandlerForDomainEventNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    public interface IEventBasedAggregate
    {
        Guid Id { get; }
        int Version { get; }

        void ApplyEvent(object @event);
        ICollection GetUncommittedEvents();
        void ClearUncommittedEvents();
    }

    /// <summary>
    /// This is from https://geteventstore.com/blog/20130220/getting-started-part-2-implementing-the-commondomain-repository-interface/
    /// However the API has changed somewhat including factory methods and asynch behaviour 
    /// </summary>
    public class GetEventStoreRepository : IEventRepository
    {
        private const string EventClrTypeHeader = "EventClrTypeName";
        private const string AggregateClrTypeHeader = "AggregateClrTypeName";
        private const string CommitIdHeader = "CommitId";
        private const int WritePageSize = 500;
        private const int ReadPageSize = 500;

        private readonly Func<Type, Guid, string> _aggregateIdToStreamName;

        private readonly IEventStoreConnection _eventStoreConnection;
        private static readonly JsonSerializerSettings SerializerSettings;

        static GetEventStoreRepository()
        {
            SerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };
        }

        public GetEventStoreRepository(IEventStoreConnection eventStoreConnection)
            : this(eventStoreConnection, (t, g) => string.Format("{0}-{1}", char.ToLower(t.Name[0]) + t.Name.Substring(1), g.ToString("N")))
        {
        }

        public GetEventStoreRepository(IEventStoreConnection eventStoreConnection, Func<Type, Guid, string> aggregateIdToStreamName)
        {
            _eventStoreConnection = eventStoreConnection;
            _aggregateIdToStreamName = aggregateIdToStreamName;
        }

        public ResolvedEvent[] GetLastEvents(int count = 20)
        {
            //TODO - get a real user set up
            var task = _eventStoreConnection.ReadAllEventsBackwardAsync(Position.End, count, true,
                //new UserCredentials("EventReader", "Password1")
                new UserCredentials("admin", "changeit")
                );
            var result = task.Result;
            return result.Events;
        }

        public TAggregate GetById<TAggregate>(Guid id) where TAggregate : class, IEventBasedAggregate
        {
            return GetById<TAggregate>(id, int.MaxValue);
        }

        public TAggregate GetById<TAggregate>(Guid id, int version) where TAggregate : class, IEventBasedAggregate
        {
            if (version <= 0)
                throw new InvalidOperationException("Cannot get version <= 0");

            var streamName = _aggregateIdToStreamName(typeof(TAggregate), id);
            var aggregate = ConstructAggregate<TAggregate>();

            var sliceStart = 0;
            StreamEventsSlice currentSlice;
            do
            {
                var sliceCount = sliceStart + ReadPageSize <= version
                                    ? ReadPageSize
                                    : version - sliceStart + 1;

                currentSlice = _eventStoreConnection.ReadStreamEventsForwardAsync(streamName, sliceStart, sliceCount, false).Result;//HACK - now async

                if (currentSlice.Status == SliceReadStatus.StreamNotFound)
                    throw new AggregateNotFoundException(id, typeof(TAggregate));

                if (currentSlice.Status == SliceReadStatus.StreamDeleted)
                    throw new AggregateDeletedException(id, typeof(TAggregate));

                sliceStart = currentSlice.NextEventNumber;

                foreach (var evnt in currentSlice.Events)
                    aggregate.ApplyEvent(DeserializeEvent(evnt.OriginalEvent.Metadata, evnt.OriginalEvent.Data));
            } while (version >= currentSlice.NextEventNumber && !currentSlice.IsEndOfStream);

            if (aggregate.Version != version && version < Int32.MaxValue)
                throw new AggregateVersionException(id, typeof(TAggregate), aggregate.Version, version);

            return aggregate;
        }

        private static TAggregate ConstructAggregate<TAggregate>()
        {
            return (TAggregate)Activator.CreateInstance(typeof(TAggregate), true);
        }

        private static object DeserializeEvent(byte[] metadata, byte[] data)
        {
            var eventClrTypeName = JObject.Parse(Encoding.UTF8.GetString(metadata)).Property(EventClrTypeHeader).Value;
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), Type.GetType((string)eventClrTypeName));
        }

        public void Save(IEventBasedAggregate aggregate, Guid commitId, Action<IDictionary<string, object>> updateHeaders)
        {
            var commitHeaders = new Dictionary<string, object>
            {
                {CommitIdHeader, commitId},
                {AggregateClrTypeHeader, aggregate.GetType().AssemblyQualifiedName}
            };
            updateHeaders(commitHeaders);

            var streamName = _aggregateIdToStreamName(aggregate.GetType(), aggregate.Id);
            var newEvents = aggregate.GetUncommittedEvents().Cast<object>().ToList();
            var originalVersion = aggregate.Version - newEvents.Count;
            var expectedVersion = originalVersion == 0 ? ExpectedVersion.NoStream : originalVersion - 1;
            var eventsToSave = newEvents.Select(e => ToEventData(Guid.NewGuid(), e, commitHeaders)).ToList();

            if (eventsToSave.Count < WritePageSize)
            {
                _eventStoreConnection.AppendToStreamAsync(streamName, expectedVersion, eventsToSave).Wait();//HACK - now async
            }
            else
            {
                var transaction = _eventStoreConnection.StartTransactionAsync(streamName, expectedVersion).Result;//HACK - now async

                var position = 0;
                while (position < eventsToSave.Count)
                {
                    var pageEvents = eventsToSave.Skip(position).Take(WritePageSize);
                    transaction.WriteAsync(pageEvents).Wait();//HACK - now async
                    position += WritePageSize;
                }

                transaction.CommitAsync().Wait();//HACK - now async
            }

            aggregate.ClearUncommittedEvents();
        }

        private static EventData ToEventData(Guid eventId, object evnt, IDictionary<string, object> headers)
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evnt, SerializerSettings));

            var eventHeaders = new Dictionary<string, object>(headers)
            {
                {
                    EventClrTypeHeader, evnt.GetType().AssemblyQualifiedName
                }
            };
            var metadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventHeaders, SerializerSettings));
            var typeName = evnt.GetType().Name;

            return new EventData(eventId, typeName, true, data, metadata);
        }
    }

    public abstract class AggregateBase : IEventBasedAggregate, IEquatable<IEventBasedAggregate>
    {
        private readonly ICollection<object> uncommittedEvents = new LinkedList<object>();

        private IRouteEvents registeredRoutes;

        protected AggregateBase()
            : this(null)
        {
        }

        protected AggregateBase(IRouteEvents handler)
        {
            if (handler == null) return;

            this.RegisteredRoutes = handler;
            this.RegisteredRoutes.Register(this);
        }

        public Guid Id { get; protected set; }
        public int Version { get; protected set; }

        protected IRouteEvents RegisteredRoutes
        {
            get
            {
                return registeredRoutes ?? (registeredRoutes = new ConventionEventRouter(true, this));
            }
            set
            {
                if (value == null)
                    throw new InvalidOperationException("AggregateBase must have an event router to function");

                registeredRoutes = value;
            }
        }

        protected void Register<T>(Action<T> route)
        {
            this.RegisteredRoutes.Register(route);
        }

        protected void RaiseEvent(object @event)
        {
            ((IEventBasedAggregate)this).ApplyEvent(@event);
            this.uncommittedEvents.Add(@event);
        }
        void IEventBasedAggregate.ApplyEvent(object @event)
        {
            this.RegisteredRoutes.Dispatch(@event);
            this.Version++;
        }
        ICollection IEventBasedAggregate.GetUncommittedEvents()
        {
            return (ICollection)this.uncommittedEvents;
        }
        void IEventBasedAggregate.ClearUncommittedEvents()
        {
            this.uncommittedEvents.Clear();
        }

        //IMemento IEventBasedAggregate.GetSnapshot()
        //{
        //    var snapshot = this.GetSnapshot();
        //    snapshot.Id = this.Id;
        //    snapshot.Version = this.Version;
        //    return snapshot;
        //}
        //protected virtual IMemento GetSnapshot()
        //{
        //    return null;
        //}

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return this.Equals(obj as IEventBasedAggregate);
        }
        public virtual bool Equals(IEventBasedAggregate other)
        {
            return null != other && other.Id == this.Id;
        }
    }
    public interface IRouteEvents
    {
        void Register<T>(Action<T> handler);
        void Register(IEventBasedAggregate aggregate);

        void Dispatch(object eventMessage);
    }
    public class ConventionEventRouter : IRouteEvents
    {
        readonly bool throwOnApplyNotFound;
        private readonly IDictionary<Type, Action<object>> handlers = new Dictionary<Type, Action<object>>();
        private IEventBasedAggregate registered;

        public ConventionEventRouter()
            : this(true)
        {
        }

        public ConventionEventRouter(bool throwOnApplyNotFound)
        {
            this.throwOnApplyNotFound = throwOnApplyNotFound;
        }

        public ConventionEventRouter(bool throwOnApplyNotFound, IEventBasedAggregate aggregate)
            : this(throwOnApplyNotFound)
        {
            Register(aggregate);
        }

        public virtual void Register<T>(Action<T> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            this.Register(typeof(T), @event => handler((T)@event));
        }

        public virtual void Register(IEventBasedAggregate aggregate)
        {
            if (aggregate == null)
                throw new ArgumentNullException("aggregate");

            this.registered = aggregate;

            // Get instance methods named Apply with one parameter returning void
            var applyMethods = aggregate.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.Name == "Apply" && m.GetParameters().Length == 1 && m.ReturnParameter.ParameterType == typeof(void))
                .Select(m => new
                {
                    Method = m,
                    MessageType = m.GetParameters().Single().ParameterType
                });

            foreach (var apply in applyMethods)
            {
                var applyMethod = apply.Method;
                this.handlers.Add(apply.MessageType, m => applyMethod.Invoke(aggregate, new[] { m as object }));
            }
        }

        public virtual void Dispatch(object eventMessage)
        {
            if (eventMessage == null)
                throw new ArgumentNullException("eventMessage");

            Action<object> handler;
            if (this.handlers.TryGetValue(eventMessage.GetType(), out handler))
                handler(eventMessage);
            else if (this.throwOnApplyNotFound)
                this.registered.ThrowHandlerNotFound(eventMessage);
        }

        private void Register(Type messageType, Action<object> handler)
        {
            this.handlers[messageType] = handler;
        }
    }
    internal static class ExtensionMethods
    {
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format ?? string.Empty, args);
        }

        public static void ThrowHandlerNotFound(this IEventBasedAggregate aggregate, object eventMessage)
        {
            var exceptionMessage = "Aggregate of type '{0}' raised an event of type '{1}' but not handler could be found to handle the message."
                .FormatWith(aggregate.GetType().Name, eventMessage.GetType().Name);

            throw new HandlerForDomainEventNotFoundException(exceptionMessage);
        }
    }
}
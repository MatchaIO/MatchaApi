using System;
using System.Collections.Generic;
using System.Linq;
using Matcha.WebApi.Domain.DataAccess;
using Matcha.WebApi.Domain.Models;
using Matcha.WebApi.Messages.Commands;
using Matcha.WebApi.Messages.Events;
using Matcha.WebApi.Messages.Projections;
using Matcha.WebApi.Messages.Queries;

namespace Matcha.WebApi.Handlers
{
    public abstract class Event
    {
        protected Event()
        {
            EventId = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
        }
        public Guid EventId { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public interface IEventPublisher
    {
        void Publish(Event eventToPublish);
    }

    public interface IEventRepository
    {
        IEnumerable<Event> EventsOfType(string eventType);
    }
    public interface ICommandHandler<in TCommand, out TResponse>
        where TCommand : ICommand<TResponse>
    {
        TResponse Handle(TCommand message);
    }

    public interface IQueryHandler<in TCommand, out TResponse>
        where TCommand : IQuery<TResponse>
    {
        TResponse Handle(TCommand message);
    }
    //For internal use only
    public interface IRequest<out TResponse> { }
    public interface ICommand<out TResponse> : IRequest<TResponse> { }
    public interface IQuery<out TResponse> : IRequest<TResponse> { }

    public class LeadHandler :
        ICommandHandler<CreateLeadCommand, Guid>,
        IQueryHandler<GetLeadById, LeadDetail>
    {
        private readonly ILeadRepository _repository;
        private readonly IEventPublisher _eventPublisher;

        public LeadHandler(ILeadRepository repository, IEventPublisher eventPublisher)
        {
            _repository = repository;
            _eventPublisher = eventPublisher;
        }

        public Guid Handle(CreateLeadCommand message)
        {
            var @event = new LeadCreated
            {
                LeadDetail = new LeadDetail
                {
                    Id = Guid.NewGuid(),
                    ContactDetails = message.ContactDetails
                }
            };
            var lead = new Lead(@event);
            _repository.Store(lead);
            _eventPublisher.Publish(@event);
            return lead.Id;
        }

        public LeadDetail Handle(GetLeadById message)
        {
            var lead = _repository.GetLeadById(message.Id);
            return new LeadDetail
            {
                Id = lead.Id,
                ContactDetails = lead.ContactDetails
            };
        }
    }

    public class InMemoryEventPublisher : IEventPublisher, IEventRepository
    {
        private readonly List<Event> _publishedEvents = new List<Event>();

        public List<Event> PublishedEvents
        {
            get { return _publishedEvents; }
        }

        public void Publish(Event eventToPublish)
        {
            _publishedEvents.Add(eventToPublish);
        }

        public IEnumerable<Event> EventsOfType(string eventType)
        {
            return _publishedEvents.Where(e => e.GetType().Name == eventType);
        }
    }
}
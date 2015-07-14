using System;
using System.Collections.Generic;

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

}

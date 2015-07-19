using System;
using System.Collections.Generic;
using Matcha.WebApi.Domain.Events;

namespace Matcha.WebApi.Handlers
{
    

    public interface IEventPublisher
    {
        void Publish(Event eventToPublish);
    }

    public interface IEventRepository
    {
        IEnumerable<Event> EventsOfType(string eventType);
        IEnumerable<Event> GetAll();
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

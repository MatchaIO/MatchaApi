using System;
using Matcha.WebApi.Domain.Events;
using Matcha.WebApi.Handlers;

namespace Matcha.WebApi.Messages.Events
{
    public class LeadDeleted : Event<DeletePayload>
    {
        protected LeadDeleted() { }
        public LeadDeleted(Guid id)
        {
            AggregateId = id;
            Payload = new DeletePayload { AggregateId = id };
        }
    }
}
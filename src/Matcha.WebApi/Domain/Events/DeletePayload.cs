using System;

namespace Matcha.WebApi.Domain.Events
{
    public class DeletePayload
    {
        public Guid AggregateId { get; set; }
    }
}

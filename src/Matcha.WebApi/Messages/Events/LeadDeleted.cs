using System;
using Matcha.WebApi.Handlers;

namespace Matcha.WebApi.Messages.Events
{
    public class LeadDeleted : Event
    {
        public Guid Id { get; set; }
    }
}
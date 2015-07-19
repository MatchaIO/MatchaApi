using System;
using Matcha.WebApi.Handlers;

namespace Matcha.WebApi.Messages.Events
{
    public class LeadVetted : Event
    {
        public Guid Id { get; set; }
        public Guid OpportunityId { get; set; }
    }
}
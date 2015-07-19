using System;
using Matcha.WebApi.Domain.Events;
using Matcha.WebApi.Handlers;

namespace Matcha.WebApi.Messages.Events
{
    public class LeadVetted : Event<LeadVetting>
    {
        
    }

    public class LeadVetting
    {
        public Guid LeadId { get; set; }
        public Guid OpportunityId { get; set; }
    }
}
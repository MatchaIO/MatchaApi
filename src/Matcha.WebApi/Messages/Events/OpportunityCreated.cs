using Matcha.WebApi.Handlers;
using Matcha.WebApi.Messages.Projections;

namespace Matcha.WebApi.Messages.Events
{
    public class OpportunityCreated : Event
    {
        public OpportunityDetail OpportunityDetail { get; set; }
    }
}
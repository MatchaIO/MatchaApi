using Matcha.WebApi.Handlers;
using Matcha.WebApi.Messages.Projections;

namespace Matcha.WebApi.Messages.Events
{
    public class LeadUpdated : Event
    {
        public LeadDetail LeadDetail { get; set; }
    }
}
using Matcha.WebApi.Handlers;
using Matcha.WebApi.Messages.Projections;

namespace Matcha.WebApi.Messages.Events
{
    public class LeadCreated :Event
    {
        public LeadDetail LeadDetail { get; set; }
    }
}

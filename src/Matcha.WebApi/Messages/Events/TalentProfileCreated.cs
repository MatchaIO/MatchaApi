using Matcha.WebApi.Domain.Events;
using Matcha.WebApi.Messages.Dtos;

namespace Matcha.WebApi.Messages.Events
{
    public class TalentProfileCreated : Event
    {
        public TalentProfile TalentProfile { get; set; }
    }
}
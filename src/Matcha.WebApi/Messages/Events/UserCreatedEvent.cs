using Matcha.WebApi.Domain.Events;

namespace Matcha.WebApi.Messages.Events
{
    public class UserCreatedEvent : Event
    {
        public string Username{ get; set; }
        public string Email { get; set; }
    }
}
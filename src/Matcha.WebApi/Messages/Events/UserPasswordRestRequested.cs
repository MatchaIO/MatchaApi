using Matcha.WebApi.Domain.Events;

namespace Matcha.WebApi.Messages.Events
{
    public class UserPasswordRestRequested : Event
    {
        public string Username { get; set; }
    }
}
using System;

namespace Matcha.WebApi.Messages.Dtos
{
    public class UserDetails
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
    }
}
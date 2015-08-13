using System;

namespace Matcha.WebApi.Messages.Dtos
{
    public class TalentProfile
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
    }
}
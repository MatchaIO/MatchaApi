using System;
using Matcha.WebApi.Messages.Dtos;

namespace Matcha.WebApi.Messages.Projections
{
    public class LeadDetail
    {
        public Guid Id { get; set; }
        public ContactDetails ContactDetails { get; set; }
    }
}
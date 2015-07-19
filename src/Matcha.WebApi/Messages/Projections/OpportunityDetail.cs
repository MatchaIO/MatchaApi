using System;
using Matcha.WebApi.Messages.Dtos;

namespace Matcha.WebApi.Messages.Projections
{
    public class OpportunityDetail
    {
        public Guid Id { get; set; }
        public Guid? LeadId { get; set; }
        public ContactDetails ContactDetails { get; set; }
    }
}
using System;
using Matcha.WebApi.Handlers;
using Matcha.WebApi.Messages.Projections;

namespace Matcha.WebApi.Messages.Queries
{
    public class GetOpportunityById : IQuery<OpportunityDetail>
    {
        public GetOpportunityById(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }
}
using System;
using Matcha.WebApi.Handlers;
using Matcha.WebApi.Messages.Projections;

namespace Matcha.WebApi.Messages.Queries
{
    public class GetLeadById : IQuery<LeadDetail>
    {
        public GetLeadById(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }
}

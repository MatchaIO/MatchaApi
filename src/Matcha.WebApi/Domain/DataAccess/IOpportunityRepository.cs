using System;
using Matcha.WebApi.Domain.Models;

namespace Matcha.WebApi.Domain.DataAccess
{
    public interface IOpportunityRepository
    {
        Opportunity GetOpportunityById(Guid id);
        void Store(Opportunity opportunity);
    }
}
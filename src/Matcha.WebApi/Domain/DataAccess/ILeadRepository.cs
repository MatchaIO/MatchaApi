using System;
using System.Collections.Generic;
using Matcha.WebApi.Domain.Models;

namespace Matcha.WebApi.Domain.DataAccess
{
    public interface ILeadRepository
    {
        Lead GetLeadById(Guid id);
        IEnumerable<Lead> GetAllCurrentLeads();
        void Store(Lead lead);
    }
}
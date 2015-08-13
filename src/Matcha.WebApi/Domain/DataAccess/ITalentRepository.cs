using System;
using Matcha.WebApi.Domain.Models;

namespace Matcha.WebApi.Domain.DataAccess
{
    public interface ITalentRepository
    {
        Talent GetById(Guid id);
        void Store(Talent lead);
    }
}
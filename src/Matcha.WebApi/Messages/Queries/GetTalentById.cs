using System;
using Matcha.WebApi.Handlers;
using Matcha.WebApi.Messages.Dtos;

namespace Matcha.WebApi.Messages.Queries
{
    public class GetTalentById : IQuery<TalentProfile>
    {
        public GetTalentById(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }
}
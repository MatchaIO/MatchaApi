using System;
using Matcha.WebApi.Handlers;

namespace Matcha.WebApi.Messages.Commands
{
    public class DeleteLeadCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
    }
}
using System;
using Matcha.WebApi.Handlers;
using Matcha.WebApi.Messages.Dtos;

namespace Matcha.WebApi.Messages.Commands
{
    public class CreateLeadCommand : ICommand<Guid>
    {
        public ContactDetails ContactDetails { get; set; }
        //The info regarding what they want is relavent but not sure what shape it should take yet
    }
}

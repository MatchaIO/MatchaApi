using System;
using FluentValidation;
using FluentValidation.Attributes;
using Matcha.WebApi.Handlers;
using Matcha.WebApi.Messages.Dtos;
using Matcha.WebApi.Messages.Dtos.Validators;

namespace Matcha.WebApi.Messages.Commands
{
    [Validator(typeof(UpdateLeadCommandValidator))]
    public class UpdateLeadCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
        public ContactDetails ContactDetails { get; set; }
        //The info regarding what they want is relavent but not sure what shape it should take yet
    }
    public class UpdateLeadCommandValidator : AbstractValidator<UpdateLeadCommand>
    {
        public UpdateLeadCommandValidator()
        {
            RuleFor(cmd => cmd.ContactDetails).SetValidator(new ContactDetailsValidator());
        }
    }
}
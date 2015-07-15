using System;
using FluentValidation;
using FluentValidation.Attributes;
using Matcha.WebApi.Handlers;
using Matcha.WebApi.Messages.Dtos;

namespace Matcha.WebApi.Messages.Commands
{
    [Validator(typeof(CreateLeadCommandValidator))]
    public class CreateLeadCommand : ICommand<Guid>
    {
        public ContactDetails ContactDetails { get; set; }
        //The info regarding what they want is relavent but not sure what shape it should take yet
    }
    public class CreateLeadCommandValidator : AbstractValidator<CreateLeadCommand>
    {
        public CreateLeadCommandValidator()
        {
            RuleFor(cmd => cmd.ContactDetails).SetValidator(new ContactDetailsValidator());
        }
    }
    public class ContactDetailsValidator : AbstractValidator<ContactDetails>
    {
        public ContactDetailsValidator()
        {
            RuleFor(x => x).Must(HaveAName);
        }

        private bool HaveAName(ContactDetails arg)
        {
            return !string.IsNullOrWhiteSpace(arg.ContactName) || !string.IsNullOrWhiteSpace(arg.OrganiastionName);
        }
    }

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

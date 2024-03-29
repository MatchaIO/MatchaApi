using System;
using FluentValidation;
using FluentValidation.Attributes;
using Matcha.WebApi.Handlers;
using Matcha.WebApi.Messages.Dtos;
using Matcha.WebApi.Messages.Dtos.Validators;
using Matcha.WebApi.Messages.Projections;

namespace Matcha.WebApi.Messages.Commands
{
    [Validator(typeof(UpdateLeadCommandValidator))]
    public class UpdateLeadCommand : ICommand<LeadDetail>
    {
        public Guid Id { get; set; }
        public ContactDetails ContactDetails { get; set; }
        public dynamic OpportunityProposal { get; set; }
    }
    public class UpdateLeadCommandValidator : AbstractValidator<UpdateLeadCommand>
    {
        public UpdateLeadCommandValidator()
        {
            RuleFor(cmd => cmd.ContactDetails).SetValidator(new ContactDetailsValidator());
        }
    }
}
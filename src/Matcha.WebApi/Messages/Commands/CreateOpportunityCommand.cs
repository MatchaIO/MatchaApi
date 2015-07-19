using System;
using FluentValidation;
using FluentValidation.Attributes;
using Matcha.WebApi.Handlers;
using Matcha.WebApi.Messages.Dtos;
using Matcha.WebApi.Messages.Dtos.Validators;
using Matcha.WebApi.Messages.Projections;

namespace Matcha.WebApi.Messages.Commands
{
    [Validator(typeof(CreateOpportunityCommandValidator))]
    public class CreateOpportunityCommand : ICommand<OpportunityDetail>
    {
        public Guid? LeadId { get; set; }
        public ContactDetails ContactDetails { get; set; }
        //The info regarding what they want is relavent but not sure what shape it should take yet
    }
    public class CreateOpportunityCommandValidator : AbstractValidator<CreateOpportunityCommand>
    {
        public CreateOpportunityCommandValidator()
        {
            RuleFor(cmd => cmd.ContactDetails).SetValidator(new ContactDetailsValidator());
        }
    }
}
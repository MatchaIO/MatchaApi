using FluentValidation;
using FluentValidation.Attributes;
using Matcha.WebApi.Handlers;
using Matcha.WebApi.Messages.Dtos;
using Matcha.WebApi.Messages.Dtos.Validators;
using Matcha.WebApi.Messages.Projections;

namespace Matcha.WebApi.Messages.Commands
{
    [Validator(typeof(CreateLeadCommandValidator))]
    public class CreateLeadCommand : ICommand<LeadDetail>
    {
        public ContactDetails ContactDetails { get; set; }
        public dynamic OpportunityProposal { get; set; }
        //The info regarding what they want is relavent but not sure what shape it should take yet
    }
    public class CreateLeadCommandValidator : AbstractValidator<CreateLeadCommand>
    {
        public CreateLeadCommandValidator()
        {
            RuleFor(cmd => cmd.ContactDetails).SetValidator(new ContactDetailsValidator());
        }
    }
}

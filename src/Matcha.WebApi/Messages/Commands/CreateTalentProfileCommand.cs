using FluentValidation;
using FluentValidation.Attributes;
using Matcha.WebApi.Handlers;
using Matcha.WebApi.Messages.Dtos;

namespace Matcha.WebApi.Messages.Commands
{
    [Validator(typeof(CreateTalentProfileCommandValidator))]
    public class CreateTalentProfileCommand : ICommand<TalentProfile>
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
    }
    public class CreateTalentProfileCommandValidator : AbstractValidator<CreateTalentProfileCommand>
    {
        public CreateTalentProfileCommandValidator()
        {
            //RuleFor(cmd => cmd.TalentProfile).SetValidator(new ContactDetailsValidator()); //TODO
        }
    }
}
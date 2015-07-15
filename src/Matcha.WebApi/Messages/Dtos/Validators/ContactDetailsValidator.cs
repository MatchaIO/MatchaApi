using System.Linq;
using FluentValidation;

namespace Matcha.WebApi.Messages.Dtos.Validators
{
    public class ContactDetailsValidator : AbstractValidator<ContactDetails>
    {
        public ContactDetailsValidator()
        {
            RuleFor(x => x).Must(HaveAName);
            RuleFor(x => x).Must(HaveAContact);
        }

        private bool HaveAName(ContactDetails arg)
        {
            return !string.IsNullOrWhiteSpace(arg.ContactName) || !string.IsNullOrWhiteSpace(arg.OrganiastionName);
        }
        private bool HaveAContact(ContactDetails arg)
        {
            if (!arg.Contacts.Any())
            {
                return false;
            }
            if (arg.Contacts.All(c => string.IsNullOrWhiteSpace(c.Details) || string.IsNullOrWhiteSpace(c.Type)))
            {
                return false;
            }
            return true;//Should really validate what they gave us - dictionary look up based on type?
        }
    }
}
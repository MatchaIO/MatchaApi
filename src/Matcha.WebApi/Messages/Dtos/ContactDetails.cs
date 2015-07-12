namespace Matcha.WebApi.Messages.Dtos
{
    public class ContactDetails
    {
        public string OrganiastionName { get; set; }
        public string ContactName { get; set; }
        public Contact[] Contacts { get; set; }
    }
}
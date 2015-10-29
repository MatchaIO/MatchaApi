using System;
using Matcha.WebApi.Messages.Events;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Matcha.WebApi.Domain.Models
{
    public class User
    {
        protected User() { }

        public User(UserCreatedEvent userCreateEvent)
        {
            Id = userCreateEvent.AggregateId.Value;
            //Not usre if i need to be mapping these here - this is possibly best push to the auth domain and we just keep a ref to the id of the user
            Email = userCreateEvent.Email;
            Username = userCreateEvent.Username;
        }

        public virtual Guid Id { get; protected set; }

        public virtual string Username { get; protected set; }
        public virtual string Email { get; protected set; }

        public virtual void Reset(UserPasswordRestRequested userPasswordRest)
        {
            throw new NotImplementedException();
        }
    }

    public class UserMap : ClassMapping<User>
    {
        public UserMap()
        {
            Id(x => x.Id, m => m.Generator(Generators.Assigned));
            Property(x => x.Username, p=>p.NotNullable(true));
            Property(x => x.Email, p=>p.NotNullable(true));
        }
    }
}
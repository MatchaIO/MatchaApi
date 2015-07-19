using System;
using Matcha.WebApi.Messages.Commands;
using Matcha.WebApi.Messages.Dtos;
using Matcha.WebApi.Messages.Events;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Matcha.WebApi.Domain.Models
{
    public class Lead
    {
        public Lead(LeadCreated creationEvent)
        {
            Validate(creationEvent);
            Id = creationEvent.LeadDetail.Id;
            ContactDetails = creationEvent.LeadDetail.ContactDetails;
        }

        protected Lead() { }

        public virtual Guid Id { get; protected set; }
        public virtual bool IsVetted { get; protected set; }
        public virtual Guid? OpportunityId { get; protected set; }
        public virtual bool IsDeleted { get; protected set; }

        /// <summary>
        /// Contact details are required for valid sales lead - at least one means of contact and a name (organisation or person)
        /// </summary>
        public virtual ContactDetails ContactDetails { get; protected set; }

        public virtual void Update(LeadUpdated updateEvent)
        {
            if (Id != updateEvent.LeadDetail.Id)
                throw new ArgumentException("Event is not for this Aggregate", "updateEvent");

            ContactDetails = updateEvent.LeadDetail.ContactDetails;
        }
        public virtual void Update(LeadDeleted updateEvent)
        {
            if (Id != updateEvent.Id)
                throw new ArgumentException("Event is not for this Aggregate", "updateEvent");

            IsDeleted = true;
        }
        public virtual void Update(LeadVetted updateEvent)
        {
            if (Id != updateEvent.Id)
                throw new ArgumentException("Event is not for this Aggregate", "updateEvent");

            IsVetted = true;
            OpportunityId = updateEvent.OpportunityId;
        }
        private static void Validate(LeadCreated message)
        {
            //TODO replace with fluent validation if we are going to do this
            Guard.NotNull(() => message, message);
            Guard.NotDefault(() => message.LeadDetail.Id, message.LeadDetail.Id);
            Guard.NotNull(() => message.LeadDetail.ContactDetails, message.LeadDetail.ContactDetails);
        }
    }

    public class LeadMap : ClassMapping<Lead>
    {
        public LeadMap()
        {
            Id(x => x.Id, m => m.Generator(Generators.Assigned));
            Property(x => x.IsDeleted);
            Property(x => x.IsVetted);
            Property(x => x.OpportunityId);
            Property(x => x.ContactDetails, m =>
            {
                m.NotNullable(true);
                m.Type<JsonSerialisedData<ContactDetails>>();
            });
            //TODO Considering adding a where clause for IsVetted and IsDeleted
        }
    }
}

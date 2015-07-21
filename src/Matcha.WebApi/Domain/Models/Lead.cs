using System;
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
            Id = creationEvent.Payload.Id;
            ContactDetails = creationEvent.Payload.ContactDetails;
            OpportunityProposal = creationEvent.Payload.OpportunityProposal;
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

        /// <summary>
        /// Opportunity details are not required, however if the lead is happy to provide more information we will take it
        /// </summary>
        public virtual dynamic OpportunityProposal { get; protected set; }

        public virtual void Update(LeadUpdated updateEvent)
        {
            if (Id != updateEvent.Payload.Id)
                throw new ArgumentException("Event is not for this Aggregate", "updateEvent");

            ContactDetails = updateEvent.Payload.ContactDetails;
            OpportunityProposal = updateEvent.Payload.OpportunityProposal;
        }
        public virtual void Update(LeadDeleted updateEvent)
        {
            if (Id != updateEvent.AggregateId)
                throw new ArgumentException("Event is not for this Aggregate", "updateEvent");

            IsDeleted = true;
        }
        public virtual void Update(LeadVetted updateEvent)
        {
            if (Id != updateEvent.AggregateId)
                throw new ArgumentException("Event is not for this Aggregate", "updateEvent");

            IsVetted = true;
            OpportunityId = updateEvent.Payload.OpportunityId;
        }
        private static void Validate(LeadCreated message)
        {
            //TODO replace with fluent validation if we are going to do this
            Guard.NotNull(() => message, message);
            Guard.NotDefault(() => message.Payload.Id, message.Payload.Id);
            Guard.NotNull(() => message.Payload.ContactDetails, message.Payload.ContactDetails);
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
            Property(x => x.OpportunityProposal, m =>
            {
                m.NotNullable(false);
                m.Type<JsonSerialisedData<dynamic>>();
            });
            //TODO Considering adding a where clause for IsVetted and IsDeleted
        }
    }
}

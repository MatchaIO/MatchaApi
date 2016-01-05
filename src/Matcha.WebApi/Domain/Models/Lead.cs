using System;
using System.Collections;
using Matcha.WebApi.Domain.DataAccess.EventStoreImpl;
using Matcha.WebApi.Messages.Dtos;
using Matcha.WebApi.Messages.Events;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Matcha.WebApi.Domain.Models
{
    public partial class Lead
    {
        public Lead(LeadCreated creationEvent)
            : this()
        {
            Create(creationEvent);
        }

        //protected Lead() { }//required for NH

        //public virtual Guid Id { get; protected set; }
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

        public virtual void Create(LeadCreated creationEvent)
        {
            Validate(creationEvent);
            Process(creationEvent);
            RaiseEvent(creationEvent);
        }

        private void Process(LeadCreated creationEvent)
        {
            Id = (creationEvent.Payload.Id == Guid.Empty) ? Guid.NewGuid() : creationEvent.Payload.Id;
            ContactDetails = creationEvent.Payload.ContactDetails;
            OpportunityProposal = creationEvent.Payload.OpportunityProposal;
        }

        public virtual void Update(LeadUpdated updateEvent)
        {
            if (Id != updateEvent.Payload.Id)
                throw new ArgumentException("Event is not for this Aggregate", "updateEvent");

            Process(updateEvent);
            RaiseEvent(updateEvent);
        }

        private void Process(LeadUpdated updateEvent)
        {
            ContactDetails = updateEvent.Payload.ContactDetails;
            OpportunityProposal = updateEvent.Payload.OpportunityProposal;
        }

        public virtual void Update(LeadDeleted updateEvent)
        {
            if (Id != updateEvent.AggregateId)
                throw new ArgumentException("Event is not for this Aggregate", "updateEvent");

            Process(updateEvent);
            RaiseEvent(updateEvent);
        }

        private void Process(LeadDeleted updateEvent)
        {
            IsDeleted = true;
        }

        public virtual void Update(LeadVetted updateEvent)
        {
            if (Id != updateEvent.AggregateId)
                throw new ArgumentException("Event is not for this Aggregate", "updateEvent");

            Process(updateEvent);
            RaiseEvent(updateEvent);
        }

        private void Process(LeadVetted updateEvent)
        {
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

    public partial class Lead : AggregateBase
    {
        protected Lead()
        {
            Register<LeadCreated>(Process);
            Register<LeadUpdated>(Process);
            Register<LeadDeleted>(Process);
            Register<LeadVetted>(Process);
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

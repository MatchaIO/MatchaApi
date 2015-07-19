using System;
using Matcha.WebApi.Messages.Dtos;
using Matcha.WebApi.Messages.Events;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Matcha.WebApi.Domain.Models
{
    public class Opportunity
    {
        public Opportunity(OpportunityCreated creationEvent)
        {
            Validate(creationEvent);
            Id = creationEvent.OpportunityDetail.Id;
            LeadId = creationEvent.OpportunityDetail.LeadId;
            ContactDetails = creationEvent.OpportunityDetail.ContactDetails;
        }

        protected Opportunity()
        {
        }

        public virtual Guid Id { get; protected set; }
        public virtual Guid? LeadId { get; protected set; }

        public virtual ContactDetails ContactDetails { get; protected set; }


        private static void Validate(OpportunityCreated message)
        {
            //TODO replace with fluent validation if we are going to do this
            Guard.NotNull(() => message, message);
            Guard.NotNull(() => message.OpportunityDetail, message.OpportunityDetail);
            Guard.NotNull(() => message.OpportunityDetail.ContactDetails, message.OpportunityDetail.ContactDetails);
        }
    }

    public class OpportunityMap : ClassMapping<Opportunity>
    {
        public OpportunityMap()
        {
            Id(x => x.Id, m => m.Generator(Generators.Assigned));
            Property(x => x.LeadId, m => m.NotNullable(false));
            Property(x => x.ContactDetails, m =>
            {
                m.NotNullable(true);
                m.Type<JsonSerialisedData<ContactDetails>>();
            });
            //TODO Considering adding a where clause for IsVetted and IsDeleted
        }
    }
}
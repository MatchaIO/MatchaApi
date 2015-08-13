using System;
using Matcha.WebApi.Messages.Events;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Matcha.WebApi.Domain.Models
{
    public class Talent
    {
        protected Talent(){}

        public Talent(TalentProfileCreated @event)
        {
            Id = @event.TalentProfile.Id;
            Profile = @event.TalentProfile;
        }
        public virtual Guid Id { get; protected set; }
        public virtual dynamic Profile { get; protected set; }
    }
    public class TalentMap : ClassMapping<Talent>
    {
        public TalentMap()
        {
            Id(x => x.Id, m => m.Generator(Generators.Assigned));
            Property(x => x.Profile, m =>
            {
                m.NotNullable(false);
                m.Type<JsonSerialisedData<dynamic>>();
            });
        }
    }
}
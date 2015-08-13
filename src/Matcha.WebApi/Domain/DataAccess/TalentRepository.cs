using System;
using Matcha.WebApi.Domain.Models;
using NHibernate;

namespace Matcha.WebApi.Domain.DataAccess
{
    public class TalentRepository : ITalentRepository
    {
        private readonly ISession _session;
       
        public TalentRepository(ISession session)
        {
            _session = session;
        }

        public Talent GetById(Guid id)
        {
            var talent = _session.Get<Talent>(id);
            if (talent == null)
                throw EntityNotFoundException.From<Talent>(id);
            return talent;
        }

        public void Store(Talent talent)
        {
            _session.SaveOrUpdate(talent);
            _session.Flush();
        }
    }
}
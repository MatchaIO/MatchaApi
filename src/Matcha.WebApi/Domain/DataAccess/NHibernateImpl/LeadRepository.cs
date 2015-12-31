using System;
using System.Collections.Generic;
using System.Linq;
using Matcha.WebApi.Domain.Models;
using NHibernate;
using NHibernate.Linq;

namespace Matcha.WebApi.Domain.DataAccess.NHibernateImpl
{
    public class LeadRepository : ILeadRepository
    {
        private readonly ISession _session;
        private static readonly Func<Lead, bool> IsValid = l => !l.IsDeleted && !l.IsVetted;

        public LeadRepository(ISession session)
        {
            _session = session;
        }

        public Lead GetLeadById(Guid id)
        {
            var lead = _session.Get<Lead>(id);
            if (lead == null || !IsValid(lead))
                throw EntityNotFoundException.From<Lead>(id);
            return lead;
        }

        public IEnumerable<Lead> GetAllCurrentLeads()
        {
            return _session.Query<Lead>().Where(IsValid);
        }

        public void Store(Lead lead)
        {
            _session.SaveOrUpdate(lead);
            _session.Flush();
        }
    }
}

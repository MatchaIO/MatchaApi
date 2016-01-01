using System;
using Matcha.WebApi.Domain.Models;
using NHibernate;

namespace Matcha.WebApi.Domain.DataAccess.NHibernateImpl
{
    public class OpportunityRepository : IOpportunityRepository
    {
        private readonly ISession _session;

        public OpportunityRepository(ISession session)
        {
            _session = session;
        }
        
        public Opportunity GetOpportunityById(Guid id)
        {
            var opportunity = _session.Get<Opportunity>(id);
            if (opportunity == null)
                throw EntityNotFoundException.From<Opportunity>(id);
            return opportunity;
        }
        
        public void Store(Opportunity opportunity)
        {
            _session.SaveOrUpdate(opportunity);
            _session.Flush();
        }
    }
}
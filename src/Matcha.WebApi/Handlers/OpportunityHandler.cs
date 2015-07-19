using System;
using Matcha.WebApi.Domain.DataAccess;
using Matcha.WebApi.Domain.Models;
using Matcha.WebApi.Messages.Commands;
using Matcha.WebApi.Messages.Events;
using Matcha.WebApi.Messages.Projections;
using Matcha.WebApi.Messages.Queries;

namespace Matcha.WebApi.Handlers
{
    public class OpportunityHandler :
        ICommandHandler<CreateOpportunityCommand, OpportunityDetail>,
        IQueryHandler<GetOpportunityById, OpportunityDetail>
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly IEventPublisher _eventPublisher;

        public OpportunityHandler(ILeadRepository leadRepository, IOpportunityRepository opportunityRepository, IEventPublisher eventPublisher)
        {
            _leadRepository = leadRepository;
            _opportunityRepository = opportunityRepository;
            _eventPublisher = eventPublisher;
        }

        public OpportunityDetail Handle(CreateOpportunityCommand message)
        {
            var opportunityCreated = new OpportunityCreated
            {

                OpportunityDetail = new OpportunityDetail
                {
                    Id = Guid.NewGuid(),
                    ContactDetails = message.ContactDetails,
                    LeadId = message.LeadId,
                    //TODO Payload
                }
            };

            var opportunity = new Opportunity(opportunityCreated);
            _opportunityRepository.Store(opportunity);
            _eventPublisher.Publish(opportunityCreated);

            if (message.LeadId.HasValue)
            {
                var vettedEvent = new LeadVetted { Id = message.LeadId.Value, OpportunityId = opportunity.Id };
                var lead = _leadRepository.GetLeadById(vettedEvent.Id);
                lead.Update(vettedEvent);
                _leadRepository.Store(lead);
                _eventPublisher.Publish(vettedEvent);
            }

            return MapToOpportunityDetail(opportunity);
        }

        public OpportunityDetail Handle(GetOpportunityById message)
        {
            var opportunity = _opportunityRepository.GetOpportunityById(message.Id);
            return MapToOpportunityDetail(opportunity);
        }

        private static OpportunityDetail MapToOpportunityDetail(Opportunity opportunity)
        {
            return new OpportunityDetail
            {
                Id = opportunity.Id,
                LeadId = opportunity.LeadId,
                ContactDetails = opportunity.ContactDetails
            };
        }
    }
}
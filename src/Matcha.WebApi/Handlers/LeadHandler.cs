using System;
using Matcha.WebApi.Domain.DataAccess;
using Matcha.WebApi.Domain.Models;
using Matcha.WebApi.Messages.Commands;
using Matcha.WebApi.Messages.Events;
using Matcha.WebApi.Messages.Projections;
using Matcha.WebApi.Messages.Queries;

namespace Matcha.WebApi.Handlers
{
    public class LeadHandler :
        ICommandHandler<CreateLeadCommand, Guid>,
        ICommandHandler<UpdateLeadCommand, Guid>,
        IQueryHandler<GetLeadById, LeadDetail>
    {
        private readonly ILeadRepository _repository;
        private readonly IEventPublisher _eventPublisher;

        public LeadHandler(ILeadRepository repository, IEventPublisher eventPublisher)
        {
            _repository = repository;
            _eventPublisher = eventPublisher;
        }

        public Guid Handle(CreateLeadCommand message)
        {
            var @event = new LeadCreated
            {
                LeadDetail = new LeadDetail
                {
                    Id = Guid.NewGuid(),
                    ContactDetails = message.ContactDetails
                }
            };
            var lead = new Lead(@event);
            _repository.Store(lead);
            _eventPublisher.Publish(@event);
            return lead.Id;
        }

        public Guid Handle(UpdateLeadCommand message)
        {
            var @event = new LeadUpdated
            {
                LeadDetail = new LeadDetail
                {
                    Id = message.Id,
                    ContactDetails = message.ContactDetails
                }
            };
            var lead = _repository.GetLeadById(message.Id);
            lead.Update(@event);
            _repository.Store(lead);
            _eventPublisher.Publish(@event);
            return lead.Id;
        }

        public LeadDetail Handle(GetLeadById message)
        {
            var lead = _repository.GetLeadById(message.Id);
            return new LeadDetail
            {
                Id = lead.Id,
                ContactDetails = lead.ContactDetails
            };
        }
    }
}
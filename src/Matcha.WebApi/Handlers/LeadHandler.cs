using System;
using System.Collections.Generic;
using System.Linq;
using Matcha.WebApi.Domain.DataAccess;
using Matcha.WebApi.Domain.Models;
using Matcha.WebApi.Messages.Commands;
using Matcha.WebApi.Messages.Events;
using Matcha.WebApi.Messages.Projections;
using Matcha.WebApi.Messages.Queries;

namespace Matcha.WebApi.Handlers
{
    public class LeadHandler :
        ICommandHandler<CreateLeadCommand, LeadDetail>,
        ICommandHandler<UpdateLeadCommand, LeadDetail>,
        ICommandHandler<DeleteLeadCommand, Guid>,
        IQueryHandler<GetLeadById, LeadDetail>,
        IQueryHandler<GetLeads, IEnumerable<LeadDetail>>
    {
        private readonly ILeadRepository _repository;
        private readonly IEventPublisher _eventPublisher;

        public LeadHandler(ILeadRepository repository, IEventPublisher eventPublisher)
        {
            _repository = repository;
            _eventPublisher = eventPublisher;
        }

        public LeadDetail Handle(CreateLeadCommand message)
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
            return MapToLeadDetail(lead);
        }

        public LeadDetail Handle(UpdateLeadCommand message)
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
            return MapToLeadDetail(lead);
        }

        public Guid Handle(DeleteLeadCommand message)
        {
            var @event = new LeadDeleted {Id = message.Id};
            var lead = _repository.GetLeadById(message.Id);
            lead.Update(@event);
            _repository.Store(lead);
            _eventPublisher.Publish(@event);
            return message.Id;
        }

        public LeadDetail Handle(GetLeadById message)
        {
            var lead = _repository.GetLeadById(message.Id);
            return MapToLeadDetail(lead);
        }

        private static LeadDetail MapToLeadDetail(Lead lead)
        {
            return new LeadDetail
            {
                Id = lead.Id,
                ContactDetails = lead.ContactDetails
            };
        }

        public IEnumerable<LeadDetail> Handle(GetLeads message)
        {
            return _repository.GetAllCurrentLeads()
                .Select(lead => new LeadDetail
                                    {
                                        Id = lead.Id,
                                        ContactDetails = lead.ContactDetails
                                    });
        }
    }
}
using System;
using Matcha.WebApi.Domain.DataAccess;
using Matcha.WebApi.Domain.Models;
using Matcha.WebApi.Messages.Commands;
using Matcha.WebApi.Messages.Dtos;
using Matcha.WebApi.Messages.Events;
using Matcha.WebApi.Messages.Queries;

namespace Matcha.WebApi.Handlers
{
    public class TalentHandler :
        ICommandHandler<CreateTalentProfileCommand, TalentProfile>,
        IQueryHandler<GetTalentById, TalentProfile>
    {
        private readonly ITalentRepository _repository;
        private readonly IEventPublisher _eventPublisher;

        public TalentHandler(ITalentRepository repository, IEventPublisher eventPublisher)
        {
            _repository = repository;
            _eventPublisher = eventPublisher;
        }

        public TalentProfile Handle(CreateTalentProfileCommand message)
        {
            var aggregateId = Guid.NewGuid();

            var userCreateEvent = new UserCreatedEvent { AggregateId = Guid.NewGuid(), Username = message.Email, Email = message.Email, /*Pasword??,*/ /*roles*/};
            var userPasswordRest = new UserPasswordRestRequested { Username = message.Email };
            var user = new User(userCreateEvent);
            //user.Reset(userPasswordRest);//TODO ???
            var @event = new TalentProfileCreated
            {
                AggregateId = aggregateId,
                TalentProfile = new TalentProfile
                {
                    Id = aggregateId,
                    FirstName = message.FirstName,
                    Surname = message.Surname,
                    Email = message.Email
                },
                UserDetails = new UserDetails
                {
                    UserId = user.Id,
                    Username = user.Username
                }
            };
            var talent = new Talent(@event);
            _repository.Store(talent);
            _eventPublisher.Publish(userCreateEvent);
            _eventPublisher.Publish(userPasswordRest);
            _eventPublisher.Publish(@event);
            return MapToTalentProfile(talent);
        }


        public TalentProfile Handle(GetTalentById message)
        {
            var talent = _repository.GetById(message.Id);
            return MapToTalentProfile(talent);
        }

        private static TalentProfile MapToTalentProfile(Talent talent)
        {
            return new TalentProfile
            {
                Id = talent.Id,
                FirstName = talent.Profile.FirstName,
                Surname = talent.Profile.Surname,
                Email = talent.Profile.Email
            };
        }
    }
}
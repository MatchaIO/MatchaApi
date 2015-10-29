using System;
using System.Net.Http;
using ExpectedObjects;
using Matcha.WebApi.Messages.Commands;
using Matcha.WebApi.Messages.Dtos;
using Matcha.WebApi.Messages.Events;
using Ploeh.AutoFixture;
using Xunit;

namespace Matcha.WebApi.Tests.Features.Talent
{
    public partial class TalentScenarios
    {
        private readonly WebClientProxy _proxy;
        private static readonly Fixture Auto = new Fixture();
        private CreateTalentProfileCommand _createTalentCmd;

        public TalentScenarios()
        {
            _proxy = TestInitialisation.GetWebClientProxy();
        }

        void AnAnonymousUserUsingTheApi()
        {
            _proxy.SetCurrentUser(Users.Anon);
        }

        void TheySubmitTheirProfileDetails()
        {
            _createTalentCmd = Auto.Build<CreateTalentProfileCommand>()
            .With(t => t.Email, Guid.NewGuid().ToString().Replace("-", "") + "@gmail.com")
            .Create();
            _proxy.Post("/api/talent", _createTalentCmd);

        }
        void TheNewIdIsReturned()
        {
            var lastCreatedId = _proxy.GetLastResponseAggregateId(HttpMethod.Post);
            Assert.NotEqual(default(Guid), lastCreatedId);
        }

        void ATalentadminUserCanRetrieveTheTalent()
        {
            _proxy.SetCurrentUser(Users.SalesAdmin);
            var talentProfile = _proxy.Get<TalentProfile>(_proxy.GetLastResponseHeaderLocation(HttpMethod.Post));
            _createTalentCmd
                .ToExpectedObject()
                .ShouldMatch(talentProfile);
        }

        void TalentcreatedEventIsRaised()
        {
            _proxy.SetCurrentUser(Users.EventSubscriber);
            var createdEvent = _proxy.GetLastEventOfType<TalentProfileCreated>();
            _createTalentCmd
                .ToExpectedObject()
                .ShouldMatch(createdEvent.TalentProfile);
        }

        //void AUserIsCreatedWithTheTalentEmailAddress()
        //{
        //    //throw new NotImplementedException(); //Is this even a resource we should be surfacing?
        //}

        //void APasswordResetEventIsRaisedWithTheTalentEmailAddress()
        //{
        //    _proxy.SetCurrentUser(Users.EventSubscriber);
        //    var passwordReset = _proxy.GetLastEventOfType<PasswordReset>();
        //   new 
        //      {
        //       _createTalentCmd.Email
        //    }
        //        .ToExpectedObject()
        //        .ShouldMatch(passwordReset);}
        //}
    }
}
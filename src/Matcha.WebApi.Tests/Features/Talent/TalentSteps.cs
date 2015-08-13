using System;
using System.Net.Http;
using Matcha.WebApi.Messages.Commands;
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
    }
}
using TestStack.BDDfy;
using Xunit;

namespace Matcha.WebApi.Tests.Features.Talent
{
    public partial class TalentScenarios
    {
        [Fact]
        public void TalentSignsUp()
        {
            this.Given(s => s.AnAnonymousUserUsingTheApi())
                .When(s => s.TheySubmitTheirProfileDetails())
                .Then(s => s.TheNewIdIsReturned())
                .BDDfy();
        }
    }
}
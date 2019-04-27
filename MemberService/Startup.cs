using Microsoft.Owin;
using Owin;
using Stripe;

[assembly: OwinStartupAttribute(typeof(MemberService.Startup))]
namespace MemberService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            StripeConfiguration.SetApiKey("sk_test_0Y46TdUoSFmFWs5POAT1Ncq200BR4oniz4");
        }
    }
}

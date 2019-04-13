using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MemberService.Startup))]
namespace MemberService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

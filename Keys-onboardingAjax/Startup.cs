using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Keys_onboardingAjax.Startup))]
namespace Keys_onboardingAjax
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

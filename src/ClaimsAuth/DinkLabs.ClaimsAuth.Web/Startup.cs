using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DinkLabs.ClaimsAuth.Web.Startup))]
namespace DinkLabs.ClaimsAuth.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

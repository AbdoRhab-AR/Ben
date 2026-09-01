using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KOSS.Web.Startup))]

namespace KOSS.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

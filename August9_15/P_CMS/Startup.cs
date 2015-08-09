using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(P_CMS.Startup))]
namespace P_CMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

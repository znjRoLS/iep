using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(iep_newer.Startup))]
namespace iep_newer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

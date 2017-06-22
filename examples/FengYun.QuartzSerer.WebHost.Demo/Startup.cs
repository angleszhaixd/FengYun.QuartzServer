using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FengYun.QuartzSerer.WebHost.Demo.Startup))]
namespace FengYun.QuartzSerer.WebHost.Demo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

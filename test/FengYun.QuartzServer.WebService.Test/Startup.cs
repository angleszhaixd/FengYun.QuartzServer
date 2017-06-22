using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FengYun.QuartzServer.WebService.Test.Startup))]
namespace FengYun.QuartzServer.WebService.Test
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

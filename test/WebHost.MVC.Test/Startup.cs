using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebHost.MVC.Test.Startup))]
namespace WebHost.MVC.Test
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

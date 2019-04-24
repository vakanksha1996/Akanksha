using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Akanksha.Startup))]
namespace Akanksha
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

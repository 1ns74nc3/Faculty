using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Faculty.Startup))]
namespace Faculty
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

    }
}

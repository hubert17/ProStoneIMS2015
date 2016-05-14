using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProStoneIMS2015.Startup))]
namespace ProStoneIMS2015
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

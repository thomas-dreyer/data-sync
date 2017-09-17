using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DataSyncWeb.Startup))]
namespace DataSyncWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

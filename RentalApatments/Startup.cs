using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RentalApatments.Startup))]
namespace RentalApatments
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

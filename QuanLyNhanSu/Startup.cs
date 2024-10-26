using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QuanLyNhanSu.Startup))]
namespace QuanLyNhanSu
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
            GlobalHost.Configuration.MaxIncomingWebSocketMessageSize = null; // For large audio packets

        }
    }
}

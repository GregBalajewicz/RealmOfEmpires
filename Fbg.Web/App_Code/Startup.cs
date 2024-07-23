//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using Microsoft.Owin;
//using Owin;
//using Microsoft.AspNet.SignalR;
//[assembly: OwinStartup(typeof(Startup))]

//public class Startup
//{
//    public void Configuration(IAppBuilder app)
//    {
//        //app.MapSignalR("/signalr", new HubConfiguration { EnableDetailedErrors = true, EnableJavaScriptProxies = true });
//        var hubConfiguration = new HubConfiguration();
//       // hubConfiguration.EnableDetailedErrors = true;
//        app.MapSignalR(hubConfiguration);
//        //app.MapSignalR();
//    }
//}
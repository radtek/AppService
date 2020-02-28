using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Http;
using Microsoft.Owin.Cors;
using System.IO;
using oAuth2Test.provider;

namespace oAuth2Test
{
    public partial class Startup
    {
        //oAuth2Test.App_Start.Startup sdf = new App_Start.Startup();
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            
            //var config = new HttpConfiguration();
            //ConfigureAuth(app);
            //app.UseCors(CorsOptions.AllowAll);
            //app.UseWebApi(config);
        }
    }
}
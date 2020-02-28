using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using oAuth2Test.provider;
using Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace oAuth2Test
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        //public static OAuthBearerAuthenticationOptions OAuthOptions { get; private set; }
        static Startup()
        {
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/login"),
                Provider = new OAuthAppProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(5),
                AllowInsecureHttp = true,
               
            };
           
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            //string ff =OAuthOptions.
            
            app.UseOAuthAuthorizationServer(OAuthOptions);
            
        }
        //public void Configuration(IAppBuilder app)
        //{
        //    var config = new HttpConfiguration();
        //    ConfigureAuth(app);
        //    app.UseCors(CorsOptions.AllowAll);
        //    app.UseWebApi(config);
        //}
    }
}
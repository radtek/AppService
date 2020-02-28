using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Security.Claims;
using oAuth2Test.Service;
using oAuth2Test.Models;
using System.Web.Script.Serialization;
using System.IO;

namespace oAuth2Test.provider
{
    public class OAuthAppProvider: OAuthAuthorizationServerProvider
    {
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var resObj = Task.Factory.StartNew(() =>
            {
                var username = context.UserName;
                var password = context.Password;
                var userService = new UserService();
                User user = userService.GetUserByCredentials(username, password);
                if (user != null)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim("UserID", user.Id)
                    };

                    ClaimsIdentity oAutIdentity = new ClaimsIdentity(claims, Startup.OAuthOptions.AuthenticationType);
                    var TokenResult = new AuthenticationTicket(oAutIdentity, new AuthenticationProperties() { });
                    context.Validated(TokenResult);

                   
                    //body1.
                    //string body1 = context.Response.Body.Length.ToString();
                    //logWriter._error("BODY", body1);
                }
                else
                {
                    context.SetError("invalid_grant", "нууц үг буруу байна.");
                }
            });
            return resObj;
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
            {
                context.Validated();
            }
            var result = Task.FromResult<object>(null);

           
            //string resp = string.Empty;
            //using (StreamReader sr = new StreamReader(HttpContext.Current.Response.OutputStream))
            //{
            //    resp = sr.ReadToEnd();
            //}
            //logWriter._error("SR", resp);

            return result;
        }
    }
}
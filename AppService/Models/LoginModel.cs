using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppService.Models
{
    public class LoginModel
    {
        public virtual string grant_type { get; set; }
        public virtual string username { get; set; }
        public virtual string password { get; set; }
        public virtual string refresh_token { get; set; }
        public virtual string client_id { get; set; }
        public virtual string client_secret { get; set; }
    }
    public class LoginResponse
    {
        public virtual string access_token { get; set; }
        public virtual string token_type { get; set; }
        public virtual int expires_in { get; set; }
        public virtual string refresh_token { get; set; }

    }
    public class AuthError
    {
        public virtual string error { get; set; }
        public virtual string error_description { get; set; }
    }
    public class RefreshToken
    {
        public virtual string grant_type { get; set; }
        public virtual string refresh_token { get; set; }
        public virtual string client_id { get; set; }
        public virtual string client_secret { get; set; }
    }
}
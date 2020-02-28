using oAuth2Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace oAuth2Test.Service
{
    public class UserService
    {
        public User GetUserByCredentials(string email, string password)
        {
            if(email != "batmunkh@ddishtv.mn" || password !="booboo")
            {
                return null;
            }
            User user = new User() { Id = "1", Email = "batmunkh@ddishtv.mn", Name = "Batmunkh", Password = "booboo" };
            if(user != null)
            {
                user.Password = string.Empty;
            }
            return user;
        }
    }
}
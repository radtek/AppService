using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace oAuth2Test.Controllers
{
    //[Authorize]
    public class testAuthController : ApiController
    {
        public IEnumerable<string> Get()
        {
            return new string[] { "Hello REST API", "I am Authorized" };
        }

        // GET api/WebApi/5  
        public string Get(int id)
        {
            string exptime = TimeSpan.FromMinutes(5).TotalSeconds.ToString();
            return "Hello Authorized API with Time = " + exptime;
        }
    }
}

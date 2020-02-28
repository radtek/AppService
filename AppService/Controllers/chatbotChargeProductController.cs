using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AppServiceUtil.AppserviceModel;
using AppServiceUtil.Utils;
using AppServiceUtil.Auth;
using AppServiceUtil.DBControl;
using AppService.Models;
using System.Data;
using System.Web;
using System.Web.Script.Serialization;
using Swashbuckle.Swagger.Annotations;

namespace AppService.Controllers
{
    [RoutePrefix("api/chatbotChargeProduct")]
    public class chatbotChargeProductController : ApiController
    {
        DBConnection dbconn = new DBConnection();
        private string TAG = "chatbotChargeProduct";
        private string dbres = string.Empty;
        JavaScriptSerializer serializer = new JavaScriptSerializer();
    }
}

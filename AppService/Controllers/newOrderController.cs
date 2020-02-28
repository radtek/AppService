using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AppServiceUtil.AppserviceModel;
using AppServiceUtil.Utils;
using AppServiceUtil.DBControl;
using System.Web;
using System.Web.Script.Serialization;
using Swashbuckle.Swagger.Annotations;

namespace AppService.Controllers
{
    [RoutePrefix("api/newOrder")]
    public class newOrderController : ApiController
    {
        private string TAG = "newOrder";
        DBConnection dbconn = new DBConnection();
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private string dbres = string.Empty;


        /// <summary>
        /// Шинэ хэрэглэгчийн болон антенн тохируулах захиалга хүлээн авах
        /// </summary>
        /// <param name="orderType">15377 - Багц төхөөрөмж, 15378 - Антенн тохиргоо (mandatory)</param>
        /// <param name="userName">Захиалга өгөх хэрэглэгчийн нэр (mandatory)</param>
        /// <param name="phoneNo">Захиалга өгөх хэрэглэгчийн утас (mandatory)</param>
        /// <param name="districtCode">Дүүргийн код</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{orderType}/{userName}/{phoneNo}/{districtCode}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<newOrderModel>))]
        public HttpResponseMessage Get(string orderType, string userName, string phoneNo, string districtCode)
        {
            HttpResponseMessage message = null;
            newOrderModel ordermdl = new newOrderModel();
            string request = string.Empty;
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            try
            {
                if(token == "YGHM9SHBC81LMR4G")
                {
                    string response = string.Empty;
                    request = string.Format(@"ORDER TYPE: [{0}], USERNAME:[{1}], PHONE:[{2}], DISTRICTCODE:[{3}]", orderType, userName, phoneNo, districtCode);
                    if (httpUtil.httpCall_GET(phoneNo, userName, districtCode, orderType, out response))
                    {
                        dynamic jItem = serializer.Deserialize<object>(response);
                        var issuccess = jItem["is_success"];
                        var respMessage = jItem["errorMsg"];
                        if (issuccess == "success")
                        {
                            ordermdl.isSuccess = true;
                            ordermdl.resultCode = HttpStatusCode.OK.ToString();
                            string resmessage = string.Empty;
                            switch (orderType)
                            {
                                case "15378":
                                    resmessage = "Антен тохируулах захиалгыг амжилттай хүлээн авлаа. Бид 10:00 – 18:00 цагийн хооронд тантай эргэн холбогдож захиалгын дэлгэрэнгүй мэдээллийг авах болно.";
                                    break;
                                case "15377":
                                    resmessage = "Таны шинэ хэрэглэгчийн захиалгыг амжилттай хүлээн авлаа. Бид 10:00 – 18:00 цагийн хооронд тантай эргэн холбогдож захиалгын дэлгэрэнгүй мэдээллийг авах болно.";
                                    break;
                                default:
                                    resmessage = "Таны шинэ хэрэглэгчийн/антен тохируулах захиалгыг амжилттай хүлээн авлаа. Бид 10:00 – 18:00 цагийн хооронд тантай эргэн холбогдож захиалгын дэлгэрэнгүй мэдээллийг авах болно.";
                                    break;
                            }
                            ordermdl.resultMessage = resmessage;
                        }
                        else
                        {
                            ordermdl.isSuccess = false;
                            ordermdl.resultCode = HttpStatusCode.Unauthorized.ToString();
                            ordermdl.resultMessage = respMessage;
                        }
                    }
                    else
                    {
                        ordermdl.isSuccess = false;
                        ordermdl.resultCode = HttpStatusCode.Unauthorized.ToString();
                        ordermdl.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
                    }
                }
                else
                {
                    ordermdl.isSuccess = false;
                    ordermdl.resultCode = HttpStatusCode.Unauthorized.ToString();
                    ordermdl.resultMessage = appConstantValues.MSG_EXPIRED;
                }
            }
            catch(Exception ex)
            {
                exceptionManager.ManageException(ex, TAG);
                ordermdl.isSuccess = false;
                ordermdl.resultCode = HttpStatusCode.NotFound.ToString();
                ordermdl.resultMessage = ex.Message;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, ordermdl);
            LogWriter._newOrder(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), request, serializer.Serialize(ordermdl), token));
            return message;
        }
    }
}

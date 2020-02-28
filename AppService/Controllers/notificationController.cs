using AppService.Models;
using AppServiceUtil.AppserviceModel;
using AppServiceUtil.Auth;
using AppServiceUtil.DBControl;
using AppServiceUtil.Utils;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace AppService.Controllers
{
    [RoutePrefix("api/notification")]
    public class notificationController : ApiController
    {
        DBConnection dbconn = new DBConnection();
        private string TAG = "notification";
        private string dbres = string.Empty;
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        /// <summary>
        /// Notification мэдээлэл авах service
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<notificationModel>))]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage message = null;
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            notificationModel response = new notificationModel();
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        DataTable dt = dbconn.getTable(appServiceQry.getAppNotification(userCardNo));
                        //if(dt.Rows.Count !=0)
                        //{
                        //    foreach
                        //}
                        //else
                        //{
                        //    response.isSuccess = false;
                        //    response.resultCode = HttpStatusCode.NotFound.ToString();
                        //    response.resultMessage = appConstantValues.MSG_NOFOUND;
                        //}
                        List<notificationDetial> notis = new List<notificationDetial>();
                        foreach(DataRow item in dt.Rows)
                        {
                            notificationDetial dtl = new notificationDetial();
                            dtl.notiName = item["NOTIFICATION_NAME"].ToString();
                            dtl.notiText = item["NOTIFICATION_TEXT"].ToString();
                            dtl.notiImgUrl = item["NOTIFICATION_IMG_URL"].ToString();
                            dtl.notiDate= item["NOTIDATE"].ToString();
                            notis.Add(dtl);
                        }
                        response.isSuccess = true;
                        response.resultCode = HttpStatusCode.OK.ToString();
                        response.resultMessage = "success";
                        response.notifications = notis;
                    }
                    else
                    {
                        response.isSuccess = false;
                        response.resultCode = HttpStatusCode.Unauthorized.ToString();
                        response.resultMessage = appConstantValues.MSG_EXPIRED;
                    }
                }
                else
                {
                    response.isSuccess = false;
                    response.resultCode = HttpStatusCode.NotFound.ToString();
                    response.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
                    LogWriter._error(TAG, dbres);
                }
            }
            catch(Exception ex)
            {
                exceptionManager.ManageException(ex, TAG);
                response.isSuccess = false;
                response.resultCode = HttpStatusCode.NotFound.ToString();
                response.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._noti(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), "", serializer.Serialize(response), token));
            return message;
        }
    }
}

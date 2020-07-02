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
using System.Threading.Tasks;
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
        /// [ХУУЧИН] Notification мэдээлэл авах service
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

        /// <summary>
        /// [ШИНЭ] Notification мэдээлэл авах service
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("new")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<notificationModel>))]
        public async Task<HttpResponseMessage> GetNewServ()
        {
            HttpResponseMessage message = new HttpResponseMessage();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            notificationModel response = new notificationModel();
            LogWriter._noti(TAG, string.Format(@"[>>] Request Token: [{0}]", token));
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        List<ReadStat> readList = dbconn.getTable(appServiceQry.getReadNotification(userCardNo)).AsEnumerable().Select(s => new ReadStat
                        {
                            notifId = s["NOTI_ID"].ToString(),
                            userId = s["READ_USER"].ToString()
                        }).ToList();
                        DataTable dt = dbconn.getTable(appServiceQry.getAppNotification(userCardNo));
                        List<notificationDetial> notis = new List<notificationDetial>();
                        foreach (DataRow item in dt.Rows)
                        {
                            notificationDetial dtl = new notificationDetial();
                            string id = item["NOTI_ID"].ToString();
                            dtl.notiId = id;
                            dtl.notiName = item["NOTIFICATION_NAME"].ToString();
                            dtl.notiText = item["NOTIFICATION_TEXT"].ToString();
                            dtl.notiImgUrl = item["NOTIFICATION_IMG_URL"].ToString();
                            dtl.notiDate = item["NOTIDATE"].ToString();
                            dtl.isRead = isRead(readList, id);
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
            catch (Exception ex)
            {
                exceptionManager.ManageException(ex, TAG);
                response.isSuccess = false;
                response.resultCode = HttpStatusCode.NotFound.ToString();
                response.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._noti(TAG, string.Format("[<<] IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), "", serializer.Serialize(response), token));
            return message;
        }
        private bool isRead(List<ReadStat> isReads, string notiId)
        {
            bool res = false;
            List<ReadStat> rr = isReads.Where(w => w.notifId == notiId).ToList();
            if (rr.Count != 0)
            {
                res = true;
            }
            return res;
        }

        /// <summary>
        /// Сонордуулга мэдээлэл уншигдсан төлөвт оруулах сервис
        /// </summary>
        /// <param name="nofitId">Уншигдсан сонордуулгын ID байна</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{nofitId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModel>))]
        public async Task<HttpResponseMessage> GetRead(string nofitId)
        {
            HttpResponseMessage message = new HttpResponseMessage();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            defaultResponseModel response = new defaultResponseModel();
            LogWriter._noti(TAG, string.Format(@"[>>] Request Token: [{0}], NotifId: [{1}]", token, nofitId));
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        string res = dbconn.iDBCommand(appServiceQry.setReadList(nofitId, userCardNo));
                        if (res.Contains("FFFFx["))
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
                        }
                        else
                        {
                            response.isSuccess = true;
                            response.resultCode = HttpStatusCode.OK.ToString();
                            response.resultMessage = "success";
                        }
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
            catch (Exception ex)
            {
                exceptionManager.ManageException(ex, TAG);
                response.isSuccess = false;
                response.resultCode = HttpStatusCode.NotFound.ToString();
                response.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._noti(TAG, string.Format("[<<] IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), "", serializer.Serialize(response), token));
            return message;
        }

        /// <summary>
        /// Уншигдаагүй сонордуулга мэдээллийн тоог авах сервис
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("unread")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<unreadNotifCount>))]
        public async Task<HttpResponseMessage> GetUnRead()
        {
            HttpResponseMessage message = new HttpResponseMessage();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            unreadNotifCount response = new unreadNotifCount();
            LogWriter._noti(TAG, string.Format(@"[>>] Request Token: [{0}]", token));
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        DataTable dt = dbconn.getTable(appServiceQry.getUnReadedCount(userCardNo));
                        response.isSuccess = true;
                        response.resultCode = HttpStatusCode.OK.ToString();
                        response.resultMessage = "success";
                        response.countN = dt.Rows[0]["COUNTN"].ToString();
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
            catch (Exception ex)
            {
                exceptionManager.ManageException(ex, TAG);
                response.isSuccess = false;
                response.resultCode = HttpStatusCode.NotFound.ToString();
                response.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._noti(TAG, string.Format("[<<] IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), "", serializer.Serialize(response), token));
            return message;
        }

    }
}

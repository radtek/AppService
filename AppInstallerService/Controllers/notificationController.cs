using AppServiceUtil.DBControl;
using AppServiceUtil.TabletModel;
using AppServiceUtil.Utils;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace AppInstallerService.Controllers
{
    [RoutePrefix("v1/Notif")]
    public class notificationController : ApiController
    {
        private string TAG = "notificationController";
        DBControlNew dbconn = new DBControlNew();
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// Нэвтэрсэн суурилуулагч нарт ирсэн сонордуулга авах сервис
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<NotificationResponse>))]
        public async Task<HttpResponseMessage> Get()
        {
            LogWriter._noti(TAG, string.Format(@"[>>] Request: [****]"));
            HttpResponseMessage message = new HttpResponseMessage();
            NotificationResponse response = new NotificationResponse();
            string ip = httpUtil.GetClientIPAddress(HttpContext.Current.Request);
            string secToken = string.Empty;
            try
            {
                secToken = HttpContext.Current.Request.Headers["Authorization"].Replace("Basic ", "").Trim();
                if (dbconn.idbStatOK())
                {
                    string insId = string.Empty;
                    string insPhone = string.Empty;
                    if(dbconn.tabletCheckToken(secToken, out insId, out insPhone))
                    {
                        LogWriter._noti(TAG, string.Format(@"Request Token: [{0}], InstallerId: [{1}]", secToken, insId));
                        DataTable dt = dbconn.getTable(tabletQuery.getNoifList(insId));
                        List<ReadStat> readList = dbconn.getTable(tabletQuery.getReadList(insId)).AsEnumerable().Select(s => new ReadStat
                        {
                            notifId = s["NOTIF_ID"].ToString(),
                            userId = s["READ_USER"].ToString()
                        }).ToList();
                        List<Notif> notiList = new List<Notif>();
                        foreach(DataRow item in dt.Rows)
                        {
                            Notif noti = new Notif();
                            string notiId = item["NOTIF_ID"].ToString();
                            noti.notificationId = int.Parse(notiId);
                            noti.notificationName = item["NOTIF_NAME"].ToString();
                            noti.notificationText = item["NOTIF_TXT"].ToString();
                            noti.cDate = item["CDATE"].ToString();
                            noti.readStatus = isRead(readList, notiId);
                            notiList.Add(noti);
                        }
                        response.isSuccess = true;
                        response.errorCode = Convert.ToString((int)HttpStatusCode.OK);
                        response.resultMessage = "success";
                        response.notifications = notiList;
                    }
                    else
                    {
                        response.isSuccess = false;
                        response.errorCode = Convert.ToString((int)HttpStatusCode.Unauthorized);
                        response.resultMessage = "Session has expired";
                    }
                }
                else
                {
                    response.isSuccess = false;
                    response.errorCode = Convert.ToString((int)HttpStatusCode.InternalServerError);
                    response.resultMessage = "Internal Error";
                }
            }
            catch(Exception ex)
            {
                LogWriter._error(TAG, string.Format(@"Token: [{0}], Exception: [{1}]", secToken, ex.ToString()));
                response.isSuccess = false;
                response.errorCode = Convert.ToString((int)HttpStatusCode.InternalServerError);
                response.resultMessage = ex.Message;
            }
            LogWriter._noti(TAG, string.Format(@"[<<] ClientIp: [{0}], Response: [{1}]", ip, serializer.Serialize(response)));
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            return message;
        }

        /// <summary>
        /// Хэрэглэгчийн Notification -г уншсан төлөвт оруулах болон эсвэл Устгасан төлөвт оруулах сервис
        /// </summary>
        /// <param name="notifId"></param>
        /// <param name="commandType">0 is Readed, 1 is Deleted</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{notifId}/{commandType}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<DefaultResponse>))]
        public async Task<HttpResponseMessage> GetRead(string notifId, string commandType)
        {
            //string ss = string.Format(@"[>>] NotificationId: [{0}], CommandType: [{1}]", notifId, commandType);
            LogWriter._noti(TAG, string.Format(@"[>>] NotificationId: [{0}], CommandType: [{1}]", notifId, commandType));
            DefaultResponse response = new DefaultResponse();
            HttpResponseMessage message = new HttpResponseMessage();
            string ip = httpUtil.GetClientIPAddress(HttpContext.Current.Request);
            string secToken = string.Empty;
            try
            {
                secToken = HttpContext.Current.Request.Headers["Authorization"].Replace("Basic ", "").Trim();
                if (dbconn.idbStatOK())
                {
                    string insId = string.Empty;
                    string insPhone = string.Empty;
                    if (dbconn.tabletCheckToken(secToken, out insId, out insPhone))
                    {
                        if(commandType == "0")
                        {
                            if (dbconn.idbCommand(tabletQuery.setReadList(notifId, insId)))
                            {
                                response.isSuccess = true;
                                response.errorCode = Convert.ToString((int)HttpStatusCode.OK);
                                response.resultMessage = "success";
                            }
                            else
                            {
                                response.isSuccess = false;
                                response.errorCode = Convert.ToString((int)HttpStatusCode.NotFound);
                                response.resultMessage = "unsuccess";
                            }
                        }
                        else
                        {
                            if (dbconn.idbCommand(tabletQuery.setDeletedList(notifId, insId)))
                            {
                                response.isSuccess = true;
                                response.errorCode = Convert.ToString((int)HttpStatusCode.OK);
                                response.resultMessage = "success";
                            }
                            else
                            {
                                response.isSuccess = false;
                                response.errorCode = Convert.ToString((int)HttpStatusCode.NotFound);
                                response.resultMessage = "unsuccess";
                            }
                        }
                    }
                    else
                    {
                        response.isSuccess = false;
                        response.errorCode = Convert.ToString((int)HttpStatusCode.Unauthorized);
                        response.resultMessage = "Session has expired";
                    }
                }
                else
                {
                    response.isSuccess = false;
                    response.errorCode = Convert.ToString((int)HttpStatusCode.InternalServerError);
                    response.resultMessage = "Internal Error";
                }
            }
            catch(Exception ex)
            {
                LogWriter._error(TAG, string.Format(@"NotificationId: [{0}], Exception: [{1}]", notifId, ex.ToString()));
                response.isSuccess = false;
                response.errorCode = Convert.ToString((int)HttpStatusCode.InternalServerError);
                response.resultMessage = "Internal Error";
            }
            LogWriter._noti(TAG, string.Format(@"[<<] ClientIp: [{0}], Response: [{1}]", ip, serializer.Serialize(response)));
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            return message;
        }




        private bool isRead(List<ReadStat> isReads, string notiId)
        {
            bool res = false;
            List<ReadStat> rr = isReads.Where(w => w.notifId == notiId).ToList();
            if(rr.Count != 0)
            {
                res = true;
            }
            return res;
        }
    }
    
}

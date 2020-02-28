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
using AppService.Models;
using Newtonsoft.Json;

namespace AppService.Controllers
{
    public class pushNotificationController : ApiController
    {
        DBConnection dbconn = new DBConnection();
        private string TAG = "pushNotification";
        private string dbres = string.Empty;
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        [HttpPost]
        public HttpResponseMessage Post([FromBody] pushNotiModel injson)
        {
            HttpResponseMessage message = null;
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            defaultResponseModel response = new defaultResponseModel();
            LogWriter._noti(TAG, string.Format(@"[>>] Request: [{0}]", serializer.Serialize(injson)));
            googleAPIRequest googleRequest = new googleAPIRequest();
            Notification googleNotif = new Notification();
            string googleResponse = string.Empty;
            string card = string.Empty;
            try
            {
                if (token == "YGHM9SHBC81LMR4G")
                {
                    if(dbconn.idbCheck(out dbres))
                    {
                        if(injson.to == "public")
                        {
                            googleRequest.to = "/topics/ddish";
                            googleNotif.body = injson.notification.body;
                            googleNotif.title = injson.notification.title;
                            googleRequest.notification = googleNotif;
                            card = string.Empty;
                        }
                        else
                        {
                            googleRequest.to = injson.to;
                            googleNotif.body = injson.notification.body;
                            googleNotif.title = injson.notification.title;
                            googleRequest.notification = googleNotif;
                            card = injson.cardNo;
                        }
                        if (saveData(injson.notification.title, injson.notification.body, injson.expireDate, card))
                        {
                            if (httpUtil.httpCall_POST_google(serializer.Serialize(googleRequest), out googleResponse))
                            {
                                response.isSuccess = true;
                                response.resultCode = HttpStatusCode.OK.ToString();
                                response.resultMessage = googleResponse;
                            }
                            else
                            {
                                response.isSuccess = true;
                                response.resultCode = HttpStatusCode.OK.ToString();
                                response.resultMessage = googleResponse;
                            }
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
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
                else
                {
                    response.isSuccess = false;
                    response.resultCode = HttpStatusCode.Unauthorized.ToString();
                    response.resultMessage = appConstantValues.MSG_EXPIRED;
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
            LogWriter._noti(TAG, string.Format(@"[<<] IP: [{0}], Response: [{1}], Token: [{2}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), serializer.Serialize(response), token));
            return message;
        }
        private bool saveData(string _title, string _body, string _expireDate, string _cardNo)
        {
            bool result = false;
            try
            {
                string dbcommand = dbconn.iDBCommand(appServiceQry.saveNotification(_title, _body, _expireDate, _cardNo));
                if (dbcommand.Contains("FFFFx["))
                {
                    LogWriter._error(TAG, dbcommand);
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            catch(Exception ex)
            {
                exceptionManager.ManageException(ex, TAG);
                result = false;
            }
            return result;
        }
    }
}

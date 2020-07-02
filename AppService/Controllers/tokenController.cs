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

namespace AppService.Controllers
{
    public class tokenController : ApiController
    {
        DBConnection dbconn = new DBConnection();
        private string TAG = "RefreshToken";
        private string dbres = string.Empty;
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        [HttpPost]
        public HttpResponseMessage Post([FromBody] RefreshToken injson)
        {
            HttpResponseMessage message = null;
            LoginResponse loginResp = new LoginResponse();
            AuthError authError = new AuthError();
            string cip = httpUtil.GetClientIPAddress(HttpContext.Current.Request);
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            try
            {
                if (token == "YGHM9SHBC81LMR4G")
                {
                    if (dbconn.idbCheck(out dbres))
                    {
                        DataTable dt = dbconn.getTable(appServiceQry._checkRToken(injson.refresh_token));
                        if(dt.Rows.Count != 0)
                        {
                            string cardno = dt.Rows[0]["CARD_NO"].ToString();
                            string adminnumber = dt.Rows[0]["PHONE_NO"].ToString();
                            int expSeconds = Convert.ToInt32(TimeSpan.FromMinutes(2).TotalSeconds.ToString());
                            string authExpireDate = DateTime.Now.AddMinutes(2).ToString(appConstantValues.DATE_TIME_FORMAT);
                            string authToken = tokenGenerator.generateToken(cardno);
                            string refreshToken = tokenGenerator.generateToken(cardno);
                            //string commandResult = dbconn.iDBCommand(appServiceQry._registerToken(cardno, authToken, httpUtil.GetClientIPAddress(HttpContext.Current.Request), authExpireDate));
                            if (dbconn.registerToken(cardno, adminnumber, authToken, refreshToken, cip, authExpireDate))
                            {
                                loginResp.access_token = authToken;
                                loginResp.refresh_token = refreshToken;
                                loginResp.expires_in = expSeconds;
                                loginResp.token_type = "bearer";
                                message = Request.CreateResponse(HttpStatusCode.OK, loginResp);
                            }
                            else
                            {
                                authError.error = "invalid_request";
                                authError.error_description = "[DB] Хэрэглэгчийн нэвтрэх нэр эсвэл нууц үг буруу байна.";
                                message = Request.CreateResponse(HttpStatusCode.Unauthorized, authError);
                            }
                        }
                        else
                        {
                            authError.error = "invalid_request";
                            authError.error_description = appConstantValues.MSG_LOGIN_FAILED;
                            message = Request.CreateResponse(HttpStatusCode.Unauthorized, authError);
                        }
                    }
                    else
                    {
                        authError.error = "invalid_request";
                        authError.error_description = appConstantValues.MSG_INTERNAL_ERROR;
                        LogWriter._error(TAG, dbres);
                        message = Request.CreateResponse(HttpStatusCode.Unauthorized, authError);
                    }
                }
                else
                {
                    authError.error = "invalid_request";
                    authError.error_description = appConstantValues.MSG_LOGIN_FAILED;
                    message = Request.CreateResponse(HttpStatusCode.Unauthorized, authError);
                }
            }
            catch(Exception ex)
            {
                authError.error = "invalid_request";
                authError.error_description = ex.Message;
                exceptionManager.ManageException(ex, TAG);
                message = Request.CreateResponse(HttpStatusCode.Unauthorized, authError);
            }
            LogWriter._login(TAG, string.Format(@"IP: [{0}], REQUEST: [{1}], SUCCESS RESP: [{2}], FAILED RESP: [{3}]", cip, serializer.Serialize(injson), serializer.Serialize(loginResp), serializer.Serialize(authError)));
            return message;
        }
    }
}

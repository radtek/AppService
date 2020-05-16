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
using System.Text;

namespace AppService.Controllers
{
    [RoutePrefix("api/login")]
    public class loginController : ApiController
    {
        DBConnection dbconn = new DBConnection();
        private string TAG = "LoginController";
        private string dbres = string.Empty;
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private int expiryMinut = 15;

        [HttpPost]
        public HttpResponseMessage Post([FromBody] LoginModel injson)
        {
            //foreach(var header in HttpContext.Current.Request.Headers)
            //{
            //    LogWriter._login(TAG, string.Format("HEADERNAME: [{0}], HEADERVALUE: [{1}]", header.ToString(), HttpContext.Current.Request.Headers[header.ToString()]));
            //}
            string secToken = HttpContext.Current.Request.Headers["Authorization"].Replace("Basic ", "").Trim();
            HttpResponseMessage message = null;
            LoginResponse loginResp = new LoginResponse();
            AuthError authError = new AuthError();
            string cip = httpUtil.GetClientIPAddress(HttpContext.Current.Request);
            try
            {
                string decodedStr = decodeBase64(secToken);
                string[] clientSS = decodedStr.Split(':');
                string clientId = clientSS[0];
                string clientSecret = clientSS[1];
                if (clientId == "A54FQFR46BD3U2C51D9PZ0QY8AXXC482")
                {
                    if (dbconn.idbCheck(out dbres))
                    {
                        if (injson.grant_type == "password")
                        {
                            DataTable dt = new DataTable();
                            switch (injson.username.Length)
                            {
                                case 8:
                                    dt = dbconn.getTable(appServiceQry._getUserInfoByAdminNo(injson.username));
                                    break;
                                case 10:
                                    dt = dbconn.getTable(appServiceQry._getUserInfoByCardNo(injson.username));
                                    break;
                                case 16:
                                    dt = dbconn.getTable(appServiceQry._getUserInfoByCardNo(injson.username));
                                    break;
                                case 12:
                                    dt = dbconn.getTable(appServiceQry._getUserInfoByCardNo(injson.username.Substring(0, 10)));
                                    break;
                                default:
                                    dt = null;
                                    break;
                            }
                            if (dt.Rows.Count != 0)
                            {
                                string cardno = dt.Rows[0]["CARD_NO"].ToString();
                                string adminnumber = dt.Rows[0]["PHONE_NO"].ToString();
                                string passCode = dt.Rows[0]["SUBSCRIBER_PASS"].ToString();
                                if (passCode == injson.password)
                                {
                                    int expSeconds = Convert.ToInt32(TimeSpan.FromMinutes(expiryMinut).TotalSeconds.ToString());
                                    string authExpireDate = DateTime.Now.AddMinutes(expiryMinut).ToString(appConstantValues.DATE_TIME_FORMAT);
                                    string authToken = tokenGenerator.tokenGen20(cardno);
                                    string refreshToken = tokenGenerator.tokenGen20(cardno);
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
                                authError.error_description = appConstantValues.MSG_LOGIN_FAILED;
                                message = Request.CreateResponse(HttpStatusCode.Unauthorized, authError);
                            }
                        }
                        else
                        {
                            DataTable dt = dbconn.getTable(appServiceQry._checkRToken(injson.refresh_token));
                            if (dt.Rows.Count != 0)
                            {
                                string cardno = dt.Rows[0]["CARD_NO"].ToString();
                                string adminnumber = dt.Rows[0]["PHONE_NO"].ToString();
                                int expSeconds = Convert.ToInt32(TimeSpan.FromMinutes(expiryMinut).TotalSeconds.ToString());
                                string authExpireDate = DateTime.Now.AddMinutes(expiryMinut).ToString(appConstantValues.DATE_TIME_FORMAT);
                                string accessToken = string.Empty;
                                if (dbconn.refreshToken(injson.refresh_token, cardno, authExpireDate, out accessToken))
                                {
                                    loginResp.access_token = accessToken;
                                    loginResp.refresh_token = injson.refresh_token;
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
                                authError.error_description = appConstantValues.MSG_NOT_ACCEPTED;
                                message = Request.CreateResponse(HttpStatusCode.Unauthorized, authError);
                            }
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
            catch (Exception ex)
            {
                authError.error = "invalid_request";
                authError.error_description = ex.Message;
                exceptionManager.ManageException(ex, TAG);
                message = Request.CreateResponse(HttpStatusCode.Unauthorized, authError);
            }
            LogWriter._login(TAG, string.Format(@"IP: [{0}], REQUEST: [{1}], SUCCESS RESP: [{2}], FAILED RESP: [{3}], CLIENT TOKEN: [{4}]", cip, serializer.Serialize(injson), serializer.Serialize(loginResp), serializer.Serialize(authError), secToken));
            return message;
        }

        private string encodeBase64(string orginalTxt)
        {
            byte[] byt = System.Text.Encoding.UTF8.GetBytes(orginalTxt);
            return Convert.ToBase64String(byt);
        }
        private string decodeBase64(string base64Txt)
        {
            byte[] b = Convert.FromBase64String(base64Txt);
            return Encoding.UTF8.GetString(b);
        }
    }
}
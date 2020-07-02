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
using System.Threading.Tasks;
using Swashbuckle.Swagger.Annotations;

namespace AppService.Controllers
{

    [RoutePrefix("api/fbLogin")]
    public class fbLoginController : ApiController
    {
        DBConnection dbconn = new DBConnection();
        private string TAG = "LoginController";
        private string dbres = string.Empty;
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private int expiryMinut = 15;


        /// <summary>
        /// Facebook хаягаар нэвтрэх
        /// </summary>
        /// <param name="fbId">FacebookId (mandatory)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{fbId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<LoginResponse>))]
        public async Task<HttpResponseMessage> GetFacebookLogin(string fbId)
        {
            LogWriter._login(TAG, string.Format("[>>] REQUEST: [{0}]", fbId));
            string secToken = HttpContext.Current.Request.Headers["Authorization"].Replace("Basic ", "").Trim();
            HttpResponseMessage message = new HttpResponseMessage();
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
                        DataTable dt = new DataTable();
                        dt = dbconn.getTable(appServiceQry._getFBUser(fbId));
                        if (dt.Rows.Count != 0)
                        {
                            string cardno = dt.Rows[0]["CARD_NO"].ToString();
                            string adminnumber = dt.Rows[0]["ADMIN_NO"].ToString();

                            int expSeconds = Convert.ToInt32(TimeSpan.FromMinutes(expiryMinut).TotalSeconds.ToString());
                            string authExpireDate = DateTime.Now.AddMinutes(expiryMinut).ToString(appConstantValues.DATE_TIME_FORMAT);
                            string authToken = tokenGenerator.generateToken(cardno);
                            string refreshToken = tokenGenerator.generateToken(cardno);
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
            catch (Exception ex)
            {
                authError.error = "invalid_request";
                authError.error_description = ex.Message;
                exceptionManager.ManageException(ex, TAG);
                message = Request.CreateResponse(HttpStatusCode.Unauthorized, authError);
            }
            LogWriter._login(TAG, string.Format(@"[<<] IP: [{0}], SUCCESS RESP: [{1}], FAILED RESP: [{2}], CLIENT TOKEN: [{3}]", cip, serializer.Serialize(loginResp), serializer.Serialize(authError), secToken));
            return message;
        }

        /// <summary>
        /// Facebook хэрэглэгч бүртгэх
        /// </summary>
        /// <param name="cardOrAdmin">Admin or CardNo (mandatory)</param>
        /// <param name="fbId">FacebookId (mandatory)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{cardOrAdmin}/{fbId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModel>))]
        public async Task<HttpResponseMessage> GetRegisterFbUser(string cardOrAdmin, string fbId)
        {
            LogWriter._login(TAG, string.Format(@"[>>] REQUEST: [CARD:{0}, FBID:{1}]", cardOrAdmin, fbId));
            HttpResponseMessage message = new HttpResponseMessage();
            defaultResponseModel response = new defaultResponseModel();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        string res = dbconn.iDBCommand(appServiceQry._registerFBUser(userCardNo, userAdminNo, fbId));
                        if (res.Contains("FFFFx["))
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = "Хэрэглэгч бүртгэж чадсангүй.";
                        }
                        else
                        {
                            response.isSuccess = true;
                            response.resultCode = HttpStatusCode.OK.ToString();
                            response.resultMessage = appConstantValues.MSG_SUCCESS;
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
            LogWriter._login(TAG, string.Format(@"[<<] RESPONSE: [{0}]", serializer.Serialize(response)));
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

using AppServiceUtil.DBControl;
using AppServiceUtil.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using Swashbuckle.Swagger.Annotations;
using System.Data;
using System.Runtime.CompilerServices;
using AppServiceUtil.TabletModel;

namespace AppInstallerService.Controllers
{
    [RoutePrefix("v1/Login")]
    public class loginController : ApiController
    {
        private string TAG = "Login";
        DBControlNew dbconn = new DBControlNew();
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// Системд нэвтрэх хүсэлт авах.
        /// </summary>
        /// <param name="credentials">Хэрэглэгчийн код Base64</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{credentials}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<LoginResponse>))]
        public async Task<HttpResponseMessage> Get(string credentials)
        {
            LogWriter._login(TAG, string.Format(@"[>>] Request: [{0}]", credentials));
            HttpResponseMessage message = new HttpResponseMessage();
            string ip = httpUtil.GetClientIPAddress(HttpContext.Current.Request);
            string secToken = string.Empty;
            LoginResponse response = new LoginResponse();
            try
            {
                
                secToken = HttpContext.Current.Request.Headers["Authorization"].Replace("Basic ", "").Trim();
                string[] clienCredent = convertors.decodeBase64(secToken).Split(':');
                if (clienCredent[1] == "A54FQFR46BD3U2C51D9PZ0QY8AXXC482")
                {
                    string[] secret = convertors.decodeBase64(credentials).Split(':');
                    string loginName = secret[0];
                    string password = secret[1];
                    if (dbconn.idbStatOK())
                    {
                        DataTable dt = dbconn.getTable(tabletQuery.getLogin(loginName, password));
                        if(dt.Rows.Count != 0)
                        {
                            string _phone = dt.Rows[0]["USER_PHONE"].ToString();
                            string _userId = dt.Rows[0]["USERID"].ToString();
                            string _token = convertors.generateToken(_phone);
                            string _expDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
                            if(dbconn.registerToken(_userId, _phone, _token, _expDate))
                            {
                                response.isSuccess = true;
                                response.errorCode = Convert.ToString((int)HttpStatusCode.OK);
                                response.resultMessage = "success";
                                response.accessToken = _token;
                            }
                            else
                            {
                                response.isSuccess = false;
                                response.errorCode = Convert.ToString((int)HttpStatusCode.InternalServerError);
                                response.resultMessage = "Internal Error";
                            }
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.errorCode = Convert.ToString((int)HttpStatusCode.BadRequest);
                            response.resultMessage = "Хэрэглэгчийн нэвтрэх нэр эсвэл нууц үг буруу байна.";
                        }
                    }
                    else
                    {
                        response.isSuccess = false;
                        response.errorCode = Convert.ToString((int)HttpStatusCode.InternalServerError);
                        response.resultMessage = "Internal Error";
                    }
                    
                }
                else
                {
                    response.isSuccess = false;
                    response.errorCode = Convert.ToString((int)HttpStatusCode.Unauthorized);
                    response.resultMessage = "Client identifier is not match";
                }
                
            }
            catch(Exception ex)
            {
                response.isSuccess = false;
                response.errorCode = Convert.ToString((int)HttpStatusCode.BadRequest);
                response.resultMessage = "Invalid request";
                LogWriter._error(TAG, string.Format(@"ClientIp: [{0}], BasicToken: [{1}], Execption: [{2}]", ip, secToken, ex.ToString()));
            }
            LogWriter._login(TAG, string.Format(@"[<<] ClientIp: [{0}], Response: [{1}]", ip,  serializer.Serialize(response)));
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            return message;
        }
    }
}

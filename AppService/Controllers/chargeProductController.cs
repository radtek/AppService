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
using System.Threading.Tasks;

namespace AppService.Controllers
{
    [RoutePrefix("api/chargeProduct")]
    public class chargeProductController : ApiController
    {
        DBConnection dbconn = new DBConnection();
        private string TAG = "chargeProduct";
        private string dbres = string.Empty;
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// Багц сунгах service
        /// </summary>
        /// <param name="productId">Сунгах багцын Id (mandatory)</param>
        /// <param name="month">Сунгах сар (mandatory)</param>
        /// <remarks></remarks>
        /// <returns></returns>
        [HttpGet]
        [Route("{productId}/{month}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModel>))]
        public async Task<HttpResponseMessage> Get(string productId, string month)
        {
            HttpResponseMessage message = null;
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
                        string resMon = string.Empty;
                        string resEng = string.Empty;
                        string resCry = string.Empty;
                        if (checkCustomType(userCardNo))
                        {
                            if (dbconn.chargeProductByCounter(userCardNo, productId, month, userAdminNo, out resEng, out resMon, out resCry))
                            {
                                response.isSuccess = true;
                                response.resultCode = HttpStatusCode.OK.ToString();
                                response.resultMessage = resMon;
                            }
                            else
                            {
                                response.isSuccess = false;
                                response.resultCode = HttpStatusCode.NotFound.ToString();
                                response.resultMessage = resMon;
                            }
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = "Дараа төлбөрт хэрэглэгч багц сунгах боломжгүй.";
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
            catch(Exception ex)
            {
                response.isSuccess = false;
                response.resultCode = HttpStatusCode.NotFound.ToString();
                response.resultMessage = ex.Message;
                exceptionManager.ManageException(ex, TAG);
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            string req = string.Format(@"PRODUCT: [{0}], MONTH: [{1}]", productId, month);
            LogWriter._chargeProd(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), req, serializer.Serialize(response), token));
            return message;
        }

        /// <summary>
        /// Кино сан түрээслэх service
        /// </summary>
        /// <param name="productId">VOD cувгийн productId (mandatory)</param>
        /// <param name="smsCode">VOD cуваг захиалах smsCode (mandatory)</param>
        /// <param name="inDate">Тухайн контент гарах өдөр (mandatory)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{productId}/{smsCode}/{inDate}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModel>))]
        public async Task<HttpResponseMessage> Get(string productId, string smsCode, string inDate)
        {
            HttpResponseMessage message = null;
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
                        string resMon = string.Empty;
                        string resEng = string.Empty;
                        string resCry = string.Empty;
                        if (checkCustomType(userCardNo))
                        {
                            if (dbconn.addNvodByCounter(userCardNo, userAdminNo, inDate, smsCode, productId, out resEng, out resMon, out resCry))
                            {
                                response.isSuccess = true;
                                response.resultCode = HttpStatusCode.OK.ToString();
                                response.resultMessage = resMon;
                            }
                            else
                            {
                                response.isSuccess = false;
                                response.resultCode = HttpStatusCode.NotFound.ToString();
                                response.resultMessage = resMon;
                            }
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = "Дараа төлбөрт хэрэглэгч захиалга хийх боломжгүй.";
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
                response.isSuccess = false;
                response.resultCode = HttpStatusCode.NotFound.ToString();
                response.resultMessage = ex.Message;
                exceptionManager.ManageException(ex, TAG);

            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            string req = string.Format(@"PRODUCT: [{0}], SMSCODE: [{1}], INDATE: [{2}]", productId, smsCode, inDate);
            LogWriter._addNvod(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), req, serializer.Serialize(response), token));
            return message;
        }

        private bool checkCustomType(string cardNo)
        {
            bool res = false;
            try
            {
                DataTable dt = dbconn.getTable(appServiceQry.checkCard(cardNo));
                if(dt.Rows.Count != 0)
                {
                    string type = dt.Rows[0]["IS_PREPAID"].ToString();
                    if(type == "1")
                    {
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionManager.ManageException(ex, TAG);
            }
            return res;
        }
    }
}

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
        /// Виртуал данснаас Багц сунгах service
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
                            if(productId != "86")
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
                                DataTable dtZ = dbconn.getTable(appServiceQry.checkZuslanProduct(userCardNo));
                                if(dtZ.Rows.Count != 0)
                                {
                                    response.isSuccess = false;
                                    response.resultCode = HttpStatusCode.NotFound.ToString();
                                    response.resultMessage = "Та Зуслан багцыг идэвхжүүлсэн байна.";
                                }
                                else
                                {
                                    if (dbconn.chargeProductByCounter(userCardNo, productId, "3", userAdminNo, out resEng, out resMon, out resCry))
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
        /// Мерчантаар багц сунгах service
        /// </summary>
        /// <param name="productId">Сунгах багцын Id (mandatory)</param>
        /// <param name="month">Сунгах сар (mandatory)</param>
        /// <param name="amount">Мөнгөн дүн (mandatory)</param>
        /// <param name="bankName">Банкны нэр (mandatory)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{productId}/{month}/{amount}/{bankName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModelWidthVat>))]
        public async Task<HttpResponseMessage> Get(string productId, string month, string amount, string bankName)
        {
            HttpResponseMessage message = null;
            defaultResponseModelWidthVat response = new defaultResponseModelWidthVat();
            _eBarimtRequest ebarimt = new _eBarimtRequest();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            string req = string.Format(@"PRODUCT: [{0}], MONTH: [{1}], AMOUNT: [{2}], BANKNAME: [{3}]", productId, month, amount, bankName);
            LogWriter._chargeProd(TAG, string.Format(@"[>>] Request: ({0}), ", req));
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;

                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        
                        if (checkCustomType(userCardNo))
                        {
                            ebarimt.cardNo = userCardNo;
                            ebarimt.channelNo = "6";
                            ebarimt.customerEmail = string.Empty;
                            ebarimt.sendEmail = false;
                            ebarimt.employeeCode = userAdminNo;
                            ebarimt.organization = false;
                            ebarimt.customerNo = string.Empty;
                            var detials = new List<_transactionDetial>();
                            var stock = new _transactionDetial();
                            stock.barCode = "8463100";
                            stock.price = amount;
                            stock.productId = productId;
                            stock.productName = "Үйлчилгээ идэвхжүүлэх";
                            stock.unit = "сар";
                            stock.qty = month;
                            detials.Add(stock);
                            ebarimt.transaction = detials;
                            string desc = string.Format(@"[Charge Product] Mobile App emerchant {0}", bankName);
                            if(dbconn.chargeProduct(productId, month, userAdminNo, amount, desc, userCardNo))
                            {
                                
                                response.isSuccess = true;
                                response.resultCode = HttpStatusCode.OK.ToString();
                                //response.resultMessage = "success";
                                int sttCode = 0;
                                string resp = string.Empty;
                                if (httpWorker.http_POST("http://192.168.10.182:5050/vat/getEBarimt", serializer.Serialize(ebarimt), out sttCode, out resp))
                                {
                                    _eBarimtResponse mta = serializer.Deserialize<_eBarimtResponse>(resp);
                                    if (mta.isSuccess)
                                    {
                                        response.mtaResult = new MTAResult { merchantId = mta.merchantId, amount = mta.amount, billId = mta.billId, date = mta.resultDate, loterryNo = mta.lotteryNo, qrData = mta.qrData, tax = mta.cityTax, vat = mta.vat };
                                        response.resultMessage = "success";
                                    }
                                    else
                                    {
                                        response.resultMessage = "Ebarimt гаргахад алдаа гарлаа. Лавлах: 77771434, 1434";
                                    }
                                    
                                }
                                else
                                {
                                    response.resultMessage = "Ebarimt гаргахад алдаа гарлаа. Лавлах: 77771434, 1434";
                                }
                            }
                            else
                            {
                                response.isSuccess = false;
                                response.resultCode = HttpStatusCode.Conflict.ToString();
                                response.resultMessage = "Багц сунгахад алдаа гарлаа";
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
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.resultCode = HttpStatusCode.NotFound.ToString();
                response.resultMessage = ex.Message;
                exceptionManager.ManageException(ex, TAG);
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._chargeProd(TAG, string.Format("[<<] IP: [{0}], Response: [{1}], Token: [{2}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), serializer.Serialize(response), token));
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

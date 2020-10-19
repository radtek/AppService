using AppServiceUtil.AppserviceModel;
using AppServiceUtil.DBControl;
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

namespace AppService.Controllers
{
    [RoutePrefix("api/accountCharge")]
    public class accountChargeController : ApiController
    {
        DBConnection dbconn = new DBConnection();
        private string TAG = "accountCharge";
        private string dbres = string.Empty;
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// Данс цэнэглэх сервис зөвхөн өөрийн данс цэнэглэхэд ашиглана.
        /// </summary>
        /// <param name="amount">Мөнгөн дүн (mandatory)</param>
        /// <param name="bankName">Банкны нэр (mandatory)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{amount}/{bankName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModelWidthVat>))]
        public async Task<HttpResponseMessage> Get(string amount, string bankName)
        {
            HttpResponseMessage message = null;
            defaultResponseModelWidthVat response = new defaultResponseModelWidthVat();
            _eBarimtRequest ebarimt = new _eBarimtRequest();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            string req = string.Format(@"AMOUNT: [{0}], BANKNAME: [{1}]", amount, bankName);
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
                            stock.productId = "8";
                            stock.productName = "Данс цэнэглэх үйлчилгээ";
                            stock.unit = "ш";
                            stock.qty = "1";
                            detials.Add(stock);
                            ebarimt.transaction = detials;
                            string desc = string.Format(@"[Charge Account] Mobile App emerchant {0}", bankName);
                            if(dbconn.chargeAccount(userCardNo, amount, userAdminNo, desc))
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
                                response.resultMessage = "Данс цэнэглэхэд алдаа гарлаа";
                            }
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = "Дараа төлбөрт хэрэглэгч Данс цэнэглэх боломжгүй.";
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
        /// Бусдын данс цэнэглэхэд Хэрэглэгч шалгах сервис Хэрэглэгчийн овог нууцлагдсан байдлаар илгээгдэнэ.
        /// </summary>
        /// <param name="searchValue">Admin or CardNo (mandatory)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{searchValue}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<othersAccountChargeMdl>))]
        public async Task<HttpResponseMessage> Get(string searchValue)
        {
            HttpResponseMessage message = null;
            othersAccountChargeMdl response = new othersAccountChargeMdl();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            string req = string.Format(@"SEARCH VALUE: [{0}]", searchValue);
            LogWriter._chargeProd(TAG, string.Format(@"[>>] Request: ({0}), ", req));
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    
                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        DataTable dt = dtS(searchValue);
                        if(dt.Rows.Count != 0)
                        {
                            string isprepaid = dt.Rows[0]["IS_PREPAID"].ToString();
                            string fName = dt.Rows[0]["SUBSCRIBER_FNAME"].ToString();
                            string lName = dt.Rows[0]["SUBSCRIBER_LNAME"].ToString();
                            string card = dt.Rows[0]["CARD_NO"].ToString();
                            string admin = dt.Rows[0]["PHONE_NO"].ToString();
                            if (isprepaid != "2")
                            {
                                
                                response.isSuccess = true;
                                response.resultCode = HttpStatusCode.OK.ToString();
                                response.firstName = fName;
                                response.lastName = convertors.replaceName(lName);
                                response.cardNo = card;
                                response.adminNo = admin;
                                response.resultMessage = "success";
                            }
                            else
                            {
                                response.isSuccess = false;
                                response.resultCode = HttpStatusCode.OK.ToString();
                                response.firstName = fName;
                                response.lastName = convertors.replaceName(lName);
                                response.cardNo = card;
                                response.adminNo = admin;
                                response.resultMessage = "Дараа төлбөрт хэрэглэгч тул ашиглах боломжгүй.";

                            }
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = "Хайлтын үр дүн хоосон байна.";
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
            LogWriter._chargeProd(TAG, string.Format("[<<] IP: [{0}], Response: [{1}], Token: [{2}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), serializer.Serialize(response), token));
            return message;
        }

        /// <summary>
        /// [Unlogin] Бусдын данс цэнэглэхэд Хэрэглэгч шалгах сервис Хэрэглэгчийн овог нууцлагдсан байдлаар илгээгдэнэ.
        /// </summary>
        /// <param name="searchValue">Admin or CardNo (mandatory)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("unlogin/{searchValue}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<othersAccountChargeMdl>))]
        public async Task<HttpResponseMessage> GetUnLogin(string searchValue)
        {
            HttpResponseMessage message = null;
            othersAccountChargeMdl response = new othersAccountChargeMdl();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            string req = string.Format(@"SEARCH VALUE: [{0}]", searchValue);
            LogWriter._chargeProd(TAG, string.Format(@"[>>] Request: ({0}), ", req));
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    
                    if (token == "YGHM9SHBC81LMR4G")
                    {
                        DataTable dt = dtS(searchValue);
                        if (dt.Rows.Count != 0)
                        {
                            string isprepaid = dt.Rows[0]["IS_PREPAID"].ToString();
                            string fName = dt.Rows[0]["SUBSCRIBER_FNAME"].ToString();
                            string lName = dt.Rows[0]["SUBSCRIBER_LNAME"].ToString();
                            string card = dt.Rows[0]["CARD_NO"].ToString();
                            string admin = dt.Rows[0]["PHONE_NO"].ToString();
                            if (isprepaid != "2")
                            {

                                response.isSuccess = true;
                                response.resultCode = HttpStatusCode.OK.ToString();
                                response.firstName = fName;
                                response.lastName = convertors.replaceName(lName);
                                response.cardNo = card;
                                response.adminNo = admin;
                                response.resultMessage = "success";
                            }
                            else
                            {
                                response.isSuccess = false;
                                response.resultCode = HttpStatusCode.OK.ToString();
                                response.firstName = fName;
                                response.lastName = convertors.replaceName(lName);
                                response.cardNo = card;
                                response.adminNo = admin;
                                response.resultMessage = "Дараа төлбөрт хэрэглэгч тул ашиглах боломжгүй.";

                            }
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = "Хайлтын үр дүн хоосон байна.";
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
        /// Бусдын данс цэнэглэх сервис. Хэрэглэгч шалгах сервисээс амжилттай хариу ирсэн тохиолдолд ашиглана.
        /// </summary>
        /// <param name="amount">Мөнгөн дүн (mandatory)</param>
        /// <param name="bankName">Банкны нэр (mandatory)</param>
        /// <param name="cardNo">Шалгасан хэрэглэгчийн картын дугаар (mandatory)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{amount}/{bankName}/{cardNo}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModelWidthVat>))]
        public async Task<HttpResponseMessage> Get(string amount, string bankName, string cardNo)
        {
            HttpResponseMessage message = null;
            defaultResponseModelWidthVat response = new defaultResponseModelWidthVat();
            _eBarimtRequest ebarimt = new _eBarimtRequest();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            string req = string.Format(@"AMOUNT: [{0}], BANKNAME: [{1}], CARDNO: [{2}]", amount, bankName, cardNo);
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
                            ebarimt.cardNo = cardNo;
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
                            stock.productId = "8";
                            stock.productName = "Данс цэнэглэх үйлчилгээ";
                            stock.unit = "ш";
                            stock.qty = "1";
                            detials.Add(stock);
                            ebarimt.transaction = detials;
                            string desc = string.Format(@"[Other's Charge Account] Mobile App emerchant {0}", bankName);
                            if (dbconn.chargeAccount(cardNo, amount, userAdminNo, desc))
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
                                response.resultMessage = "Данс цэнэглэхэд алдаа гарлаа";
                            }
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = "Дараа төлбөрт хэрэглэгч Данс цэнэглэх боломжгүй.";
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


        private bool checkCustomType(string cardNo)
        {
            bool res = false;
            try
            {
                DataTable dt = dbconn.getTable(appServiceQry.checkCard(cardNo));
                if (dt.Rows.Count != 0)
                {
                    string type = dt.Rows[0]["IS_PREPAID"].ToString();
                    if (type == "1")
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
        private DataTable dtS(string inS)
        {
            DataTable dt = new DataTable();
            try
            {
                if (inS.Length == 8)
                {
                    dt = dbconn.getTable(tabletQuery.getUserInfoByAdminNo(inS));
                }
                else
                {
                    string cardno = string.Empty;
                    if (correctionalFunc.cardFunc(inS, out cardno))
                    {
                        dt = dbconn.getTable(tabletQuery.getUserInfoByCardNo(cardno));
                    }
                    else
                    {
                        dt.Clear(); ;
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter._error(TAG, string.Format("Input: [{0}], Exception: [{1}]", inS, ex.ToString()));
                dt.Clear();
            }
            return dt;
        }
    }
}

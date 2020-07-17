using AppServiceUtil.DBControl;
using AppServiceUtil.TabletModel;
using AppServiceUtil.Utils;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace AppInstallerService.Controllers
{
    [RoutePrefix("v1/charge/Account")]
    public class chargeAccountController : ApiController
    {
        private string TAG = "chargeAccountController";
        DBControlNew dbconn = new DBControlNew();
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// Хэрэглэгчийн данс цэнэглэх сервис
        /// </summary>
        /// <param name="amount">Мөнгөн дүн</param>
        /// <param name="cardNo">Картын дугаах</param>
        /// <param name="customerNo">Албан байгуулгаар баримт авах бол Байгуулгын РД оруулна, Хувь хэрэглэгч бол хоосон байна.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{amount}/{cardNo}/{customerNo}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModelWidthVatTablet>))]
        public async Task<HttpResponseMessage> Get(string amount, string cardNo, string customerNo)
        {
            HttpResponseMessage message = new HttpResponseMessage();
            defaultResponseModelWidthVatTablet response = new defaultResponseModelWidthVatTablet();
            _eBarimtRequest ebarimt = new _eBarimtRequest();
            string secToken = string.Empty;
            string ip = httpUtil.GetClientIPAddress(HttpContext.Current.Request);
            LogWriter._chargeProd(TAG, string.Format(@"[>>] Request: [amount:{0}, card:{1}, customerNo:{2}]", amount, cardNo, customerNo));
            try
            {
                secToken = HttpContext.Current.Request.Headers["Authorization"].Replace("Basic ", "").Trim();
                if (dbconn.idbStatOK())
                {
                    string insId = string.Empty;
                    string insPhone = string.Empty;
                    if (dbconn.tabletCheckToken(secToken, out insId, out insPhone))
                    {
                        ebarimt.cardNo = cardNo;
                        ebarimt.channelNo = "11";
                        ebarimt.customerEmail = string.Empty;
                        ebarimt.sendEmail = false;
                        ebarimt.employeeCode = insPhone;
                        if (customerNo != "0")
                        {
                            ebarimt.organization = true;
                            ebarimt.customerNo = customerNo;
                        }
                        else
                        {
                            ebarimt.organization = false;
                            ebarimt.customerNo = string.Empty;
                        }
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
                        string desc = string.Format(@"[Charge Account] Installer App");
                        if (dbconn.chargeAccount(cardNo, amount, insPhone, desc, "11"))
                        {
                            response.isSuccess = true;
                            response.errorCode = Convert.ToString((int)HttpStatusCode.OK);
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
                            response.errorCode = Convert.ToString((int)HttpStatusCode.Conflict);
                            response.resultMessage = "Данс цэнэглэхэд алдаа гарлаа";
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
                LogWriter._error(TAG, string.Format(@"Token: [{0}], Exception: [{1}]", secToken, ex.ToString()));
                response.isSuccess = false;
                response.errorCode = Convert.ToString((int)HttpStatusCode.InternalServerError);
                response.resultMessage = ex.Message;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._chargeProd(TAG, string.Format(@"[<<] ClientIp: [{0}], Response: [{1}]", ip, serializer.Serialize(response)));
            return message;
        }

        /// <summary>
        /// НӨАТ хэвлэх компани шалгах сервис
        /// </summary>
        /// <param name="customerNo">Байгуулгын Регистрийн дугаар</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{customerNo}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<DefaultResponse>))]
        public async Task<HttpResponseMessage> Get(string customerNo)
        {
            HttpResponseMessage message = new HttpResponseMessage();
            DefaultResponse response = new DefaultResponse();
            string secToken = string.Empty;
            string ip = httpUtil.GetClientIPAddress(HttpContext.Current.Request);
            LogWriter._chargeProd(TAG, string.Format(@"[>>] Request: [customerNo:{0}]", customerNo));
            try
            {
                secToken = HttpContext.Current.Request.Headers["Authorization"].Replace("Basic ", "").Trim();
                if (dbconn.idbStatOK())
                {
                    string insId = string.Empty;
                    string insPhone = string.Empty;
                    if (dbconn.tabletCheckToken(secToken, out insId, out insPhone))
                    {
                        checkCompanyRequest checkCompany = new checkCompanyRequest();
                        checkCompany.customerNo = customerNo;
                        int sttCode = 0;
                        string resp = string.Empty;
                        if (httpWorker.http_POST("http://192.168.10.182:5050/vat/checkCompany", serializer.Serialize(checkCompany), out sttCode, out resp))
                        {
                            checkCompanyResult result = serializer.Deserialize<checkCompanyResult>(resp);
                            if (result.found)
                            {
                                response.isSuccess = true;
                                response.errorCode = Convert.ToString((int)HttpStatusCode.OK);
                                response.resultMessage = result.name;
                            }
                            else
                            {
                                response.isSuccess = false;
                                response.errorCode = Convert.ToString((int)HttpStatusCode.NoContent);
                                response.resultMessage = "Хайлтын үр дүн хоосон байна.";
                            }
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.errorCode = Convert.ToString((int)HttpStatusCode.NotFound);
                            response.resultMessage = "Хайлтын үр дүн хоосон байна.";
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
                LogWriter._error(TAG, string.Format(@"Token: [{0}], Exception: [{1}]", secToken, ex.ToString()));
                response.isSuccess = false;
                response.errorCode = Convert.ToString((int)HttpStatusCode.InternalServerError);
                response.resultMessage = ex.Message;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._chargeProd(TAG, string.Format(@"[<<] ClientIp: [{0}], Response: [{1}]", ip, serializer.Serialize(response)));
            return message;
        }

    }
}

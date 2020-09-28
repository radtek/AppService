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
    [RoutePrefix("api/qpay")]
    public class qPayController : ApiController
    {
        DBConnection dbconn = new DBConnection();
        private string TAG = "qPayController";
        private string dbres = string.Empty;
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// [Webhook] Гүйлгээ амжилттай болсон эсэхийг хүлээн авах сервис.
        /// </summary>
        /// <param name="invoiceId">Тухайн гүйлгээнд харгалзах Invoice no байна.</param>
        /// <returns></returns>
        [HttpGet]
        //[Route("{amount}/{bankName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModel>))]
        public async Task<HttpResponseMessage> Get(string invoiceId)
        {
            HttpResponseMessage message = new HttpResponseMessage();
            defaultResponseModel response = new defaultResponseModel();
            LogWriter._qPay(TAG, string.Format(@"[>>] Request: ({0}), ", invoiceId));
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string commandResult = string.Empty;
                    bool cmp = completeTrans(invoiceId, out commandResult);
                    if (cmp)
                    {
                        string cmdRes = dbconn.iDBCommand(appServiceQry.updateInvTrans(invoiceId));
                        LogWriter._qPay(TAG, string.Format("Update Command Ressult: [{0}]", cmdRes));
                    }
                    response.isSuccess = cmp;
                    response.resultCode = HttpStatusCode.OK.ToString();
                    response.resultMessage = commandResult;
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
            LogWriter._qPay(TAG, string.Format("[<<] IP: [{0}], Response: [{1}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), serializer.Serialize(response)));
            return message;
        }

        /// <summary>
        /// Гүйлгээ амжилттай болсон эсэхийг шалгах сервис.
        /// </summary>
        /// <param name="sourceName">{MobileApp is 18}, {Web is 26}, {STB is 34}</param>
        /// <param name="invoiceId">Клиент талаас үүсгэсэн invoice_no байна.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{sourceName}/{invoiceId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModel>))]
        public async Task<HttpResponseMessage> Get(string sourceName, string invoiceId)
        {
            HttpResponseMessage message = new HttpResponseMessage();
            defaultResponseModel response = new defaultResponseModel();
            LogWriter._qPay(TAG, string.Format(@"[>>] Request: (InvId: {0}, SourceName:{1}), ", invoiceId, sourceName));
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            try
            {
                if(token == "YGHM9SHBC81LMR4G")
                {
                    if (dbconn.idbCheck(out dbres))
                    {
                        if (invoiceId.Length != 0)
                        {
                            DataTable dt = dbconn.getTable(appServiceQry.getQpayInvoice(invoiceId));
                            if (dt.Rows.Count != 0)
                            {
                                string invId = dt.Rows[0]["STATUS"].ToString();
                                if (invId == "C")
                                {
                                    response.isSuccess = true;
                                    response.resultCode = HttpStatusCode.OK.ToString();
                                    response.resultMessage = "success";
                                }
                                else
                                {
                                    response.isSuccess = false;
                                    response.resultCode = HttpStatusCode.NotFound.ToString();
                                    response.resultMessage = "transaction is failed";
                                }
                            }
                            else
                            {
                                response.isSuccess = false;
                                response.resultCode = HttpStatusCode.NotFound.ToString();
                                response.resultMessage = "no Invoice";
                            }
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = "bad invoice_no";
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
                response.isSuccess = false;
                response.resultCode = HttpStatusCode.NotFound.ToString();
                response.resultMessage = ex.Message;
                exceptionManager.ManageException(ex, TAG);
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._qPay(TAG, string.Format("[<<] IP: [{0}], Response: [{1}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), serializer.Serialize(response)));
            return message;
        }

        /// <summary>
        /// Source тус бүрээс дахин давхардахгүй InvoiceNo generate хийх 
        /// </summary>
        /// <param name="sourceName">{MobileApp is 18}, {Web is 26}, {STB is 34}</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{sourceName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModel>))]
        public async Task<HttpResponseMessage> GetInvoice(string sourceName)
        {
            HttpResponseMessage message = new HttpResponseMessage();
            defaultResponseModel response = new defaultResponseModel();
            LogWriter._qPay(TAG, string.Format(@"[>>] Request: ({0}), ", sourceName));
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            try
            {
                if (token == "YGHM9SHBC81LMR4G")
                {
                    string genInvoice = string.Empty;
                    switch (sourceName)
                    {
                        case "18":
                            genInvoice = convertors.generateInvoiceNo("1");
                            break;
                        case "26":
                            genInvoice = convertors.generateInvoiceNo("3");
                            break;
                        case "34":
                            genInvoice = convertors.generateInvoiceNo("5");
                            break;
                        default:
                            genInvoice = string.Empty;
                            break;
                    }
                     
                    response.isSuccess = true;
                    response.resultCode = HttpStatusCode.OK.ToString();
                    response.resultMessage = genInvoice;

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
                response.isSuccess = false;
                response.resultCode = HttpStatusCode.NotFound.ToString();
                response.resultMessage = ex.Message;
                exceptionManager.ManageException(ex, TAG);
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._qPay(TAG, string.Format("[<<] IP: [{0}], Response: [{1}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), serializer.Serialize(response)));
            return message;
        }

        private bool completeTrans(string invNo, out string resultMessage)
        {
            bool result = false;
            resultMessage = string.Empty;
            try
            {
                DataTable dt = dbconn.getTable(appServiceQry.checkInvTrans(invNo));
                if (dt.Rows.Count != 0)
                {
                    string trId = dt.Rows[0]["ID"].ToString();
                    string type = dt.Rows[0]["REQUEST_TYPE"].ToString();
                    string cardno = dt.Rows[0]["CARD_NO"].ToString();
                    string phone = dt.Rows[0]["PHONE_NO"].ToString();
                    string invoice = dt.Rows[0]["INVOICE_NO"].ToString();
                    string productid = dt.Rows[0]["PRODUCT_ID"].ToString();
                    string month = dt.Rows[0]["MONTH"].ToString();
                    string amount = dt.Rows[0]["AMOUNT"].ToString();
                    string bank = dt.Rows[0]["BANKNAME"].ToString();
                    string smscode = dt.Rows[0]["SMSCODE"].ToString();
                    string indate = dt.Rows[0]["INDATE"].ToString();
                    switch (type)
                    {
                        case "1001":
                            // charge Product
                            result = chargeProduct(cardno, phone, productid, month, amount, bank, out resultMessage);
                            break;
                        case "1004":
                            // order Nvod
                            result = orderNvod(cardno, phone, productid, amount, bank, smscode, indate, out resultMessage);
                            break;
                        case "1007":
                            // charge Account
                            result = chargeAccount(cardno, phone, amount, bank, out resultMessage);
                            break;
                        case "1010":
                            // others charge Account
                            result = chargeAccount(cardno, phone, amount, bank, out resultMessage);
                            break;
                        case "1013":
                            // upgrade Product
                            result = upgradeProduct(cardno, phone, productid, amount, bank, out resultMessage);
                            break;
                        default:
                            result = false;
                            resultMessage = "Гүйлгээний төрөл таарсангүй.";
                            break;
                    }
                }
                else
                {
                    result = false;
                    resultMessage = "Гүйлгээ олдсонгүй.";
                }
            }
            catch(Exception ex)
            {
                result = false;
                LogWriter._error(TAG, ex.Message);
            }
            return result;
        }

        private bool chargeProduct(string card, string phone, string productId, string month, string amount, string bankName, out string message)
        {
            bool cp = false;
            message = string.Empty;
            _eBarimtRequest ebarimt = new _eBarimtRequest();
            try
            {
                ebarimt.cardNo = card;
                ebarimt.channelNo = "6";
                ebarimt.customerEmail = string.Empty;
                ebarimt.sendEmail = false;
                ebarimt.employeeCode = phone;
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
                if (dbconn.chargeProduct(productId, month, phone, amount, desc, card, "6"))
                {
                    cp = true;
                    int sttCode = 0;
                    string resp = string.Empty;
                    if (httpWorker.http_POST("http://192.168.10.182:5050/vat/getEBarimt", serializer.Serialize(ebarimt), out sttCode, out resp))
                    {
                       
                        _eBarimtResponse mta = serializer.Deserialize<_eBarimtResponse>(resp);
                        if (mta.isSuccess)
                        {
                            //response.mtaResult = new MTAResult { merchantId = mta.merchantId, amount = mta.amount, billId = mta.billId, date = mta.resultDate, loterryNo = mta.lotteryNo, qrData = mta.qrData, tax = mta.cityTax, vat = mta.vat };
                            //response.resultMessage = "success";
                            message = "Амжилттай";
                        }
                        else
                        {
                            message = "Ebarimt гаргахад алдаа гарлаа. Лавлах: 77771434, 1434";
                        }

                    }
                    else
                    {
                        message = "Ebarimt гаргахад алдаа гарлаа. Лавлах: 77771434, 1434";
                    }
                }
                else
                {
                    cp = false;
                    message = "Багц сунгахад алдаа гарлаа. Лавлах: 77771434, 1434";
                }
            }
            catch(Exception ex)
            {
                LogWriter._error(TAG, ex.Message);
                message = ex.Message;
            }
            return cp;
        }

        private bool orderNvod(string _card, string _phone, string productId, string amount, string bankName, string smsCode, string inDate, out string message)
        {
            bool addnvod = false;
            message = string.Empty;
            _eBarimtRequest ebarimt = new _eBarimtRequest();
            try
            {
                ebarimt.cardNo = _card;
                ebarimt.channelNo = "6";
                ebarimt.customerEmail = string.Empty;
                ebarimt.sendEmail = false;
                ebarimt.employeeCode = _phone;
                ebarimt.organization = false;
                ebarimt.customerNo = string.Empty;
                var detials = new List<_transactionDetial>();
                var stock = new _transactionDetial();
                stock.barCode = "8463100";
                stock.price = amount;
                stock.productId = productId;
                stock.productName = "Kино сан түрээслэх үйлчилгээ";
                stock.unit = "ш";
                stock.qty = "1";
                detials.Add(stock);
                ebarimt.transaction = detials;
                string desc = string.Format(@"[Order VOD] Mobile App emerchant {0}", bankName);
                if (dbconn.chargeAccount(_card, amount, _phone, desc))
                {
                    string resMon = string.Empty;
                    string resEng = string.Empty;
                    string resCry = string.Empty;
                    if (dbconn.addNvodByCounter(_card, _phone, inDate, smsCode, productId, out resEng, out resMon, out resCry))
                    {
                        addnvod = true;
                        int sttCode = 0;
                        string resp = string.Empty;
                        if (httpWorker.http_POST("http://192.168.10.182:5050/vat/getEBarimt", serializer.Serialize(ebarimt), out sttCode, out resp))
                        {
                            _eBarimtResponse mta = serializer.Deserialize<_eBarimtResponse>(resp);
                            if (mta.isSuccess)
                            {
                                //response.mtaResult = new MTAResult { merchantId = mta.merchantId, amount = mta.amount, billId = mta.billId, date = mta.resultDate, loterryNo = mta.lotteryNo, qrData = mta.qrData, tax = mta.cityTax, vat = mta.vat };
                                //response.resultMessage = "success";
                                message = "Амжилттай";
                            }
                            else
                            {
                                message = "Ebarimt гаргахад алдаа гарлаа. Лавлах: 77771434, 1434";
                            }
                        }
                        else
                        {
                            message = "Ebarimt гаргахад алдаа гарлаа. Лавлах: 77771434, 1434";
                        }
                    }
                    else
                    {
                        message = resMon;
                    }
                }
                else
                {
                    message = "Данс цэнэглэхэд алдаа гарлаа. Лавлах: 77771434, 1434";
                }
            }
            catch(Exception ex)
            {
                LogWriter._error(TAG, ex.Message);
                message = ex.Message;
            }
            return addnvod;
        }

        private bool chargeAccount(string _card, string _phone, string amount, string bankName, out string message)
        {
            bool ca = false;
            message = string.Empty;
            _eBarimtRequest ebarimt = new _eBarimtRequest();
            try
            {
                ebarimt.cardNo = _card;
                ebarimt.channelNo = "6";
                ebarimt.customerEmail = string.Empty;
                ebarimt.sendEmail = false;
                ebarimt.employeeCode = _phone;
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
                if (dbconn.chargeAccount(_card, amount, _phone, desc))
                {
                    ca = true;
                    int sttCode = 0;
                    string resp = string.Empty;
                    if (httpWorker.http_POST("http://192.168.10.182:5050/vat/getEBarimt", serializer.Serialize(ebarimt), out sttCode, out resp))
                    {
                        _eBarimtResponse mta = serializer.Deserialize<_eBarimtResponse>(resp);
                        if (mta.isSuccess)
                        {
                            //response.mtaResult = new MTAResult { merchantId = mta.merchantId, amount = mta.amount, billId = mta.billId, date = mta.resultDate, loterryNo = mta.lotteryNo, qrData = mta.qrData, tax = mta.cityTax, vat = mta.vat };
                            //response.resultMessage = "success";
                            message = "Амжилттай";
                        }
                        else
                        {
                            message = "Ebarimt гаргахад алдаа гарлаа. Лавлах: 77771434, 1434";
                        }

                    }
                    else
                    {
                        message = "Ebarimt гаргахад алдаа гарлаа. Лавлах: 77771434, 1434";
                    }
                }
                else
                {
                    message = "Данс цэнэглэхэд алдаа гарлаа. Лавлах: 77771434, 1434";
                }
            }
            catch(Exception ex)
            {
                LogWriter._error(TAG, ex.Message);
            }
            return ca;
        }
        private bool upgradeProduct(string _card, string _phone, string toProductId, string amount, string bankName, out string message)
        {
            bool up = false;
            message = string.Empty;
            _eBarimtRequest ebarimt = new _eBarimtRequest();
            try
            {
                ebarimt.cardNo = _card;
                ebarimt.channelNo = "6";
                ebarimt.customerEmail = string.Empty;
                ebarimt.sendEmail = false;
                ebarimt.employeeCode = _phone;
                ebarimt.organization = false;
                ebarimt.customerNo = string.Empty;
                var detials = new List<_transactionDetial>();
                var stock = new _transactionDetial();
                stock.barCode = "8463100";
                stock.price = amount;
                stock.productId = "8";
                stock.productName = "Багц ахиулах үйлчилгээ";
                stock.unit = "ш";
                stock.qty = "1";
                detials.Add(stock);
                ebarimt.transaction = detials;
                //---
                localConvertProdcutMdl convProd = new localConvertProdcutMdl();
                convProd.BranchId = "286";
                convProd.Channel = "6";
                convProd.cardNo = _card;
                convProd.ConvertProduct = toProductId;
                convProd.Pay_type = "0";
                convProd.Username = _phone;
                string convJson = serializer.Serialize(convProd);
                string localResponse = string.Empty;
                if (httpUtil.httpCall_POST_local(convJson, "processProduct", out localResponse))
                {
                    LogWriter._qPay(TAG, string.Format("[Upgrade Product] Local Service Response: [{0}]", localResponse));
                    var convObj = serializer.Deserialize<localCheckProductResponse>(localResponse);
                    if (convObj.issuccess)
                    {
                        up = true;
                        int sttCode = 0;
                        string resp = string.Empty;
                        if (httpWorker.http_POST("http://192.168.10.182:5050/vat/getEBarimt", serializer.Serialize(ebarimt), out sttCode, out resp))
                        {
                            _eBarimtResponse mta = serializer.Deserialize<_eBarimtResponse>(resp);
                            if (mta.isSuccess)
                            {
                                //response.mtaResult = new MTAResult { merchantId = mta.merchantId, amount = mta.amount, billId = mta.billId, date = mta.resultDate, loterryNo = mta.lotteryNo, qrData = mta.qrData, tax = mta.cityTax, vat = mta.vat };
                                //response.resultMessage = "success";
                                message = "Амжилттай";
                            }
                            else
                            {
                                message = "Ebarimt гаргахад алдаа гарлаа. Лавлах: 77771434, 1434";
                            }

                        }
                        else
                        {
                            message = "Ebarimt гаргахад алдаа гарлаа. Лавлах: 77771434, 1434";
                        }
                    }
                    else
                    {
                        message = "Багц ахиулахад алдаа гарлаа. Лавлах: 77771434, 1434";
                    }
                }
                else
                {
                    message = "Багц ахиулахад алдаа гарлаа. Лавлах: 77771434, 1434";
                }

            }
            catch(Exception ex)
            {
                LogWriter._error(TAG, ex.Message);
            }
            return up;
        }
    }
}

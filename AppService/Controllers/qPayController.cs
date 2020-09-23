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
        /// [Webhook] Гүйлгээ амжилттай болсон эсэхийг хүлээн авах сервис. Qpay талаас дуудна.
        /// </summary>
        /// <param name="invoiceId">Клиент талаас үүсгэсэн invoice_no байна.</param>
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
                    if(invoiceId.Length != 0)
                    {
                        string result = dbconn.iDBCommand(appServiceQry.setQpayInvoice(invoiceId));
                        if (result.Contains("FFFFx["))
                        {
                            LogWriter._error(TAG, result);
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.InternalServerError.ToString();
                            response.resultMessage = "falied";
                        }
                        else
                        {
                            response.isSuccess = true;
                            response.resultCode = HttpStatusCode.OK.ToString();
                            response.resultMessage = "success";
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
                                if (invId == "0")
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
    }
}

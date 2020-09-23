using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AppServiceUtil.AppserviceModel;
using AppServiceUtil.Utils;
using AppServiceUtil.DBControl;
using System.Web;
using System.Web.Script.Serialization;
using Swashbuckle.Swagger.Annotations;
using AppService.Models;
using Newtonsoft.Json;
using AppServiceUtil.chatbotModel;

namespace AppService.Controllers
{
    [RoutePrefix("api/chatbotGetCardNo")]
    public class chatbotGetCardNoController : ApiController
    {
        private string TAG = "ChatBot";
        DBConnection dbconn = new DBConnection();
        JavaScriptSerializer serialzer = new JavaScriptSerializer();
        private string dbres = string.Empty;
        /// <summary>
        /// [only ChatBot] Хэрэглэгчийн картын дугаар авах сервис
        /// </summary>
        /// <param name="phoneNo"> Админ дугаар (mandatory)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{phoneNo}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<getCardNoMdl>))]
        public HttpResponseMessage Get(string phoneNo)
        {
            LogWriter._chatBot(TAG, string.Format(@"[>>] Request: [{0}]", phoneNo));
            HttpResponseMessage message = null;
            getCardNoMdl response = new getCardNoMdl();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            try
            {
                if (token == "YGHM9SHBC81LMR4G")
                {
                    if(dbconn.idbCheck(out dbres))
                    {
                        DataTable dt = dbconn.getTable(chatbotQry._getCardNo(phoneNo));
                        if(dt.Rows.Count != 0)
                        {
                            string cardNo = dt.Rows[0]["CARD_NO"].ToString();
                            string adminNumber = dt.Rows[0]["PHONE_NO"].ToString();
                            string confirmCode = convertors.getUniqueKey(4);
                            string smsText = string.Format("DDISH OFFICIAL Batalgaajuulah kod: {0}", confirmCode);
                            string command = dbconn.iDBCommand(chatbotQry._sendSMS(adminNumber, smsText));
                            if (command.Contains("FFFFx["))
                            {
                                response.isSuccess = false;
                                response.resultCode = HttpStatusCode.NotFound.ToString();
                                response.resultMessage = "Could not send sms";
                            }
                            else
                            {
                                response.isSuccess = true;
                                response.resultCode = HttpStatusCode.OK.ToString();
                                response.resultMessage = appConstantValues.MSG_SUCCESS;
                                response.phoneNo = adminNumber;
                                response.cardNo = cardNo;
                                response.confirmCode = confirmCode;
                            }
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = appConstantValues.MSG_NOFOUND;
                        }
                    }
                    else
                    {
                        response.isSuccess = false;
                        response.resultCode = HttpStatusCode.NotFound.ToString();
                        response.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
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
                exceptionManager.ManageException(ex, TAG);
                response.isSuccess = false;
                response.resultCode = HttpStatusCode.NotFound.ToString();
                response.resultMessage = ex.Message;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._chatBot(TAG, string.Format("[<<] IP: [{0}], Response: [{1}], Token: [{2}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), serialzer.Serialize(response), token));
            return message;
        }

        /// <summary>
        /// [only ChatBot] Хэрэглэгчийн идэвхтэй багцын жагсаалтыг админ дугаараар авах сервис
        /// </summary>
        /// <param name="adminNo">Админ дугаар (mandatory)</param>
        /// <param name="activeProducts">defualt value is Yes</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{adminNo}/{activeProducts}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<activeProductsMdl>))]
        public HttpResponseMessage GetAcitveProduct(string adminNo, string activeProducts)
        {
            LogWriter._chatBot(TAG, string.Format(@"[>>] Request: [{0}]", adminNo));
            HttpResponseMessage message = new HttpResponseMessage() ;
            activeProductsMdl response = new activeProductsMdl();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            try
            {
                if (token == "YGHM9SHBC81LMR4G")
                {
                    if (dbconn.idbCheck(out dbres))
                    {
                        DataTable dt = dbconn.getTable(chatbotQry._getActiveProducts(adminNo));
                        if (dt.Rows.Count != 0)
                        {
                            string cardno = dt.Rows[0]["CARD_NUMBER"].ToString();
                            List<chatbot_productDetial> prods = new List<chatbot_productDetial>();
                            foreach(DataRow item in dt.Rows)
                            {
                                chatbot_productDetial prodDetial = new chatbot_productDetial();
                                string prodId = item["PRODUCT_ID"].ToString();
                                string prodName = item["PRODUCT_NAME_MON"].ToString();
                                string endDate = item["ENDD"].ToString();
                                prodDetial.productId = prodId;
                                prodDetial.productName = prodName;
                                prodDetial.endDate = endDate;
                                prods.Add(prodDetial);
                            }
                            response.isSuccess = true;
                            response.resultCode = HttpStatusCode.OK.ToString();
                            response.resultMessage = appConstantValues.MSG_SUCCESS;
                            response.cardNo = cardno;
                            response.products = prods;
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = appConstantValues.MSG_NOFOUND;
                        }
                    }
                    else
                    {
                        response.isSuccess = false;
                        response.resultCode = HttpStatusCode.NotFound.ToString();
                        response.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
                    }
                }
                else
                {
                    response.isSuccess = false;
                    response.resultCode = HttpStatusCode.Unauthorized.ToString();
                    response.resultMessage = appConstantValues.MSG_EXPIRED;
                }
            }
            catch(Exception ex)
            {
                exceptionManager.ManageException(ex, TAG);
                response.isSuccess = false;
                response.resultCode = HttpStatusCode.NotFound.ToString();
                response.resultMessage = ex.Message;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._chatBot(TAG, string.Format("[<<] IP: [{0}], Response: [{1}], Token: [{2}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), serialzer.Serialize(response), token));
            return message;
        }


        /// <summary>
        /// [only ChatBot] Хэрэглэгчийн facebook id хадгалах сервис
        /// </summary>
        /// <param name="adminNo"></param>
        /// <param name="facebookId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("saveFB/{adminNo}/{facebookId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModel>))]
        public HttpResponseMessage GetSaveFBId(string adminNo, string facebookId)
        {
            LogWriter._chatBot(TAG, string.Format(@"[>>] Request: [{0}], FacebookId: [{1}]", adminNo, facebookId));
            HttpResponseMessage message = new HttpResponseMessage();
            defaultResponseModel response = new defaultResponseModel();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            try
            {
                if (token == "YGHM9SHBC81LMR4G")
                {
                    if (dbconn.idbCheck(out dbres))
                    {
                        string res = dbconn.iDBCommand(chatbotQry._saveFbId(facebookId, adminNo));
                        if (res.Contains("FFFFx["))
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = "Could not save facebook id.";
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
                        response.resultCode = HttpStatusCode.NotFound.ToString();
                        response.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
                    }
                }
                else
                {
                    response.isSuccess = false;
                    response.resultCode = HttpStatusCode.Unauthorized.ToString();
                    response.resultMessage = appConstantValues.MSG_EXPIRED;
                }
            }
            catch(Exception ex)
            {
                exceptionManager.ManageException(ex, TAG);
                response.isSuccess = false;
                response.resultCode = HttpStatusCode.NotFound.ToString();
                response.resultMessage = ex.Message;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._chatBot(TAG, string.Format("[<<] IP: [{0}], Response: [{1}], Token: [{2}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), serialzer.Serialize(response), token));
            return message;
        }
    }
}

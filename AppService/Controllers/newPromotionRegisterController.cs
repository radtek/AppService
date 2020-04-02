using AppService.Models;
using AppServiceUtil.AppserviceModel;
using AppServiceUtil.Auth;
using AppServiceUtil.DBControl;
using AppServiceUtil.Utils;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace AppService.Controllers
{
    [RoutePrefix("api/newPromotionRegister")]
    public class newPromotionRegisterController : ApiController
    {
        DBConnection dbconn = new DBConnection();
        private string TAG = "newPromotionRegister";
        private string dbres = string.Empty;
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModel>))]
        public HttpResponseMessage Post([FromBody] req_newPromotionRegisterModel injson)
        {
            HttpResponseMessage message = null;
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            
            defaultResponseModel response = new defaultResponseModel();
            try
            {
                // new promo--- this is ok

                LogWriter._promo(TAG, string.Format("[>>] Request: [{0}]", serializer.Serialize(injson)));
                if(token == "YGHM9SHBC81LMR4G")
                {
                    if (injson != null)
                    {
                        if (dbconn.idbCheck(out dbres))
                        {
                            string promoName = injson.promoName;
                            string promoText = injson.promoText;
                            string poster = injson.promoPosterImageUrl;
                            string expire = injson.expireDate;
                            int promoId = 0;
                            if (dbconn.iDBCommandRetID(chatbotQry._addNewPromo(promoName, promoText, poster, expire), out promoId))
                            {
                                string detRes = string.Empty;
                                foreach (var item in injson.detialImageUrls)
                                {
                                    if (promoId != 0)
                                    {
                                        string dbresult = dbconn.iDBCommand(chatbotQry._addNewPromoDetial(promoId.ToString(), item.detialImageUrls));
                                        LogWriter._promo(TAG, string.Format("Promo Id: [{0}], ImgUrl: [{1}], Result: [{2}]", promoId, item.detialImageUrls, dbresult));
                                    }
                                }
                                response.isSuccess = true;
                                response.resultCode = HttpStatusCode.OK.ToString();
                                response.resultMessage = "success";
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
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
                            LogWriter._error(TAG, dbres);
                        }
                    }
                    else
                    {
                        response.isSuccess = false;
                        response.resultCode = HttpStatusCode.NotFound.ToString();
                        response.resultMessage = "Хоосон утга";
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
                response.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._promo(TAG, string.Format("[<<] Ip: [{0}], Token: [{1}], Response: [{2}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), token, serializer.Serialize(response)));
            return message;
        }

        //public HttpResponseMessage Get()
        //{
        //    //HttpResponseMessage = new HttpResponseMessage();
        //    HttpResponseMessage response = new HttpResponseMessage() ;
        //    return response;
        //}
    }
}

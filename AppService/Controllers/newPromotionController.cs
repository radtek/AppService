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
    [RoutePrefix("api/newPromotion")]
    public class newPromotionController : ApiController
    {
        DBConnection dbconn = new DBConnection();
        private string TAG = "newPromotion";
        private string dbres = string.Empty;
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// Шинээр зарлагдсан урамшуулал авах service.
        /// </summary>
        /// <remarks>promotionImg энэ талбар нь эхний жагсаалтанд харагдах зураг байна. promotionDetialImg энэ талбар нь дэлгэрэнгүй рүү ороход харагдах зураг. Login хийхгүй байхад харах боломжтой учираас нөгөө token оо ашиглана.</remarks>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<newPromotionModel>))]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage message = null;
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            newPromotionModel response = new newPromotionModel();
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    if (token == "YGHM9SHBC81LMR4G")
                    {
                        List<promotions> promo = new List<promotions>();
                        DataTable dt = dbconn.getTable(appServiceQry._getNewPromotion());
                        foreach(DataRow item in dt.Rows)
                        {
                            promotions pr = new promotions();
                            pr.promotionName = item["PROMOTION_NAME"].ToString();
                            pr.promotionText = item["PROMOTION_TEXT"].ToString();
                            pr.promotionImg = item["PROMOTION_IMG"].ToString();
                            DataTable dt_det = dbconn.getTable(appServiceQry._getNewPromotionDetial(item["PROMOTION_ID"].ToString()));
                            List<promoDetials> prDetial = new List<promoDetials>();
                            foreach(DataRow dr in dt_det.Rows)
                            {
                                promoDetials prdet = new promoDetials();
                                prdet.promoId = dr["PROMOTION_ID"].ToString();
                                prdet.detialPoster = dr["DETIAL_POSTER_URL"].ToString();
                                prDetial.Add(prdet);
                            }
                            pr.promoDetials = prDetial;
                            promo.Add(pr);
                        }
                        response.isSuccess = true;
                        response.resultCode = HttpStatusCode.OK.ToString();
                        response.resultMessage = "success";
                        response.promotions = promo;
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
            LogWriter._promo(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), "", serializer.Serialize(response), token));
            return message;
        }
        /// <summary>
        /// Антенн тохируулах зурган заавар авах сервис
        /// </summary>
        /// <param name="manual">Ямар нэгэн утга дамжуулахад болно.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{manual}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<antennaInstallationManualMdl>))]
        public HttpResponseMessage Get(string manual)
        {
            HttpResponseMessage message = null;
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            antennaInstallationManualMdl response = new antennaInstallationManualMdl();
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    if (token == "YGHM9SHBC81LMR4G")
                    {
                        DataTable dt = dbconn.getTable(appServiceQry._getAtennaInstallationManual());
                        List<AntennaManualDetial> manuals = new List<AntennaManualDetial>();
                        foreach(DataRow dr in dt.Rows)
                        {
                            AntennaManualDetial anten = new AntennaManualDetial();
                            anten.imgUrl = dr["IMAGE_URL"].ToString();
                            manuals.Add(anten);
                        }
                        response.isSuccess = true;
                        response.resultCode = HttpStatusCode.OK.ToString();
                        response.resultMessage = "success";
                        response.manuals = manuals;
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
            LogWriter._promo(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), manual, serializer.Serialize(response), token));
            return message;
        }

        /// <summary>
        /// Антенн тохируулах видео заавар авах сервис
        /// </summary>
        /// <param name="manual">defualt value is 0</param>
        /// <param name="video">defualt value is 0</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{manual}/{video}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<antennaInstallationVideoManualMdl>))]
        public HttpResponseMessage Get(string manual, string video)
        {
            HttpResponseMessage message = null;
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            antennaInstallationVideoManualMdl response = new antennaInstallationVideoManualMdl();
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    if (token == "YGHM9SHBC81LMR4G")
                    {
                        DataTable dt = dbconn.getTable(appServiceQry._getAtennaVideoManual());
                        List<videoDetial> vdDtl = new List<videoDetial>();
                        foreach(DataRow dr in dt.Rows)
                        {
                            videoDetial vd = new videoDetial();
                            vd.videoId = dr["VIDEO_ID"].ToString();
                            vd.videoName = dr["VIDEO_NAME"].ToString();
                            vdDtl.Add(vd);
                        }
                        response.isSuccess = true;
                        response.resultCode = HttpStatusCode.OK.ToString();
                        response.resultMessage = "success";
                        response.videoManuals = vdDtl;
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
                exceptionManager.ManageException(ex, TAG);
                response.isSuccess = false;
                response.resultCode = HttpStatusCode.NotFound.ToString();
                response.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._promo(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), string.Format("Manual: {0}, Video: {1}", manual, video), serializer.Serialize(response), token));
            return message;
        }
    }
}

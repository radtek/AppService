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
    [RoutePrefix("api/pushVod")]
    public class pushVodController : ApiController
    {
        DBConnection dbconn = new DBConnection();
        private string TAG = "pushVod";
        private string dbres = string.Empty;
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// Кино санд шинээр нэмэгдэх контентын жагсаалт
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<pushVodComingModel>))]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage message = null;
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            pushVodComingModel response = new pushVodComingModel();
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        List<comingContent> cnt = new List<comingContent>();
                        DataTable dt = dbconn.getTable(appServiceQry._getVODComing());
                        foreach(DataRow item in dt.Rows)
                        {
                            comingContent c = new comingContent();
                            c.contentName = item["CONTENT_NAME"].ToString();
                            c.contentImgUrl = item["CONTENT_IMG_URL"].ToString().Replace("192.168.10.93", "my.ddishtv.mn:808");
                            cnt.Add(c);
                        }
                        response.isSuccess = true;
                        response.resultCode = HttpStatusCode.OK.ToString();
                        response.resultMessage = "success";
                        response.contents = cnt;
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
            LogWriter._pushVod(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), "", serializer.Serialize(response), token));
            return message;
        }

        /// <summary>
        /// Кино сангаас контент захиалах service
        /// </summary>
        /// <param name="contentId">Захиалах контентын Id (mandatory)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{contentId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModel>))]
        public HttpResponseMessage Get(string contentId)
        {
            HttpResponseMessage message = null;
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            defaultResponseModel response = new defaultResponseModel();
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        // TODO. Энд кино захиалах код бичигдэнэ.
                        
                        string beginDate = DateTime.Now.ToString(appConstantValues.FORMAT_DATE_TIME_LONG);
                        string endDate = DateTime.Now.AddDays(3).ToString(appConstantValues.FORMAT_DATE_TIME_LONG);
                        string resMon = string.Empty;
                        string resEng = string.Empty;
                        string resCry = string.Empty;
                        if (checkCustomType(userCardNo))
                        {
                            if (dbconn.callARProcedure(userCardNo, contentId, beginDate, endDate, "0", userAdminNo, out resEng, out resMon, out resCry))
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
                exceptionManager.ManageException(ex, TAG);
                response.isSuccess = false;
                response.resultCode = HttpStatusCode.NotFound.ToString();
                response.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._pushVod(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), contentId, serializer.Serialize(response), token));
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
    }
}

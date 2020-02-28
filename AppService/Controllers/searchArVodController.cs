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

namespace AppService.Controllers
{
    [RoutePrefix("api/searchArVod")]
    public class searchArVodController : ApiController
    {
        DBConnection dbconn = new DBConnection();
        private string TAG = "searchArVod";
        private string dbres = string.Empty;
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// ARVOD болон PUSHVOD-н кино хайх сервис
        /// </summary>
        /// <param name="movieId">Захиалах контентийн ID байна. (mandatory)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{movieId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<searchArVodMdl>))]
        public HttpResponseMessage Get(string movieId)
        {
            LogWriter._noti(TAG, string.Format("[>>] Requist: [{0}]", movieId));
            HttpResponseMessage message = null;
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            searchArVodMdl response = new searchArVodMdl();
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        DataTable dt = dbconn.getTable(appServiceQry._searchARVOD(movieId));
                        if (dt.Rows.Count != 0)
                        {
                            response.isSuccess = true;
                            response.resultCode = HttpStatusCode.OK.ToString();
                            response.resultMessage = appConstantValues.MSG_SUCCESS;
                            response.contentNameEng = dt.Rows[0]["NAME_ENG"].ToString();
                            response.contentNameMon = dt.Rows[0]["NAME_MON"].ToString();
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
            LogWriter._noti(TAG, string.Format("[<<] IP: [{0}], Response: [{1}], Token: [{2}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), serializer.Serialize(response), token));
            return message;
        }
    }
}

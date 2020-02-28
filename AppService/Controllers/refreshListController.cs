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

namespace AppService.Controllers
{
    [RoutePrefix("api/refreshList")]
    public class refreshListController : ApiController
    {
        private string TAG = "refreshList";
        DBConnection dbconn = new DBConnection();
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private string dbres = string.Empty;
        /// <summary>
        /// Хэрэглэгчийн идэвхжүүлэх боломжтой үйлчилгээний жагсаалт авах сервис
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<refreshListMdl>))]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage message = null;
            refreshListMdl response = new refreshListMdl();
            LogWriter._other(TAG, "[>>] Request");
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        DataTable dtVod = dbconn.getTable(appServiceQry._getRefNvod(userCardNo));
                        DataTable dtProd = dbconn.getTable(appServiceQry._getRefProduct(userCardNo));
                        DataTable dtLive = dbconn.getTable(appServiceQry._getRefLive(userCardNo));
                        List<nvodActiveList> nvodList = new List<nvodActiveList>();
                        List<productActiveList> prodList = new List<productActiveList>();
                        List<liveActiveList> liveList = new List<liveActiveList>();
                        foreach(DataRow drVod in dtVod.Rows)
                        {
                            nvodActiveList nvod = new nvodActiveList();
                            nvod.contentName = drVod["NAME_MON"].ToString();
                            nvod.contentId = drVod["CONTENT_ID"].ToString();
                            nvod.productId = drVod["PRODUCT_ID"].ToString();
                            nvod.endTime = drVod["ENDTIME"].ToString();
                            nvodList.Add(nvod);
                        }
                        foreach(DataRow drProd in dtProd.Rows)
                        {
                            productActiveList prod = new productActiveList();
                            prod.productName = drProd["PRODUCT_NAME_MON"].ToString();
                            prod.productId = drProd["PRODUCT_ID"].ToString();
                            prod.endTime = drProd["ENDTIME"].ToString();
                            prodList.Add(prod);
                        }
                        foreach(DataRow drLive in dtLive.Rows)
                        {
                            liveActiveList live = new liveActiveList();
                            live.eventName = drLive["CONTENT_NAME"].ToString();
                            live.eventId = drLive["EVENT_ID"].ToString();
                            live.endTime = drLive["ENDTIME"].ToString();
                            liveList.Add(live);
                        }
                        response.isSuccess = true;
                        response.resultCode = HttpStatusCode.OK.ToString();
                        response.resultMessage = appConstantValues.MSG_SUCCESS;
                        response.livePosterUrl = "http://my.ddishtv.mn:808/refresh/ddish_live.png";
                        response.productPosterUrl = "http://my.ddishtv.mn:808/refresh/ddish_product.png";
                        response.nvodPosterUrl = string.Empty;
                        response.refreshDesc = "Та дахин идэвхжүүлэлт авахдаа хүлээн авагчаа заавал асаалттай байхыг анхаараарай.";
                        response.nvodList = nvodList;
                        response.productList = prodList;
                        response.liveList = liveList;
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
            LogWriter._other(TAG, string.Format("[<<] IP: [{0}], Response: [{1}], Token: [{2}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), serializer.Serialize(response), token));
            return message;
        }
    }
}

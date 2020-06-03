using AppServiceUtil.DBControl;
using AppServiceUtil.TabletModel;
using AppServiceUtil.Utils;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using AppServiceUtil.AppserviceModel;

namespace AppInstallerService.Controllers
{
    [RoutePrefix("v1/Refresh")]
    public class refreshServiceController : ApiController
    {
        private string TAG = "refreshServiceController";
        DBControlNew dbconn = new DBControlNew();
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// Дахин идэвхжүүлэлт хийх хэрэглэгч хайх сервис
        /// </summary>
        /// <param name="searchVal">Admin or CardNo</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{searchVal}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<refreshServiceResponse>))]
        public async Task<HttpResponseMessage> Get(string searchVal)
        {
            LogWriter._other(TAG, string.Format(@"[>>] Request: [{0}]", searchVal));
            HttpResponseMessage message = new HttpResponseMessage();
            refreshServiceResponse response = new refreshServiceResponse();
            string ip = httpUtil.GetClientIPAddress(HttpContext.Current.Request);
            string secToken = string.Empty;
            try
            {
                secToken = HttpContext.Current.Request.Headers["Authorization"].Replace("Basic ", "").Trim();
                if (dbconn.idbStatOK())
                {
                    string insId = string.Empty;
                    string insPhone = string.Empty;
                    if (dbconn.tabletCheckToken(secToken, out insId, out insPhone))
                    {
                        LogWriter._noti(TAG, string.Format(@"Request Token: [{0}], InstallerId: [{1}]", secToken, insId));
                        DataTable dt = dtS(searchVal);
                        if(dt.Rows.Count == 1)
                        {
                            string fName = dt.Rows[0]["SUBSCRIBER_FNAME"].ToString();
                            string cardNo = dt.Rows[0]["CARD_NO"].ToString();
                            string admin = dt.Rows[0]["PHONE_NO"].ToString();
                            DataTable dtP = dbconn.getTable(appServiceQry._getRefProduct(cardNo));
                            DataTable dtN = dbconn.getTable(appServiceQry._getRefNvod(cardNo));
                            DataTable dtL = dbconn.getTable(appServiceQry._getRefLive(cardNo));
                            List<Product> prodList = new List<Product>();
                            List<Vod> vodList = new List<Vod>();
                            List<Live> liveList = new List<Live>();
                            foreach(DataRow item in dtP.Rows)
                            {
                                Product prd = new Product();
                                prd.productName = item["PRODUCT_NAME_MON"].ToString();
                                prd.endDate = item["ENDTIME"].ToString();
                                prodList.Add(prd);
                            }
                            foreach(DataRow item in dtN.Rows)
                            {
                                Vod vod = new Vod();
                                vod.contentName = item["NAME_MON"].ToString();
                                vod.endDate = item["ENDTIME"].ToString();
                                vodList.Add(vod);
                            }
                            foreach(DataRow item in dtL.Rows)
                            {
                                Live live = new Live();
                                live.concertName = item["CONTENT_NAME"].ToString();
                                live.endDate = item["ENDTIME"].ToString();
                                liveList.Add(live);
                            }
                            response.isSuccess = true;
                            response.errorCode = Convert.ToString((int)HttpStatusCode.OK);
                            response.resultMessage = "success";
                            response.firstName = fName;
                            response.cardNo = cardNo;
                            response.adminNo = admin;
                            response.products = prodList;
                            response.vods = vodList;
                            response.lives = liveList;
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
            LogWriter._other(TAG, string.Format(@"[<<] ClientIp: [{0}], Response: [{1}]", ip, serializer.Serialize(response)));
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            return message;
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
            catch(Exception ex)
            {
                LogWriter._error(TAG, string.Format("Input: [{0}], Exception: [{1}]", inS, ex.ToString()));
                dt.Clear();
            }
            return dt;
        }

        /// <summary>
        /// Хэрэглэгчийн үйлчилгээг дахин илгээх сервис
        /// </summary>
        /// <param name="cardNo">Хэрэглэгчийн картын дугаар</param>
        /// <param name="adminNo">Хэрэглэгчийн админ дугаар</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{cardNo}/{adminNo}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<DefaultResponse>))]
        public async Task<HttpResponseMessage> GetRefreshProcess(string cardNo, string adminNo)
        {
            LogWriter._other(TAG, string.Format(@"[>>] Request: [{0}, {1}]", cardNo, adminNo));
            HttpResponseMessage message = new HttpResponseMessage();
            DefaultResponse response = new DefaultResponse();
            string ip = httpUtil.GetClientIPAddress(HttpContext.Current.Request);
            string secToken = string.Empty;
            try
            {
                secToken = HttpContext.Current.Request.Headers["Authorization"].Replace("Basic ", "").Trim();
                if (dbconn.idbStatOK())
                {
                    string insId = string.Empty;
                    string insPhone = string.Empty;
                    if (dbconn.tabletCheckToken(secToken, out insId, out insPhone))
                    {
                        if (dbconn.refreshService(cardNo, insPhone))
                        {
                            response.isSuccess = true;
                            response.errorCode = Convert.ToString((int)HttpStatusCode.OK);
                            response.resultMessage = "success";
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.errorCode = Convert.ToString((int)HttpStatusCode.InternalServerError);
                            response.resultMessage = "Internal Error";
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
            LogWriter._other(TAG, string.Format(@"[<<] ClientIp: [{0}], Response: [{1}]", ip, serializer.Serialize(response)));
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            return message;
        }
    }
}

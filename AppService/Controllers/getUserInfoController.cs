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
    [RoutePrefix("api/getUserInfo")]
    public class getUserInfoController : ApiController
    {
        private string TAG = "getUserInfo";
        DBConnection dbconn = new DBConnection();
        JavaScriptSerializer serialzer = new JavaScriptSerializer();
        private string dbres = string.Empty;
        /// <summary>
        /// Хэрэглэгчийн мэдээлэл
        /// </summary>
        /// <remarks>activeProducts - Хэрэглэгч дээр идэвхтэй байгаа Зөвхөн үндсэн багцын мэдээлэл,
        /// additionalProducts - Хэрэглэгч дээр идэвхтэй байгаа нэмэлт сувгийн багцын мэдээлэл,
        /// activeCounters - Хэрэглэгч дээр идэвхтэй байгаа урамшууллын дансны мэдээлэл байна.
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<getUserInfoResponse>))]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage message = null;
            getUserInfoResponse response = new getUserInfoResponse();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            try
            {
                //string userCardNo = string.Empty;
                if(dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        DataTable dt = dbconn.getTable(appServiceQry._getUserInfoByCardNo(userCardNo));
                        if (dt.Rows.Count != 0)
                        {
                            response.userFirstName = dt.Rows[0]["SUBSCRIBER_FNAME"].ToString();
                            response.userLastName = dt.Rows[0]["SUBSCRIBER_LNAME"].ToString();
                            response.userRegNo = dt.Rows[0]["CERTIFICATE_NO"].ToString();
                            response.adminNumber = dt.Rows[0]["PHONE_NO"].ToString();
                            string cardno = dt.Rows[0]["CARD_NO"].ToString();
                            response.cardNo = cardno;
                            List<Products> prodList = new List<Products>();
                            List<Products> addProdList = new List<Products>();
                            List<Counters> counterList = new List<Counters>();
                            DataTable dtProd = dbconn.getTable(appServiceQry._getProducts(cardno));
                            foreach (DataRow item in dtProd.Rows)
                            {
                                Products prodObj = new Products();
                                string prodName = item["PRODUCT_NAME_MON"].ToString();
                                string prodId = item["PRODUCT_ID"].ToString();
                                string endDate = item["ENDDATE"].ToString();
                                string orderNo = item["ORDERING"].ToString();
                                prodObj.productName = prodName;
                                prodObj.productId = prodId;
                                prodObj.endDate = endDate;
                                prodObj.orderingNo = orderNo;
                                bool ismain = false;
                                switch(prodId)
                                {
                                    case "28":
                                        ismain = true;
                                        break;
                                    case "27":
                                        ismain = true;
                                        break;
                                    case "29":
                                        ismain = true;
                                        break;
                                    case "73":
                                        ismain = true;
                                        break;
                                    default:
                                        ismain = false;
                                        break;
                                }
                                prodObj.isMain = ismain;
                                if(ismain)
                                {
                                    prodList.Add(prodObj);
                                }
                                else
                                {
                                    addProdList.Add(prodObj);
                                }
                            }
                            DataTable dtCounter = dbconn.getTable(appServiceQry._getPromoCounters(cardno));
                            foreach (DataRow dr in dtCounter.Rows)
                            {
                                Counters cntr = new Counters();
                                string cName = dr["NAME"].ToString();
                                string cId = dr["COUNTER_ID"].ToString();
                                string balance = dr["COUNTER_AMOUNT"].ToString();
                                string expDate = dr["EXPIREDATE"].ToString();
                                string unit = dr["MEASUREUNIT"].ToString();
                                cntr.counterName = cName;
                                cntr.counterBalance = balance;
                                cntr.countId = cId;
                                cntr.counterMeasureUnit = unit;
                                cntr.counterExpireDate = expDate;
                                bool ismain = false;
                                if(cId == "1001")
                                {
                                    ismain = true;
                                }
                                cntr.isMain = ismain;
                                counterList.Add(cntr);
                            }
                            response.isSuccess = true;
                            response.resultCode = HttpStatusCode.OK.ToString();
                            response.resultMessage = "success";
                            response.activeProducts = prodList;
                            response.additionalProducts = addProdList;
                            response.activeCounters = counterList;
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = "Хэрэглэгч олдсонгүй.";
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
                    response.resultMessage = dbres;
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
            LogWriter._userInfo(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), "", serialzer.Serialize(response), token));
            return message;
        }

        /// <summary>
        /// Үндсэн дансны мэдээлэл харах
        /// </summary>
        /// <param name="counter">Ямар утга өгч болно</param>
        /// <remarks>Зөвхөн Үндсэн дансны мэдээлэл буцаана.</remarks>
        /// <returns></returns>
        [HttpGet]
        [Route("{counter}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<mainAccount>))]
        public HttpResponseMessage Get(string counter)
        {
            HttpResponseMessage message = null;
            mainAccount response = new mainAccount();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        DataTable dtCounter = dbconn.getTable(appServiceQry._getMainCounters(userCardNo));
                        if(dtCounter.Rows.Count !=0)
                        {
                            response.isSuccess = true;
                            response.resultCode = HttpStatusCode.OK.ToString();
                            response.resultMessage = "success";
                            Counters cntr = new Counters();
                            cntr.countId = dtCounter.Rows[0]["COUNTER_ID"].ToString();
                            cntr.counterName = dtCounter.Rows[0]["NAME"].ToString();
                            cntr.counterBalance = dtCounter.Rows[0]["COUNTER_AMOUNT"].ToString();
                            cntr.counterMeasureUnit = dtCounter.Rows[0]["MEASUREUNIT"].ToString();//
                            cntr.counterExpireDate = dtCounter.Rows[0]["EXPIREDATE"].ToString();
                            response.mainCounter = cntr;
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.Unauthorized.ToString();
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
                    response.resultMessage = dbres;
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
            LogWriter._userInfo(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), counter, serialzer.Serialize(response), token));
            return message;
        }
    }
}

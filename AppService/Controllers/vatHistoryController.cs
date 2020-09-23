using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AppService.Models;
using AppServiceUtil.AppserviceModel;
using AppServiceUtil.Auth;
using AppServiceUtil.DBControl;
using AppServiceUtil.Utils;
using Swashbuckle.Swagger.Annotations;
using System.Web.Script.Serialization;
using System.Web;
using System.Data;

namespace AppService.Controllers
{
    [RoutePrefix("api/vatHistory")]
    public class vatHistoryController : ApiController
    {
        DBConnection dbconn = new DBConnection();
        private string TAG = "notification";
        private string dbres = string.Empty;
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// Хэрэглэгч дээр хэвлэгдсэн ebarimt-н түүх авах сервис
        /// </summary>
        /// <param name="uriValue"> default is 0</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{uriValue}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<vatHistoryModel>))]
        public HttpResponseMessage Get([FromUri] PagingParameterModel paging, string uriValue)
        {
            HttpResponseMessage message = null;
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            vatHistoryModel response = new vatHistoryModel();
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        DataTable dt = dbconn.getTable(appServiceQry.vatHistoryList(userCardNo));
                        int count = dt.Rows.Count;
                        List<vatList> vats = new List<vatList>();
                        if (count != 0)
                        {
                            int CurrentPage = paging.pageNumber;
                            int PageSize = paging.pageSize;
                            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

                            
                            foreach (DataRow dr in dt.Rows)
                            {
                                vatList vat = new vatList();
                                vat.cardNo = dr["CARD_NO"].ToString();
                                vat.amount = dr["AMOUNT"].ToString();
                                vat.billId = dr["BILLID"].ToString();
                                vat.qrData = dr["QRDATA"].ToString();
                                vat.billDate = dr["RESULTDATE"].ToString();
                                vat.createUser = dr["USERCODE"].ToString();
                                vat.lotteryNo = dr["LOTTERYNO"].ToString();
                                vats.Add(vat);
                            }

                            var items = vats.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                            var previousPage = CurrentPage > 1 ? "Yes" : "No";
                            var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

                            var paginationMetadata = new
                            {
                                totalCount = count,
                                pageSize = PageSize,
                                currentPage = CurrentPage,
                                totalPages = TotalPages,
                                previousPage,
                                nextPage
                            };
                            HttpContext.Current.Response.Headers.Add("X-Paging-Headers", serializer.Serialize(paginationMetadata));
                            response.isSuccess = true;
                            response.resultCode = HttpStatusCode.OK.ToString();
                            response.resultMessage = appConstantValues.MSG_SUCCESS;
                            response.vats = items;
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = appConstantValues.MSG_NOFOUND;
                            response.vats = vats;
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
                //exceptionManager.ManageException(ex, TAG);
                LogWriter._error(TAG, ex.ToString());
                response.isSuccess = false;
                response.resultCode = HttpStatusCode.NotFound.ToString();
                response.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._noti(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), "", serializer.Serialize(response), token));
            return message;
        }
    }
}

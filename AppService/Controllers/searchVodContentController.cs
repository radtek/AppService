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
    [RoutePrefix("api/searchVodContent")]
    public class searchVodContentController : ApiController
    {
        private string TAG = "vodList";
        DBConnection dbconn = new DBConnection();
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private string dbres = string.Empty;

        /// <summary>
        /// Идэвхтэй хөтөлбөрийн жагсаалтаас контент хайх service
        /// </summary>
        /// <param name="searchValue">Хайх контентын англи болон монгол нэр (mandatory)</param>
        /// <param name="res">default value is 0</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{searchValue}/{res}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<vodProgramModel>))]
        public HttpResponseMessage Get([FromUri]PagingParameterModel vodPagingModel, string searchValue, string res)
        {
            HttpResponseMessage message = null;
            vodProgramModel program = new vodProgramModel();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        string _begin = DateTime.Now.ToString(appConstantValues.DATE_FORMAT_LONG);
                        DataTable dt = dbconn.getTable(appServiceQry._vodProgramSearch(_begin, searchValue));
                        int count = dt.Rows.Count;
                        if (count != 0)
                        {
                            int CurrentPage = vodPagingModel.pageNumber;
                            int PageSize = vodPagingModel.pageSize;
                            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                            

                            List<programDetial> prmList = new List<programDetial>();
                            //prmList.Skip
//dt
                            foreach (DataRow dr in dt.Rows)
                            {
                                programDetial prmDetial = new programDetial();
                                prmDetial.productId = dr["PRODUCT_ID"].ToString();
                                prmDetial.smsCode = dr["SMS_CODE"].ToString();
                                prmDetial.contentId = dr["ID"].ToString();
                                prmDetial.contentNameMon = dr["NAME"].ToString();
                                prmDetial.contentNameEng = dr["NAME_ENGLISH"].ToString();
                                DataTable dtGen = dbconn.getTable(appServiceQry._getGenres(dr["ID"].ToString()));
                                if (dtGen.Rows.Count != 0)
                                {
                                    prmDetial.contentGenres = dtGen.Rows[0]["GENRES"].ToString();
                                }
                                else
                                {
                                    prmDetial.contentGenres = String.Empty;
                                }
                                prmDetial.inDate = dr["INDATE"].ToString();
                                prmDetial.beginDate = dr["BEGINDATE"].ToString();
                                prmDetial.endDate = dr["ENDDATE"].ToString();
                                prmDetial.posterUrl = dr["IMAGE_WEB"].ToString().Replace("192.168.10.93", "my.ddishtv.mn:808");
                                prmDetial.contentPrice = dr["PRICE"].ToString();
                                prmList.Add(prmDetial);
                            }

                            var items = prmList.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
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
                            HttpContext.Current.Response.Headers.Add("X-Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
                            program.isSuccess = true;
                            program.resultCode = HttpStatusCode.OK.ToString();
                            program.resultMessage = appConstantValues.MSG_SUCCESS;
                            program.programList = items;
                        }
                        else
                        {
                            program.isSuccess = false;
                            program.resultCode = HttpStatusCode.NotFound.ToString();
                            program.resultMessage = appConstantValues.MSG_NOFOUND;
                        }
                    }
                    else
                    {
                        program.isSuccess = false;
                        program.resultCode = HttpStatusCode.Unauthorized.ToString();
                        program.resultMessage = appConstantValues.MSG_EXPIRED;
                    }
                }
                else
                {
                    program.isSuccess = false;
                    program.resultCode = HttpStatusCode.NotFound.ToString();
                    program.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
                }
            }
            catch (Exception ex)
            {
                exceptionManager.ManageException(ex, TAG);
                program.isSuccess = false;
                program.resultCode = HttpStatusCode.NotFound.ToString();
                program.resultMessage = ex.Message;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, program);
            LogWriter._vodList(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), searchValue, serializer.Serialize(program), token));
            return message;
        }
    }
}

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
    [RoutePrefix("api/vodListWeb")]
    public class vodListWebController : ApiController
    {
        private string TAG = "vodListWeb";
        DBConnection dbconn = new DBConnection();
        JavaScriptSerializer serialzer = new JavaScriptSerializer();
        private string dbres = string.Empty;

        /// <summary>
        /// VOD сувгийн жагсаалт харах
        /// </summary>
        /// <remarks>VOD сувгийн мэдээлэл буцаана.</remarks>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<vodListModel>))]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage message = null;
            vodListModel vod = new vodListModel();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (token == "YGHM9SHBC81LMR4G")
                    {
                        DataTable dt = dbconn.getTable(appServiceQry._vodChannelList());
                        if (dt.Rows.Count != 0)
                        {
                            List<vodDetial> vodDet = new List<vodDetial>();
                            foreach (DataRow item in dt.Rows)
                            {
                                vodDetial cdetial = new vodDetial();
                                cdetial.productName = item["PRODUCT_NAME_MON"].ToString();
                                cdetial.productId = item["PRODUCT_ID"].ToString();
                                cdetial.channelNo = item["TV_CHANNEL"].ToString();
                                cdetial.channelLogo = httpUtil.getProductLogoUrl(item["PRODUCT_ID"].ToString());
                                vodDet.Add(cdetial);
                            }
                            vod.isSuccess = true;
                            vod.resultCode = HttpStatusCode.OK.ToString();
                            vod.resultMessage = appConstantValues.MSG_SUCCESS;
                            vod.vodChannels = vodDet;
                        }
                        else
                        {
                            vod.isSuccess = false;
                            vod.resultCode = HttpStatusCode.NotFound.ToString();
                            vod.resultMessage = appConstantValues.MSG_NOFOUND;
                        }
                    }
                    else
                    {
                        vod.isSuccess = false;
                        vod.resultCode = HttpStatusCode.Unauthorized.ToString();
                        vod.resultMessage = appConstantValues.MSG_EXPIRED;
                    }
                }
                else
                {
                    vod.isSuccess = false;
                    vod.resultCode = HttpStatusCode.NotFound.ToString();
                    vod.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
                }
            }
            catch (Exception ex)
            {
                exceptionManager.ManageException(ex, TAG);
                vod.isSuccess = false;
                vod.resultCode = HttpStatusCode.NotFound.ToString();
                vod.resultMessage = ex.Message;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, vod);
            LogWriter._vodList(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), "", serialzer.Serialize(vod), token));
            return message;
        }
        /// <summary>
        /// Сонгосон сувгийн хөтөлбөрийн мэдээлэл
        /// </summary>
        /// <remarks>Сонгосон сувгийн хөтөлбөрийг productId болон inDate(хөтөлбөр харах өдөр)-р харах боломжтой.</remarks>
        /// <param name="productId">Сонгосон сувгийн productId (mandatory)</param>
        /// <param name="indate">Хөтөлбөрийг зөвхөн нэг нэг өдрөөр харна. Date format - yyyyMMdd (mandatory)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{productId}/{indate}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<vodProgramModel>))]
        public HttpResponseMessage Get(string productId, string indate)
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
                    if (token == "YGHM9SHBC81LMR4G")
                    {
                        DataTable dt = dbconn.getTable(appServiceQry._vodProgramList(indate, productId));
                        if (dt.Rows.Count != 0)
                        {
                            List<programDetial> prmList = new List<programDetial>();
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
                                string price = "0";
                                switch (productId)
                                {
                                    case "17":
                                        price = "3000";
                                        break;
                                    case "25":
                                        price = dr["PRICE"].ToString();
                                        break;
                                    case "18":
                                        price = "2000";
                                        break;
                                    case "19":
                                        price = "1000";
                                        break;
                                    default:
                                        price = "0";
                                        break;
                                }
                                prmDetial.contentPrice = price;
                                prmList.Add(prmDetial);
                            }
                            program.isSuccess = true;
                            program.resultCode = HttpStatusCode.OK.ToString();
                            program.resultMessage = appConstantValues.MSG_SUCCESS;
                            program.programList = prmList;
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
            LogWriter._vodList(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), productId, serialzer.Serialize(program), token));
            return message;
        }
        /// <summary>
        /// Сонгосон контентын дэлгэрэнгүй мэдээлэл.
        /// </summary>
        /// <remarks>Сонгосон контентын дэлгэрэнгүй мэдээлэл.</remarks>
        /// <param name="contentId">Сонгосон контентийн ID (mandatory)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{contentId}")]
        //[SwaggerOperation(Tags = new[] { "ContentDetial" })]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<contentDetialModel>))]
        public HttpResponseMessage Get_ContentDet(string contentId)
        {
            HttpResponseMessage message = null;
            contentDetialModel cont = new contentDetialModel();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (token == "YGHM9SHBC81LMR4G")
                    {
                        DataTable dt = dbconn.getTable(appServiceQry._getContentDetial(contentId));
                        if (dt.Rows.Count != 0)
                        {
                            string contId = dt.Rows[0]["ID"].ToString();
                            string contNameMon = dt.Rows[0]["NAME_MON"].ToString();
                            string contNameEng = dt.Rows[0]["NAME_ENG"].ToString();
                            string contPrice = dt.Rows[0]["PRICE"].ToString();
                            string contDesc = dt.Rows[0]["CONTENT_DESCR"].ToString();
                            string contPoster = dt.Rows[0]["IMAGE_WEB"].ToString();
                            string contYear = dt.Rows[0]["YEAR"].ToString();
                            string contTrailer = dt.Rows[0]["TRAILER"].ToString();
                            string contDuration = dt.Rows[0]["DURATION_TIME"].ToString();
                            string contDirector = dt.Rows[0]["DIRECTOR"].ToString();
                            string contActor = string.Empty;
                            string contGenre = string.Empty;
                            DataTable dt_actor = dbconn.getTable(appServiceQry._getActors(contentId));
                            if (dt_actor.Rows.Count != 0)
                            {
                                contActor = dt_actor.Rows[0]["ACTORS"].ToString();
                            }
                            DataTable dtGen = dbconn.getTable(appServiceQry._getGenres(contId));
                            contGenre = dtGen.Rows[0]["GENRES"].ToString();
                            bool isOrdered = false;
                            // = to obj
                            cont.isSuccess = true;
                            cont.resultCode = HttpStatusCode.OK.ToString();
                            cont.resultMessage = appConstantValues.MSG_SUCCESS;
                            cont.contentId = contId;
                            cont.contentNameMon = contNameMon;
                            cont.contentNameEng = contNameEng;
                            cont.contentGenres = contGenre;
                            cont.contentPrice = contPrice;
                            cont.contentDescr = contDesc;
                            cont.posterUrl = contPoster.Replace("192.168.10.93", "my.ddishtv.mn:808");
                            cont.contentYear = contYear;
                            cont.contentDuration = contDuration;
                            cont.trailerUrl = contTrailer;
                            cont.directors = contDirector;
                            cont.actors = contActor;
                            cont.isOrdered = isOrdered;
                        }
                        else
                        {
                            cont.isSuccess = false;
                            cont.resultCode = HttpStatusCode.NotFound.ToString();
                            cont.resultMessage = appConstantValues.MSG_NOFOUND;
                        }
                    }
                    else
                    {
                        cont.isSuccess = false;
                        cont.resultCode = HttpStatusCode.Unauthorized.ToString();
                        cont.resultMessage = appConstantValues.MSG_EXPIRED;
                    }
                }
                else
                {
                    cont.isSuccess = false;
                    cont.resultCode = HttpStatusCode.NotFound.ToString();
                    cont.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
                }
            }
            catch (Exception ex)
            {
                exceptionManager.ManageException(ex, TAG);
                cont.isSuccess = false;
                cont.resultCode = HttpStatusCode.NotFound.ToString();
                cont.resultMessage = ex.Message;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, cont);
            LogWriter._vodList(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), contentId, serialzer.Serialize(cont), token));
            return message;
        }
    }
}

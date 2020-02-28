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
    [RoutePrefix("api/getSalesCenter")]
    public class getSalesCenterController : ApiController
    {
        DBConnection dbconn = new DBConnection();
        private string TAG = "getSalesCenter";
        private string dbres = string.Empty;
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// Салбарын үйлчилгээ, төрөл, бүсийн мэдээлэл авах
        /// </summary>
        /// <remarks>branchAreas - Салбарын бүсийн мэдээлэл, branchServices - Үйлчилгээний мэдээлэл, branchTypes - Салбарын төрөл</remarks>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<salesCenterType>))]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage message = null;
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            salesCenterType slstype = new salesCenterType();
            try
            {
                if (token == "YGHM9SHBC81LMR4G")
                {
                    if (dbconn.idbCheck(out dbres))
                    {
                        DataTable dt_area = dbconn.getTable(appServiceQry.getSalesCenterAre());
                        DataTable dt_service = dbconn.getTable(appServiceQry.getSalesCenterService());
                        DataTable dt_type = dbconn.getTable(appServiceQry.getSalesCenterType());
                        List<Area> area = new List<Area>();
                        List<Service> service = new List<Service>();
                        List<BranchType> type = new List<BranchType>();
                        foreach (DataRow item in dt_area.Rows)
                        {
                            Area arr = new Area();
                            arr.areaName = item["AREA_NAME"].ToString();
                            arr.areaCode = item["AREA_CODE"].ToString();
                            area.Add(arr);
                        }
                        foreach (DataRow dr in dt_service.Rows)
                        {
                            Service serv = new Service();
                            serv.serviceName = dr["SERVICE_NAME"].ToString();
                            serv.serviceCode = dr["SERVICE_CODE"].ToString();
                            service.Add(serv);
                        }
                        foreach (DataRow df in dt_type.Rows)
                        {
                            BranchType tpe = new BranchType();
                            tpe.typeName = df["TYPE_NAME"].ToString();
                            tpe.typeCode = df["TYPE_CODE"].ToString();
                            type.Add(tpe);
                        }
                        slstype.isSuccess = true;
                        slstype.resultCode = HttpStatusCode.OK.ToString();
                        slstype.resultMessage = "success";
                        slstype.branchAreas = area;
                        slstype.branchServices = service;
                        slstype.branchTypes = type;
                    }
                    else
                    {
                        slstype.isSuccess = false;
                        slstype.resultCode = HttpStatusCode.NotFound.ToString();
                        slstype.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
                        LogWriter._error(TAG, dbres);
                    }
                }
                else
                {
                    slstype.isSuccess = false;
                    slstype.resultCode = HttpStatusCode.Unauthorized.ToString();
                    slstype.resultMessage = appConstantValues.MSG_EXPIRED;
                }
            }
            catch (Exception ex)
            {
                exceptionManager.ManageException(ex, TAG);
                slstype.isSuccess = false;
                slstype.resultCode = HttpStatusCode.NotFound.ToString();
                slstype.resultMessage = ex.Message;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, slstype);
            LogWriter._getBranches(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), "", serializer.Serialize(slstype), token));
            return message;
        }


        /// <summary>
        /// Салбарын мэдээлэл харах
        /// </summary>
        /// <param name="area">Бүсийн код areCode (mandatory)</param>
        /// <param name="branchType">Салбарын төрөл typeCode (mandatory)</param>
        /// <param name="service">Үйлчилгээ serivceCode (mandatory)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{area}/{branchType}/{service}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<getSalesCenterModel>))]
        public HttpResponseMessage Get(string area, string branchType, string service)
        {
            string parameters = string.Format(@"Area:[{0}], BranchType:[{1}], Service:[{2}]", area, branchType, service);
            LogWriter._getBranches(TAG, string.Format("[>>] REQUEST: [{0}]", parameters));
            HttpResponseMessage message = null;
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            getSalesCenterModel response = new getSalesCenterModel();
            DateTime date;
            try
            {
                if (token == "YGHM9SHBC81LMR4G")
                {
                    if (dbconn.idbCheck(out dbres))
                    {
                        List<salesCenterDetial> scList = new List<salesCenterDetial>();
                        DataTable dt = dbconn.getTable(appServiceQry._getSalesCenters(area, service, branchType));
                        foreach (DataRow dr in dt.Rows)
                        {
                            salesCenterDetial scenter = new salesCenterDetial();
                            scenter.branchId = dr["BRANCH_ID"].ToString();
                            scenter.branchName = dr["BRANCH_NAME"].ToString();
                            scenter.address = dr["ADDRESS"].ToString();
                            scenter.type = dr["BRANCH_TYPE"].ToString();
                            scenter.latitude = dr["LATITUDE"].ToString();
                            scenter.longtitue = dr["LONGTITUDE"].ToString();
                            scenter.img = dr["IMAGE"].ToString();
                            scenter.zoom = dr["ZOOM"].ToString();
                            string fullwork = dr["FULLWORKING_DAYS"].ToString();
                            string fullopentime = dr["FULLWORKING_OPENTIME"].ToString();
                            string fullclosetime = dr["FULLWORKING_CLOSETIME"].ToString();
                            string halfwork = dr["HALFWORKING_DAYS"].ToString();
                            string halfopentime = dr["HALFWORKING_OPENTIME"].ToString();
                            string halfclosetime = dr["HALFWORKING_CLOSETIME"].ToString();
                            string door = string.Empty;
                            date = DateTime.Parse(DateTime.Today.ToString(appConstantValues.DATE_FORMAT), CultureInfo.InvariantCulture);
                            string timeTable = string.Empty;
                            string timeTableHalf = string.Empty;
                            string timeTableFull = string.Empty;
                            string nowDayNumber = ((int)date.DayOfWeek).ToString();
                            if (fullwork.Contains(nowDayNumber))
                            {
                                if (Convert.ToInt32(fullopentime.Replace(":", "")) < Convert.ToInt32(DateTime.Now.ToString(appConstantValues.TIME_SHORT_LONG)) && Convert.ToInt32(fullclosetime.Replace(":", "")) > Convert.ToInt32(DateTime.Now.ToString(appConstantValues.TIME_SHORT_LONG)))
                                {
                                    door = "Нээлттэй";
                                }
                                else
                                {
                                    door = "Хаалттай";
                                }
                            }
                            else
                            {
                                if (halfwork.Length != 0)
                                {
                                    if (Convert.ToInt32(halfopentime.Replace(":", "")) < Convert.ToInt32(DateTime.Now.ToString(appConstantValues.TIME_SHORT_LONG)) && Convert.ToInt32(halfclosetime.Replace(":", "")) > Convert.ToInt32(DateTime.Now.ToString(appConstantValues.TIME_SHORT_LONG)))
                                    {
                                        door = "Нээлттэй";
                                    }
                                    else
                                    {
                                        door = "Хаалттай";
                                    }

                                }
                            }
                            string orgMonDay = fullwork.Replace("1", "Да").Replace("2", "Мя").Replace("3", "Лх").Replace("4", "Пү").Replace("5", "Ба").Replace("6", "Бя").Replace("7", "Ня");
                            timeTableFull = string.Format(@"{0}@{1} - {2}", orgMonDay, fullopentime, fullclosetime);
                            if (halfwork.Length != 0)
                            {
                                string orghalfMonDay = halfwork.Replace("1", "Да").Replace("2", "Мя").Replace("3", "Лх").Replace("4", "Пү").Replace("5", "Ба").Replace("6", "Бя").Replace("7", "Ня");
                                timeTableHalf = string.Format("{0}@{1} - {2}", orghalfMonDay, halfopentime, halfclosetime);
                            }
                            if (timeTableHalf.Length != 0)
                            {
                                timeTable = string.Format("{0} &&& {1}", timeTableFull, timeTableHalf);
                            }
                            else
                            {
                                timeTable = timeTableFull;
                            }
                            scenter.timeTable = timeTable;
                            scenter.door = door;
                            scList.Add(scenter);
                        }
                        response.isSuccess = true;
                        response.resultCode = HttpStatusCode.OK.ToString();
                        response.resultMessage = "success";
                        response.salesCenters = scList;
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
                    response.resultCode = HttpStatusCode.Unauthorized.ToString();
                    response.resultMessage = appConstantValues.MSG_EXPIRED;
                }
            }
            catch (Exception ex)
            {
                exceptionManager.ManageException(ex, TAG);
                response.isSuccess = false;
                response.resultCode = HttpStatusCode.NotFound.ToString();
                response.resultMessage = ex.Message;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._getBranches(TAG, string.Format("IP: [{0}], Response: [{1}], Token: [{2}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), serializer.Serialize(response), token));
            return message;
        }
    }
}

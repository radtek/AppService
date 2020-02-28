﻿using System;
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
    [RoutePrefix("api/upgradeProductNew")]
    public class upgradeProductNewController : ApiController
    {
        private string TAG = "upgradeProduct";
        DBConnection dbconn = new DBConnection();
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private string dbres = string.Empty;

        /// <summary>
        /// [Багц ахиулалтын шинэ нөхцөл] Ахиулах боломжтой багцын жагсаалт болон үнийн дүн, хоног авах сервис. Хуучин багц ахиулалтаас App-н UI талд харагдах байдал нь өөр болсон байгаа.
        /// </summary>
        /// <param name="productId">Одоо идэвхтэй байгаа багцын ID (mandatory)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{productId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<upNewProd>))]
        public HttpResponseMessage Get(string productId)
        {
            LogWriter._upgradeProduct(TAG, string.Format("[>>]  Request Product: [{0}]", productId));
            HttpResponseMessage message = null;
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            upNewProd response = new upNewProd();
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        DataTable dtCh = dbconn.getTable(appServiceQry._getMainProducts(productId));
                        if (dtCh.Rows.Count != 0)
                        {
                            DataTable dtUp = dbconn.getTable(appServiceQry._getMainProducts(dtCh.Rows[0]["UPGRADE_PRODUCTS"].ToString()));
                            List<upProdDetial> upProds = new List<upProdDetial>();
                            foreach (DataRow dr in dtUp.Rows)
                            {
                                upProdDetial upch = new upProdDetial();
                                upch.productName = dr["PRODUCT_NAME_MON"].ToString();
                                string upNewProdId = dr["PRODUCT_ID"].ToString();
                                upch.productId = upNewProdId;
                                upch.smsCode = dr["SMS_CODE"].ToString();
                                upch.productImg = httpUtil.getProductLogoUrl(upNewProdId);
                                string localResponse = string.Empty;
                                localCheckProduct api = new localCheckProduct();
                                api.cardNo = userCardNo;
                                api.Pay_type = "0";
                                api.ConvertProduct = upNewProdId;
                                string reqJsonStr = serializer.Serialize(api);
                                if (httpUtil.httpCall_POST_local(reqJsonStr, "checkProduct", out localResponse))
                                {
                                    LogWriter._upgradeProduct(TAG, string.Format("Local Service Response: [{0}]", localResponse));
                                    var localStruct = serializer.Deserialize<localCheckProductResponse>(localResponse);
                                    if (localStruct.issuccess)
                                    {
                                        upch.amount = localStruct.pamount;
                                        upch.convertDay = localStruct.pday;
                                        upch.endDate = localStruct.penddate;
                                        upProds.Add(upch);
                                    }
                                    //else
                                    //{
                                    //    throw new Exception("Ахиулах багцын мэдээлэл хүлээн авахад алдаа гарлаа.");
                                    //}
                                }
                                else
                                {
                                    throw new Exception("Дотоод сервис дуудахад алдаа гарлаа.");
                                }
                            }
                            response.isSuccess = true;
                            response.resultCode = HttpStatusCode.OK.ToString();
                            response.resultMessage = appConstantValues.MSG_SUCCESS;
                            response.upProducts = upProds;

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
            catch(Exception ex)
            {
                exceptionManager.ManageException(ex, TAG);
                response.isSuccess = false;
                response.resultCode = HttpStatusCode.NotFound.ToString();
                response.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._upgradeProduct(TAG, string.Format("[<<] IP: [{0}], Response: [{1}], Token: [{2}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), serializer.Serialize(response), token));
            return message;
        }

        /// <summary>
        /// [Багц ахиулалтын шинэ нөхцөл] Сонгосон багц руу ахиулах сервис.
        /// </summary>
        /// <param name="toProductId">Ахиулах багцын ID (mandatory)</param>
        /// <param name="accept">default value is yes</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{toProductId}/{accept}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModel>))]
        public HttpResponseMessage GetUpgrade(string toProductId, string accept)
        {
            LogWriter._upgradeProduct(TAG, string.Format("[>>]  Request toProduct: [{0}], Accept: [{1}]", toProductId, accept));
            HttpResponseMessage message = null;
            defaultResponseModel response = new defaultResponseModel();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        localConvertProdcutMdl convProd = new localConvertProdcutMdl();
                        convProd.BranchId = "286";
                        convProd.Channel = "6";
                        convProd.cardNo = userCardNo;
                        convProd.ConvertProduct = toProductId;
                        convProd.Pay_type = "1";
                        convProd.Username = userAdminNo;
                        string convJson = serializer.Serialize(convProd);
                        string localResponse = string.Empty;
                        if (httpUtil.httpCall_POST_local(convJson, "processProduct", out localResponse))
                        {
                            LogWriter._upgradeProduct(TAG, string.Format("Local Service Response: [{0}]", localResponse));
                            var convObj = serializer.Deserialize<localCheckProductResponse>(localResponse);
                            if (convObj.issuccess)
                            {
                                response.isSuccess = true;
                                response.resultCode = HttpStatusCode.OK.ToString();
                                response.resultMessage = appConstantValues.MSG_SUCCESS;
                            }
                            else
                            {
                                response.isSuccess = false;
                                response.resultCode = HttpStatusCode.NotFound.ToString();
                                response.resultMessage = "Багц ахиулалт амжилтгүй боллоо.";
                            }
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = "Дотоод сервис дуудахад алдаа гарлаа.";
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
            catch(Exception ex)
            {
                response.isSuccess = false;
                response.resultCode = HttpStatusCode.NotFound.ToString();
                response.resultMessage = ex.Message;
                exceptionManager.ManageException(ex, TAG);
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._upgradeProduct(TAG, string.Format("[<<] IP: [{0}], Response: [{1}], Token: [{2}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), serializer.Serialize(response), token));
            return message;
        }
    }
}

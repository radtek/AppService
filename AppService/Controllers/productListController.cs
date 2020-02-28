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
using Swashbuckle.Examples;

namespace AppService.Controllers
{
    [RoutePrefix("api/productList")]
    public class productListController : ApiController
    {
        private string TAG = "productList";
        DBConnection dbconn = new DBConnection();
        JavaScriptSerializer serialzer = new JavaScriptSerializer();
        private string dbres = string.Empty;

        /// <summary>
        /// Үндсэн багцуудын мэдээлэл
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<productListModel>))]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage message = null;
            productListModel prodMdl = new productListModel();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        DataTable dt = dbconn.getTable(appServiceQry._getMainProducts("27,28,29,73"));
                        if(dt.Rows.Count !=0)
                        {
                            List<productDetial> prdList = new List<productDetial>();
                            foreach (DataRow item in dt.Rows)
                            {
                                productDetial prodDetial = new productDetial();
                                prodDetial.productName = item["PRODUCT_NAME_MON"].ToString();
                                prodDetial.productId = item["PRODUCT_ID"].ToString();
                                prodDetial.smsCode = item["SMS_CODE"].ToString();
                                prodDetial.productImg = httpUtil.getProductLogoUrl(item["PRODUCT_ID"].ToString());//"http://my.ddishtv.mn:808/products/bagts.png";
                                                                                                         //prodDetial.additionalProducts = null;
                                                                                                         //DataTable dtUp = dbconn.getTable(appServiceQry._getMainProducts(item["UPGRADE_PRODUCTS"].ToString()));
                                                                                                         //List<addChennelDetial> upChannels = new List<addChennelDetial>();
                                                                                                         //foreach(DataRow dr in dtUp.Rows)
                                                                                                         //{
                                                                                                         //    addChennelDetial upch = new addChennelDetial();
                                                                                                         //    upch.productName = dr["PRODUCT_NAME_MON"].ToString();
                                                                                                         //    upch.productId = dr["PRODUCT_ID"].ToString();
                                                                                                         //    upch.smsCode = dr["SMS_CODE"].ToString();
                                                                                                         //    upch.productImg = "http://my.ddishtv.mn:808/products/bagts.png";
                                                                                                         //    upChannels.Add(upch);
                                                                                                         //}
                                                                                                         //prodDetial.upProducts = upChannels;
                                                                                                         // ======================= price List awah =============================

                                DataTable dtPrice = dbconn.getTable(appServiceQry._getMainProductPrices(item["PRODUCT_ID"].ToString()));
                                string priceP = string.Empty;
                                if(dtPrice.Rows.Count !=0)
                                {
                                    priceP = dtPrice.Rows[0]["PRICE"].ToString();
                                }
                                prodDetial.price = priceP;
                                ////List<priceList> priceL = new List<priceList>();
                                ////foreach (DataRow rr in dtPrice.Rows)
                                ////{
                                ////    priceList price = new priceList();
                                ////    price.productId = rr["PRODUCT_ID"].ToString();
                                ////    price.price = rr["PRICE"].ToString();
                                ////    price.month = rr["MONTH"].ToString();
                                ////    priceL.Add(price);
                                ////}
                                ////prodDetial.priceList = priceL;

                                prdList.Add(prodDetial);
                            }
                            prodMdl.isSuccess = true;
                            prodMdl.resultCode = HttpStatusCode.OK.ToString();
                            prodMdl.resultMessage = appConstantValues.MSG_SUCCESS;
                            prodMdl.productList = prdList;
                        }
                        else
                        {
                            prodMdl.isSuccess = false;
                            prodMdl.resultCode = HttpStatusCode.NotFound.ToString();
                            prodMdl.resultMessage = appConstantValues.MSG_NOFOUND;
                        }
                    }
                    else
                    {
                        prodMdl.isSuccess = false;
                        prodMdl.resultCode = HttpStatusCode.Unauthorized.ToString();
                        prodMdl.resultMessage = appConstantValues.MSG_EXPIRED;
                    }
                }
                else
                {
                    prodMdl.isSuccess = false;
                    prodMdl.resultCode = HttpStatusCode.NotFound.ToString();
                    prodMdl.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
                    LogWriter._error(TAG, dbres);
                }
            }
            catch(Exception ex)
            {
                exceptionManager.ManageException(ex, TAG);
                prodMdl.isSuccess = false;
                prodMdl.resultCode = HttpStatusCode.NotFound.ToString();
                prodMdl.resultMessage = ex.Message;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, prodMdl);
            LogWriter._channelList(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), "", serialzer.Serialize(prodMdl), token));
            return message;
        }
        /// <summary>
        /// Бүтээгдэхүүний дэлгэрэнгүй мэдээлэл
        /// </summary>
        /// <param name="productId">сонгосон багцын Id (mandatory)</param>
        /// <remarks>Тухайн бүтээгдэхүүний дэлгэрэнгүй мэдээлэл гарч ирнэ. upProducts array-д энэ service-с утга буцаахаа больсон байгаа. upgradeProduct service-г ашиглана уу.</remarks>
        /// <returns></returns>
        [HttpGet]
        [Route("{productId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<productAdditionDetial>))]
        public HttpResponseMessage Get(string productId)
        {
            HttpResponseMessage message = null;
            productAdditionDetial prdAddDetial = new productAdditionDetial();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        DataTable dtCh = dbconn.getTable(appServiceQry._getMainProducts(productId));
                        if(dtCh.Rows.Count !=0)
                        {
                            //DataTable dtUp = dbconn.getTable(appServiceQry._getMainProducts(dtCh.Rows[0]["UPGRADE_PRODUCTS"].ToString()));
                            //List<productDetial> upChannels = new List<productDetial>();
                            //foreach (DataRow dr in dtUp.Rows)
                            //{
                            //    productDetial upch = new productDetial();
                            //    upch.productName = dr["PRODUCT_NAME_MON"].ToString();
                            //    upch.productId = dr["PRODUCT_ID"].ToString();
                            //    upch.smsCode = dr["SMS_CODE"].ToString();
                            //    upch.productImg = httpUtil.getProductLogoUrl(dr["PRODUCT_ID"].ToString());
                            //    upChannels.Add(upch);
                            //}
                            DataTable dtAdd = dbconn.getTable(appServiceQry._getMainProducts(additionalProductSelector.getAddProd(productId)));
                            List<productDetial> addChannels = new List<productDetial>();
                            foreach(DataRow rf in dtAdd.Rows)
                            {
                                productDetial prd = new productDetial();
                                prd.productName = rf["PRODUCT_NAME_MON"].ToString();
                                prd.productId = rf["PRODUCT_ID"].ToString();
                                prd.smsCode = rf["SMS_CODE"].ToString();
                                prd.productImg = httpUtil.getProductLogoUrl(rf["PRODUCT_ID"].ToString());
                                DataTable dtPrice = dbconn.getTable(appServiceQry._getMainProductPrices(rf["PRODUCT_ID"].ToString()));
                                string priceP = string.Empty;
                                if (dtPrice.Rows.Count != 0)
                                {
                                    priceP = dtPrice.Rows[0]["PRICE"].ToString();
                                }
                                prd.price = priceP;
                                addChannels.Add(prd);
                            }
                            prdAddDetial.productId = productId;
                            prdAddDetial.upProducts = null;
                            prdAddDetial.additionalProducts = addChannels;
                            prdAddDetial.isSuccess = true;
                            prdAddDetial.resultCode = HttpStatusCode.OK.ToString();
                            prdAddDetial.resultMessage = appConstantValues.MSG_SUCCESS;
                        }
                        else
                        {
                            prdAddDetial.isSuccess = false;
                            prdAddDetial.resultCode = HttpStatusCode.NotFound.ToString();
                            prdAddDetial.resultMessage = appConstantValues.MSG_NOFOUND;
                        }
                    }
                    else
                    {
                        prdAddDetial.isSuccess = false;
                        prdAddDetial.resultCode = HttpStatusCode.Unauthorized.ToString();
                        prdAddDetial.resultMessage = appConstantValues.MSG_EXPIRED;
                    }
                }
                else
                {
                    prdAddDetial.isSuccess = false;
                    prdAddDetial.resultCode = HttpStatusCode.NotFound.ToString();
                    prdAddDetial.resultMessage = appConstantValues.MSG_INTERNAL_ERROR;
                    LogWriter._error(TAG, dbres);
                }
            }
            catch(Exception ex)
            {
                exceptionManager.ManageException(ex, TAG);
                prdAddDetial.isSuccess = false;
                prdAddDetial.resultCode = HttpStatusCode.NotFound.ToString();
                prdAddDetial.resultMessage = ex.Message;
            }
            message = Request.CreateResponse(HttpStatusCode.OK, prdAddDetial);
            LogWriter._channelList(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), productId, serialzer.Serialize(prdAddDetial), token));
            return message;
        }
    }
}

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
    [RoutePrefix("api/upgradeProduct")]
    public class upgradeProductController : ApiController
    {
        private string TAG = "upgradeProduct";
        DBConnection dbconn = new DBConnection();
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private string dbres = string.Empty;
        /// <summary>
        /// Ахиулах боломжтой багцын жагсаалт авах
        /// </summary>
        /// <param name="productId">Одоо идэвхтэй байгаа багцын ID (mandatory)</param>
        /// <remarks>Багц ахиулалт нь зөвхөн хэрэглэгчийн үндсэн багцууд идэвхтэй үед харагдана. Идэвхгүй үед багц ахиулалтыг ашиглавал хэрэглэгч талдаа ашиггүй байдаг юм. Тиймээс зөвхөн үндсэн багцуудын аль нэг нь идэвхтэй үед UI дээр харагддаг байдлаар зохион байгуулбал зүгээр</remarks>
        /// <returns></returns>
        [HttpGet]
        [Route("{productId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<upgradeProductListModel>))]
        public HttpResponseMessage Get(string productId)
        {
            HttpResponseMessage message = null;
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            upgradeProductListModel response = new upgradeProductListModel();
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        string oldProdEndDate = string.Empty;
                        DataTable dtOldProd = dbconn.getTable(appServiceQry._getupProdState(userCardNo, productId));
                        if(dtOldProd.Rows.Count !=0)
                        {
                            oldProdEndDate = dtOldProd.Rows[0]["ENDDATE"].ToString();
                        }
                        else
                        {
                            oldProdEndDate = DateTime.Now.AddDays(-1).ToString(appConstantValues.DATE_TIME_FORMAT);
                        }
                        DataTable dtCh = dbconn.getTable(appServiceQry._getMainProducts(productId));
                        if (dtCh.Rows.Count != 0)
                        {
                            DataTable dtUp = dbconn.getTable(appServiceQry._getMainProducts(dtCh.Rows[0]["UPGRADE_PRODUCTS"].ToString()));
                            List<upgradeproductDetial> upProds = new List<upgradeproductDetial>();
                            foreach (DataRow dr in dtUp.Rows)
                            {
                                upgradeproductDetial upch = new upgradeproductDetial();
                                upch.productName = dr["PRODUCT_NAME_MON"].ToString();
                                string upNewProdId = dr["PRODUCT_ID"].ToString();
                                upch.productId = upNewProdId;
                                upch.smsCode = dr["SMS_CODE"].ToString();
                                upch.productImg = httpUtil.getProductLogoUrl(dr["PRODUCT_ID"].ToString());
                                List<priceList> priceList = new List<priceList>();
                                for (int i = 1; i < 5; i++)
                                {
                                    priceList prLn = new priceList();
                                    int newUpPrice = 0;
                                    string endDateNew = string.Empty;
                                    if (i != 2)
                                    {
                                        prLn.month = i.ToString();
                                        prLn.productId = upNewProdId;
                                        if (upgradeProductCalculator.calcDate(productId, oldProdEndDate, upNewProdId, i, out newUpPrice, out endDateNew))
                                        {
                                            prLn.price = newUpPrice.ToString();
                                            prLn.endDate = endDateNew;
                                        }
                                        else
                                        {
                                            throw new InvalidOperationException("Ахиулах багцын үнэ бодоход алдаа гарлаа.");
                                        }
                                        priceList.Add(prLn);
                                    }
                                }
                                upch.priceList = priceList;
                                upProds.Add(upch);
                            }
                            response.isSuccess = true;
                            response.resultCode = HttpStatusCode.OK.ToString();
                            response.resultMessage = "success";
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
            string req = string.Format(@"PRODUCT: [{0}]", productId);
            LogWriter._upgradeProduct(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), req, serializer.Serialize(response), token));
            return message;
        }

        /// <summary>
        /// Ахиулах боломжтой багцын жагсаалтыг дурын сар дээр үнийн дүн бодуулж авах
        /// </summary>
        /// <param name="productId">Одоо идэвхтэй байгаа багцын ID (mandatory)</param>
        /// <param name="month">Багц ахиулах сарын тоо (mandatory)</param>
        /// <param name="toProductId">Ахиулахыг хүсэж буй багцын ID (mandatory)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{productId}/{month}/{toProductId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<upgradeoneproductDetial>))]
        public HttpResponseMessage Get(string productId, string month, string toProductId)
        {
            HttpResponseMessage message = null;
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            upgradeoneproductDetial response = new upgradeoneproductDetial();
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;
                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        string oldProdEndDate = string.Empty;
                        DataTable dtOldProd = dbconn.getTable(appServiceQry._getupProdState(userCardNo, productId));
                        if (dtOldProd.Rows.Count != 0)
                        {
                            oldProdEndDate = dtOldProd.Rows[0]["ENDDATE"].ToString();
                        }
                        else
                        {
                            oldProdEndDate = DateTime.Now.AddDays(-1).ToString(appConstantValues.DATE_TIME_FORMAT);
                        }

                        
                        DataTable dtUp = dbconn.getTable(appServiceQry._getMainProducts(toProductId));
                        //List<upgradeproductDetial> upProds = new List<upgradeproductDetial>();
                        //foreach (DataRow dr in dtUp.Rows)
                        //{
                        //    upgradeproductDetial upch = new upgradeproductDetial();
                        //    upch.productName = dr["PRODUCT_NAME_MON"].ToString();
                        string upNewProdId = dtUp.Rows[0]["PRODUCT_ID"].ToString();
                        //    upch.productId = upNewProdId;
                        //    upch.smsCode = dr["SMS_CODE"].ToString();
                        //    upch.productImg = httpUtil.getProductLogoUrl(dr["PRODUCT_ID"].ToString());
                        //    List<priceList> priceList = new List<priceList>();

                        //    priceList.Add(prLn);
                        //    upch.priceList = priceList;
                        //    upProds.Add(upch);
                        //}
                        priceList prLn = new priceList();
                        int newUpPrice = 0;
                        string endDateNew = string.Empty;
                        prLn.month = month;
                        prLn.productId = upNewProdId;
                        if (upgradeProductCalculator.calcDate(productId, oldProdEndDate, upNewProdId, Convert.ToInt32(month), out newUpPrice, out endDateNew))
                        {
                            prLn.price = newUpPrice.ToString();
                            prLn.endDate = endDateNew;
                        }
                        else
                        {
                            throw new InvalidOperationException("Ахиулах багцын үнэ бодоход алдаа гарлаа.");
                        }
                        response.isSuccess = true;
                        response.resultCode = HttpStatusCode.OK.ToString();
                        response.resultMessage = "success";
                        response.priceInfo = prLn;
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
            string req = string.Format(@"PRODUCT: [{0}], MONTH: [{1}], TOPRODUCTID: [{2}]", productId, month, toProductId);
            LogWriter._upgradeProduct(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), req, serializer.Serialize(response), token));
            return message;
        }

        /// <summary>
        /// Багц ахиулах service
        /// </summary>
        /// <param name="productId">Одоо идэвхтэй байгаа багцын ID (mandatory)</param>
        /// <param name="month">Багц ахиулах сарын тоо (mandatory)</param>
        /// <param name="price">Багц ахиулахад шаардагдах мөнгөн дүн (mandatory)</param>
        /// <param name="toProductId">Ахиулахыг хүсэж буй багцын ID (mandatory)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{productId}/{month}/{price}/{toProductId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModel>))]
        public HttpResponseMessage Get(string productId, string month, string price, string toProductId)
        {
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
                        string resMon = string.Empty;
                        string resEng = string.Empty;
                        string resCry = string.Empty;

                        if(dbconn.upgradeProductByCounter(userCardNo, userAdminNo, productId, toProductId, price, month, out resEng, out resMon, out resCry))
                        {
                            response.isSuccess = true;
                            response.resultCode = HttpStatusCode.OK.ToString();
                            response.resultMessage = resMon;
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = resMon;
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
                response.isSuccess = false;
                response.resultCode = HttpStatusCode.NotFound.ToString();
                response.resultMessage = ex.Message;
                exceptionManager.ManageException(ex, TAG);
            }
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            string req = string.Format(@"PRODUCT: [{0}], MONTH: [{1}], PRICE: [{2}], TOPRODUCTID: [{3}]", productId, month, price, toProductId);
            LogWriter._upgradeProduct(TAG, string.Format("IP: [{0}], Request: [{1}], Response: [{2}], Token: [{3}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), req, serializer.Serialize(response), token));
            return message;
        }
    }
}

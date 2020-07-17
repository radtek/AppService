//using AppServiceUtil.AppserviceModel;
using AppServiceUtil.DBControl;
using AppServiceUtil.TabletModel;
using AppServiceUtil.Utils;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace AppInstallerService.Controllers
{
    [RoutePrefix("v1/charge/Product")]
    public class chargeProductController : ApiController
    {
        private string TAG = "chargeProductController";
        DBControlNew dbconn = new DBControlNew();
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// Тухайн хэрэглэгчийг хайх Үндсэн багцын жагсаалт авах сервис
        /// </summary>
        /// <param name="searchVal">admin or cardno</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{searchVal}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<ProdutListResponse>))]
        public async Task<HttpResponseMessage> Get(string searchVal)
        {
            HttpResponseMessage message = new HttpResponseMessage();
            string ip = httpUtil.GetClientIPAddress(HttpContext.Current.Request);
            string secToken = string.Empty;
            ProdutListResponse response = new ProdutListResponse();
            LogWriter._chargeProd(TAG, string.Format(@"[>>] Request: [{0}]", searchVal));
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
                        if(dt.Rows.Count != 0)
                        {
                            string ccType = dt.Rows[0]["IS_PREPAID"].ToString();
                            string fname = dt.Rows[0]["SUBSCRIBER_FNAME"].ToString();
                            string admin = dt.Rows[0]["PHONE_NO"].ToString();
                            string card = dt.Rows[0]["CARD_NO"].ToString();
                            if (ccType != "2")
                            {
                                DataTable dtProd = dbconn.getTable(appServiceQry._getMainProducts("27,28,29,73"));
                                List<productDetial> prdList = new List<productDetial>();
                                foreach (DataRow item in dtProd.Rows)
                                {
                                    productDetial prodDetial = new productDetial();
                                    prodDetial.productName = item["PRODUCT_NAME_MON"].ToString();
                                    prodDetial.productId = item["PRODUCT_ID"].ToString();
                                    prodDetial.smsCode = item["SMS_CODE"].ToString();
                                    prodDetial.productImg = httpUtil.getProductLogoUrl(item["PRODUCT_ID"].ToString());
                                    DataTable dtPrice = dbconn.getTable(appServiceQry._getMainProductPrices(item["PRODUCT_ID"].ToString()));
                                    string priceP = string.Empty;
                                    if (dtPrice.Rows.Count != 0)
                                    {
                                        priceP = dtPrice.Rows[0]["PRICE"].ToString();
                                    }
                                    prodDetial.price = priceP;
                                    prdList.Add(prodDetial);
                                }
                                // additional channel list
                                DataTable dtAct = dbconn.getTable(tabletQuery.getActiveMainProducts(card, "27,28,29,73"));
                                DataTable dtAdd = new DataTable();
                                if (dtAct.Rows.Count != 0)
                                {
                                    string mProd = dtAct.Rows[0]["PRODUCT_ID"].ToString();
                                    dtAdd = dbconn.getTable(appServiceQry._getMainProducts(additionalProductSelector.getAddProd(mProd)));
                                }
                                else
                                {
                                    dtAdd = dbconn.getTable(appServiceQry._getMainProducts(additionalProductSelector.getAddProd("28")));
                                }
                                List<productDetial> addChannels = new List<productDetial>();
                                foreach (DataRow rf in dtAdd.Rows)
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

                                response.isSuccess = true;
                                response.errorCode = Convert.ToString((int)HttpStatusCode.OK);
                                response.resultMessage = "success";
                                response.firstName = fname;
                                response.cardNo = card;
                                response.adminNo = admin;
                                response.productList = prdList;
                                response.additionalProductList = addChannels;
                                response.ccType = ccType;
                            }
                            else
                            {
                                response.isSuccess = true;
                                response.errorCode = Convert.ToString((int)HttpStatusCode.OK);
                                response.resultMessage = "Дараа төлбөрт хэрэглэгч";
                                response.firstName = fname;
                                response.cardNo = card;
                                response.adminNo = admin;
                                response.ccType = ccType;
                            }
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
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._chargeProd(TAG, string.Format(@"[<<] ClientIp: [{0}], Response: [{1}]", ip, serializer.Serialize(response)));
            return message;
        }

        /// <summary>
        /// Хэрэглэгчид тохирох нэмэлт багцын жагсаалт авах сервис
        /// </summary>
        /// <param name="cardNo">Хэрэглэгчийн картын дугаар</param>
        /// <param name="pref">Prefix value default is 0</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{cardNo}/{pref}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<additionalProductListResponse>))]
        public async Task<HttpResponseMessage> Get(string cardNo, string pref)
        {
            HttpResponseMessage message = new HttpResponseMessage();
            string ip = httpUtil.GetClientIPAddress(HttpContext.Current.Request);
            string secToken = string.Empty;
            additionalProductListResponse response = new additionalProductListResponse();
            LogWriter._chargeProd(TAG, string.Format(@"[>>] Request: [Card:{0}, Pref:{1}]", cardNo, pref));
            try
            {
                secToken = HttpContext.Current.Request.Headers["Authorization"].Replace("Basic ", "").Trim();
                if (dbconn.idbStatOK())
                {
                    string insId = string.Empty;
                    string insPhone = string.Empty;
                    if (dbconn.tabletCheckToken(secToken, out insId, out insPhone))
                    {
                        DataTable dtAct = dbconn.getTable(tabletQuery.getActiveMainProducts(cardNo, "27,28,29,73"));
                        DataTable dtAdd = new DataTable();
                        if(dtAct.Rows.Count != 0)
                        {
                            string mProd = dtAct.Rows[0]["PRODUCT_ID"].ToString();
                            dtAdd = dbconn.getTable(appServiceQry._getMainProducts(additionalProductSelector.getAddProd(mProd)));
                        }
                        else
                        {
                            dtAdd = dbconn.getTable(appServiceQry._getMainProducts(additionalProductSelector.getAddProd("28")));
                        }
                        List<productDetial> addChannels = new List<productDetial>();
                        foreach (DataRow rf in dtAdd.Rows)
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
                        response.isSuccess = true;
                        response.errorCode = Convert.ToString((int)HttpStatusCode.OK);
                        response.resultMessage = "success";
                        response.productList = addChannels;
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
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._chargeProd(TAG, string.Format(@"[<<] ClientIp: [{0}], Response: [{1}]", ip, serializer.Serialize(response)));
            return message;
        }

        /// <summary>
        /// Хэрэглэгчийн багц сунгах сервис
        /// </summary>
        /// <param name="productId">Сунгах багцын ID</param>
        /// <param name="month">сунгах сар</param>
        /// <param name="amount">Мөнгөн дүн</param>
        /// <param name="cardNo">Хэрэглэгчийн картын дугаар</param>
        /// <param name="customerNo">Албан байгуулгаар баримт авах бол Байгуулгын РД оруулна, Хувь хэрэглэгч бол хоосон байна.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{productId}/{month}/{amount}/{cardNo}/{customerNo}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModelWidthVatTablet>))]
        public async Task<HttpResponseMessage> Get(string productId, string month, string amount, string cardNo, string customerNo)
        {
            HttpResponseMessage message = new HttpResponseMessage();
            defaultResponseModelWidthVatTablet response = new defaultResponseModelWidthVatTablet();
            _eBarimtRequest ebarimt = new _eBarimtRequest();
            string secToken = string.Empty;
            string ip = httpUtil.GetClientIPAddress(HttpContext.Current.Request);
            LogWriter._chargeProd(TAG, string.Format(@"[>>] Request: [Product:{0}, month:{1}, amount:{2}, card:{3}, customerNo:{4}]", productId, month, amount, cardNo, customerNo));
            try
            {
                secToken = HttpContext.Current.Request.Headers["Authorization"].Replace("Basic ", "").Trim();
                if (dbconn.idbStatOK())
                {
                    string insId = string.Empty;
                    string insPhone = string.Empty;
                    if (dbconn.tabletCheckToken(secToken, out insId, out insPhone))
                    {
                        ebarimt.cardNo = cardNo;
                        ebarimt.channelNo = "11";
                        ebarimt.customerEmail = string.Empty;
                        ebarimt.sendEmail = false;
                        ebarimt.employeeCode = insPhone;
                        if(customerNo != "0")
                        {
                            ebarimt.organization = true;
                            ebarimt.customerNo = customerNo;
                        }
                        else
                        {
                            ebarimt.organization = false;
                            ebarimt.customerNo = string.Empty;
                        }
                        var detials = new List<_transactionDetial>();
                        var stock = new _transactionDetial();
                        stock.barCode = "8463100";
                        stock.price = amount;
                        stock.productId = productId;
                        stock.productName = "Үйлчилгээ идэвхжүүлэх";
                        stock.unit = "сар";
                        stock.qty = month;
                        detials.Add(stock);
                        ebarimt.transaction = detials;
                        string desc = string.Format(@"[Charge Product] Installer App");
                        if (dbconn.chargeProduct(productId, month, insPhone, amount, desc, cardNo, "11"))
                        {
                            response.isSuccess = true;
                            response.errorCode = Convert.ToString((int)HttpStatusCode.OK);
                            int sttCode = 0;
                            string resp = string.Empty;
                            if (httpWorker.http_POST("http://192.168.10.182:5050/vat/getEBarimt", serializer.Serialize(ebarimt), out sttCode, out resp))
                            {
                                _eBarimtResponse mta = serializer.Deserialize<_eBarimtResponse>(resp);
                                if (mta.isSuccess)
                                {
                                    response.mtaResult = new MTAResult { merchantId = mta.merchantId, amount = mta.amount, billId = mta.billId, date = mta.resultDate, loterryNo = mta.lotteryNo, qrData = mta.qrData, tax = mta.cityTax, vat = mta.vat };
                                    response.resultMessage = "success";
                                }
                                else
                                {
                                    response.resultMessage = "Ebarimt гаргахад алдаа гарлаа. Лавлах: 77771434, 1434";
                                }

                            }
                            else
                            {
                                response.resultMessage = "Ebarimt гаргахад алдаа гарлаа. Лавлах: 77771434, 1434";
                            }
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.errorCode = Convert.ToString((int)HttpStatusCode.Conflict);
                            response.resultMessage = "Багц сунгахад алдаа гарлаа";
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
            message = Request.CreateResponse(HttpStatusCode.OK, response);
            LogWriter._chargeProd(TAG, string.Format(@"[<<] ClientIp: [{0}], Response: [{1}]", ip, serializer.Serialize(response)));
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
            catch (Exception ex)
            {
                LogWriter._error(TAG, string.Format("Input: [{0}], Exception: [{1}]", inS, ex.ToString()));
                dt.Clear();
            }
            return dt;
        }
    }
}

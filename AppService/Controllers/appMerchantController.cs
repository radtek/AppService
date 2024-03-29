﻿using AppServiceUtil.AppserviceModel;
using AppServiceUtil.DBControl;
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
using System.Web.Http.Routing.Constraints;
using System.Web.Script.Serialization;
namespace AppService.Controllers
{
    [RoutePrefix("api/appMerchant")]
    public class appMerchantController : ApiController
    {
        DBConnection dbconn = new DBConnection();
        private string TAG = "appMerchantController";
        private string dbres = string.Empty;
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        /// <summary>
        /// App мерчантаар гүйлгээ хийхээс өмнө заавал Invoice ID -г сервис ашиглаж авна.
        /// requestType: [1001 - cProduct, 1004 - cNvod, 1007 - cAccount, 1010 - cOAccount, 1013 - uProduct, 1017 - cOAccountNoLogin]
        /// </summary>
        /// <param name="requestObj"></param>
        /// <returns>Test</returns>
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModel>))]
        public async Task<HttpResponseMessage> Post([FromBody] appMerchantRequestModel requestObj)
        {
            HttpResponseMessage message = new HttpResponseMessage();
            defaultResponseModel response = new defaultResponseModel();
            
            LogWriter._merchant(TAG, string.Format(@"[>>] Request: [{0}]", serializer.Serialize(requestObj)));
            try
            {
                string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
                if (requestObj != null)
                {
                    if (dbconn.idbCheck(out dbres))
                    {
                        string genInvoice = genInvoice = convertors.generateInvoiceNo("1");
                        string cType = requestObj.requestType;
                        if (token== "YGHM9SHBC81LMR4G")
                        {
                            switch (cType)
                            {
                                case "1017":
                                    // charge OthersAccount No Login
                                    if (registerChargeOthersAccountNoLoginRequest(token, genInvoice, requestObj.cOAccountNoLogin))
                                    {
                                        response.isSuccess = true;
                                        response.resultCode = HttpStatusCode.OK.ToString();
                                        response.resultMessage = genInvoice;
                                    }
                                    else
                                    {
                                        response.isSuccess = false;
                                        response.resultCode = HttpStatusCode.InternalServerError.ToString();
                                        response.resultMessage = "can't register";
                                    }
                                    break;
                                default:
                                    response.isSuccess = false;
                                    response.resultCode = HttpStatusCode.NotAcceptable.ToString();
                                    response.resultMessage = "invalid request type";
                                    break;
                            }
                        }
                        else
                        {
                            string userCardNo = string.Empty;
                            string userAdminNo = string.Empty;
                            if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                            {
                                
                                switch (cType)
                                {
                                    case "1001":
                                        // charge Product
                                        if (registerChargeProductRequest(userCardNo, userAdminNo, token, genInvoice, requestObj.cProduct))
                                        {
                                            response.isSuccess = true;
                                            response.resultCode = HttpStatusCode.OK.ToString();
                                            response.resultMessage = genInvoice;
                                        }
                                        else
                                        {
                                            response.isSuccess = false;
                                            response.resultCode = HttpStatusCode.InternalServerError.ToString();
                                            response.resultMessage = "can't register";
                                        }
                                        break;
                                    case "1004":
                                        // order Nvod
                                        if (registerOrderNvodRequest(userCardNo, userAdminNo, token, genInvoice, requestObj.cNvod))
                                        {
                                            response.isSuccess = true;
                                            response.resultCode = HttpStatusCode.OK.ToString();
                                            response.resultMessage = genInvoice;
                                        }
                                        else
                                        {
                                            response.isSuccess = false;
                                            response.resultCode = HttpStatusCode.InternalServerError.ToString();
                                            response.resultMessage = "can't register";
                                        }
                                        break;
                                    case "1007":
                                        // charge Account
                                        if (registerChargeAccountRequest(userCardNo, userAdminNo, token, genInvoice, requestObj.cAccount))
                                        {
                                            response.isSuccess = true;
                                            response.resultCode = HttpStatusCode.OK.ToString();
                                            response.resultMessage = genInvoice;
                                        }
                                        else
                                        {
                                            response.isSuccess = false;
                                            response.resultCode = HttpStatusCode.InternalServerError.ToString();
                                            response.resultMessage = "can't register";
                                        }
                                        break;
                                    case "1010":
                                        // charge OthersAccount
                                        if (registerChargeOthersAccountRequest(userAdminNo, token, genInvoice, requestObj.cOAccount))
                                        {
                                            response.isSuccess = true;
                                            response.resultCode = HttpStatusCode.OK.ToString();
                                            response.resultMessage = genInvoice;
                                        }
                                        else
                                        {
                                            response.isSuccess = false;
                                            response.resultCode = HttpStatusCode.InternalServerError.ToString();
                                            response.resultMessage = "can't register";
                                        }
                                        break;
                                    case "1013":
                                        // upgrade Product
                                        if (registerUpgradeProductRequest(userCardNo, userAdminNo, token, genInvoice, requestObj.uProduct))
                                        {
                                            response.isSuccess = true;
                                            response.resultCode = HttpStatusCode.OK.ToString();
                                            response.resultMessage = genInvoice;

                                        }
                                        else
                                        {
                                            response.isSuccess = false;
                                            response.resultCode = HttpStatusCode.InternalServerError.ToString();
                                            response.resultMessage = "can't register";
                                        }
                                        break;

                                    default:
                                        response.isSuccess = false;
                                        response.resultCode = HttpStatusCode.NotAcceptable.ToString();
                                        response.resultMessage = "invalid request type";
                                        break;
                                }
                            }
                            else
                            {
                                response.isSuccess = false;
                                response.resultCode = HttpStatusCode.Unauthorized.ToString();
                                response.resultMessage = appConstantValues.MSG_EXPIRED;
                            }
                        }
                    }
                    else
                    {
                        response.isSuccess = false;
                        response.resultCode = HttpStatusCode.NotFound.ToString();
                        response.resultMessage = dbres;
                        LogWriter._error(TAG, dbres);
                    }

                }
                else
                {
                    response.isSuccess = false;
                    response.resultCode = HttpStatusCode.NotFound.ToString();
                    response.resultMessage = "invalid request";
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
            LogWriter._merchant(TAG, string.Format("[<<] IP: [{0}], Response: [{1}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), serializer.Serialize(response)));
            return message;
        }


        private bool registerChargeProductRequest(string cardNo, string phoneNo, string token, string invoiceNo, chargeProductRequest request)
        {
            bool res = false;
            try
            {
                if(request != null)
                {
                    string dbres = dbconn.iDBCommand(appServiceQry.setCProductRequest(cardNo, phoneNo, token, invoiceNo, request.productId, request.month, request.amount, request.bankName));
                    if (dbres.Contains("FFFFx["))
                    {
                        res = false;
                        LogWriter._error(TAG, dbres);
                    }
                    else
                    {
                        res = true;
                    }
                }
            }
            catch(Exception ex)
            {
                LogWriter._error(TAG, ex.Message);
            }
            return res;
        }
        private bool registerOrderNvodRequest(string cardNo, string phoneNo, string token, string invoiceNo, orderNvodRequest request)
        {
            bool res = false;
            try
            {
                if (request != null)
                {
                    string dbres = dbconn.iDBCommand(appServiceQry.setONvodRequest(cardNo, phoneNo, token, invoiceNo, request.productId, request.amount, request.bankName, request.smsCode, request.inDate));
                    if (dbres.Contains("FFFFx["))
                    {
                        res = false;
                        LogWriter._error(TAG, dbres);
                    }
                    else
                    {
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter._error(TAG, ex.Message);
            }
            return res;
        }
        private bool registerChargeAccountRequest(string cardNo, string phoneNo, string token, string invoiceNo, chargeAccountRequest request)
        {
            bool res = false;
            try
            {
                if (request != null)
                {
                    string dbres = dbconn.iDBCommand(appServiceQry.setCAccount("1007", cardNo, phoneNo, token, invoiceNo,  request.amount, request.bankName));
                    if (dbres.Contains("FFFFx["))
                    {
                        res = false;
                        LogWriter._error(TAG, dbres);
                    }
                    else
                    {
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter._error(TAG, ex.Message);
            }
            return res;
        }
        private bool registerChargeOthersAccountRequest(string phoneNo, string token, string invoiceNo, chargeOthersAccountRequest request)
        {
            bool res = false;
            try
            {
                if (request != null)
                {
                    string dbres = dbconn.iDBCommand(appServiceQry.setCAccount("1010", request.cardNo, phoneNo, token, invoiceNo, request.amount, request.bankName));
                    if (dbres.Contains("FFFFx["))
                    {
                        res = false;
                        LogWriter._error(TAG, dbres);
                    }
                    else
                    {
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter._error(TAG, ex.Message);
            }
            return res;
        }
        private bool registerChargeOthersAccountNoLoginRequest(string token, string invoiceNo, chargeOthersAccountNoLoginRequest request)
        {
            bool res = false;
            try
            {
                if (request != null)
                {
                    string isvat = request.isVat ? "0" : "1";
                    string dbres = dbconn.iDBCommand(appServiceQry.setCAccountNoLogin("1017", request.cardNo, request.deviceImei, token, invoiceNo, request.amount, request.bankName, isvat, request.email));
                    if (dbres.Contains("FFFFx["))
                    {
                        res = false;
                        LogWriter._error(TAG, dbres);
                    }
                    else
                    {
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter._error(TAG, ex.Message);
            }
            return res;
        }
        private bool registerUpgradeProductRequest(string cardNo, string phoneNo, string token, string invoiceNo, upgradeProductRequest request)
        {
            bool res = false;
            try
            {
                if (request != null)
                {
                    string dbres = dbconn.iDBCommand(appServiceQry.setUProductRequest(cardNo, phoneNo, token, invoiceNo, request.toProductId, request.amount, request.bankName));
                    if (dbres.Contains("FFFFx["))
                    {
                        res = false;
                        LogWriter._error(TAG, dbres);
                    }
                    else
                    {
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter._error(TAG, ex.Message);
            }
            return res;
        }
    }
}

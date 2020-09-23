using AppServiceUtil.AppserviceModel;
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
using System.Web.Script.Serialization;

namespace AppService.Controllers
{
    [RoutePrefix("api/upoint")]
    public class uPointController : ApiController
    {
        DBConnection dbconn = new DBConnection();
        private string TAG = "uPointController";
        private string dbres = string.Empty;
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// Upoint оноогоор багц сунгах сервис. 100% upoint оноогоор хасалт хийнэ.
        /// </summary>
        /// <param name="productId">Сунгах багцын Id (mandatory)</param>
        /// <param name="month">Сунгах сар (mandatory)</param>
        /// <param name="amount">Мөнгөн дүн (mandatory)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{productId}/{month}/{amount}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModel>))]
        public async Task<HttpResponseMessage> Get(string productId, string month, string amount)
        {
            HttpResponseMessage message = new HttpResponseMessage();
            defaultResponseModel response = new defaultResponseModel();
            upointCheckRequest upoint = new upointCheckRequest();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            string req = string.Format(@"PRODUCT: [{0}], MONTH: [{1}], AMOUNT: [{2}]]", productId, month, amount);
            LogWriter._chargeProd(TAG, string.Format(@"[>>] Request: ({0}), ", req));
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;

                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {

                        if (checkCustomType(userCardNo))
                        {
                            upoint.card_number = userCardNo;
                            upoint.mobile = "";
                            int sttCode = 0;
                            string resp = string.Empty;
                            string reqst = serializer.Serialize(upoint);
                            if (httpWorker.http_POST("http://192.168.10.136/api/check_info", reqst, out sttCode, out resp))
                            {
                                LogWriter._error(TAG, string.Format(@"request: {0}, response: {1}", reqst, resp.Replace("\"{", "{").Replace("}\"", "}").Replace("\\", "")));
                                if (resp.Length > 5)
                                {
                                    //string chresp = resp.TrimStart('\"').TrimEnd('\"');
                                    string chresp = resp.Replace("\"{", "{").Replace("}\"", "}").Replace("\\","");
                                    upointCheckModel upnt = serializer.Deserialize<upointCheckModel>(chresp);
                                    if (upnt.result == 0)
                                    {
                                        if (upnt.balance >= int.Parse(amount))
                                        {
                                            DataTable dt = dbconn.getTable("SELECT UNI_DISH.SEQ_TRANSACTION.NEXTVAL as TRANSID FROM DUAL");
                                            string trnId = dt.Rows[0]["TRANSID"].ToString();
                                            upointTransactionRequest utrans = new upointTransactionRequest();
                                            utrans.card_number = userCardNo;
                                            utrans.mobile = "";
                                            utrans.spend_amount = int.Parse(amount);
                                            utrans.total_amount = amount;
                                            utrans.cash_amount = int.Parse(amount);
                                            utrans.regdate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                                            utrans.trans_id = trnId;
                                            List<Item> items = new List<Item>();
                                            List<Bank> banks = new List<Bank>();
                                            utrans.bank = banks;
                                            Item item = new Item();
                                            item.name = "Багц сунгах үйлчилгээ";
                                            item.code = productId;
                                            item.unit = "сар";
                                            item.quantity = month;
                                            item.total_price = amount;
                                            item.price = (int.Parse(amount) / int.Parse(month)).ToString();
                                            items.Add(item);
                                            utrans.items = items;

                                            int sttCode1 = 0;
                                            string resp1 = string.Empty;

                                            string jjson = serializer.Serialize(utrans);
                                            if (httpWorker.http_POST("http://192.168.10.136/api/Transaction", jjson, out sttCode1, out resp1))
                                            {
                                                LogWriter._error(TAG, string.Format(@"request: {0}, response: {1}", jjson, resp1));
                                                upointTransResponse transResponse = serializer.Deserialize<upointTransResponse>(resp1);
                                                if (transResponse.issuccess == "true")
                                                {
                                                    string desc = string.Format(@"[Charge Product] Mobile App upoint");
                                                    if (dbconn.chargeProductInTrans(productId, month, userAdminNo, amount, desc, userCardNo, "6", trnId))
                                                    {
                                                        response.isSuccess = true;
                                                        response.resultCode = HttpStatusCode.OK.ToString();
                                                        response.resultMessage = string.Format(@"Таны сонгосон багц Upoint оноогоор амжилттай сунгагдлаа. Таны upoint онооны үлдэгдэл {0} байна.", transResponse.point_balance);
                                                    }
                                                    else
                                                    {
                                                        response.isSuccess = false;
                                                        response.resultCode = HttpStatusCode.Conflict.ToString();
                                                        response.resultMessage = "Багц сунгахад алдаа гарлаа";
                                                    }
                                                }
                                                else
                                                {
                                                    response.isSuccess = false;
                                                    response.resultCode = HttpStatusCode.NotFound.ToString();
                                                    response.resultMessage = "Upoint-с зарцуулалт хийхэд алдаа гарлаа.";
                                                }
                                            }
                                            else
                                            {
                                                response.isSuccess = false;
                                                response.resultCode = HttpStatusCode.NotFound.ToString();
                                                response.resultMessage = "Upoint-с зарцуулалт хийхэд алдаа гарлаа.";
                                            }

                                        }
                                        else
                                        {
                                            response.isSuccess = false;
                                            response.resultCode = HttpStatusCode.NotFound.ToString();
                                            response.resultMessage = "Таны Upoint оноо хүрэхгүй байна.";
                                        }
                                    }
                                    else
                                    {
                                        response.isSuccess = false;
                                        response.resultCode = HttpStatusCode.NotFound.ToString();
                                        response.resultMessage = "Upoint хэрэглэгч олдсонгүй.";
                                    }
                                }
                                else
                                {
                                    response.isSuccess = false;
                                    response.resultCode = HttpStatusCode.NotFound.ToString();
                                    response.resultMessage = "Upoint хэрэглэгч олдсонгүй.";
                                }
                            }
                            else
                            {
                                response.isSuccess = false;
                                response.resultCode = HttpStatusCode.NotFound.ToString();
                                response.resultMessage = "Хэрэглэгч шалгахад алдаа гарлаа.";
                            }

                        }
                        else
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = "Дараа төлбөрт хэрэглэгч багц сунгах боломжгүй.";
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
            LogWriter._chargeProd(TAG, string.Format("[<<] IP: [{0}], Response: [{1}], Token: [{2}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), serializer.Serialize(response), token));
            return message;
        }

        /// <summary>
        /// Upoint оноогоороо дансаа цэнэглэх сервис
        /// </summary>
        /// <param name="amount">Цэнэглэх мөнгөн дүн</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{amount}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModel>))]
        public async Task<HttpResponseMessage> Get(string amount)
        {
            HttpResponseMessage message = new HttpResponseMessage();
            defaultResponseModel response = new defaultResponseModel();
            upointCheckRequest upoint = new upointCheckRequest();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            string req = string.Format(@"AMOUNT: [{0}]", amount);
            LogWriter._chargeProd(TAG, string.Format(@"[>>] Request: ({0}), ", req));
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;

                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        if (checkCustomType(userCardNo))
                        {
                            upoint.card_number = userCardNo;
                            upoint.mobile = "";
                            int sttCode = 0;
                            string resp = string.Empty;
                            string reqst = serializer.Serialize(upoint);
                            if (httpWorker.http_POST("http://192.168.10.136/api/check_info", reqst, out sttCode, out resp))
                            {
                                LogWriter._error(TAG, string.Format(@"request: {0}, response: {1}", reqst, resp.Replace("\"{", "{").Replace("}\"", "}").Replace("\\", "")));
                                if (resp.Length > 5)
                                {
                                    string chresp = resp.Replace("\"{", "{").Replace("}\"", "}").Replace("\\", "");
                                    upointCheckModel upnt = serializer.Deserialize<upointCheckModel>(chresp);
                                    if (upnt.result == 0)
                                    {
                                        DataTable dt = dbconn.getTable("SELECT UNI_DISH.SEQ_TRANSACTION.NEXTVAL as TRANSID FROM DUAL");
                                        string trnId = dt.Rows[0]["TRANSID"].ToString();
                                        upointTransactionRequest utrans = new upointTransactionRequest();
                                        utrans.card_number = userCardNo;
                                        utrans.mobile = "";
                                        utrans.spend_amount = int.Parse(amount);
                                        utrans.total_amount = amount;
                                        utrans.cash_amount = int.Parse(amount);
                                        utrans.regdate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                                        utrans.trans_id = trnId;
                                        List<Item> items = new List<Item>();
                                        List<Bank> banks = new List<Bank>();
                                        utrans.bank = banks;
                                        Item item = new Item();
                                        item.name = "Данс цэнэглэх үйлчилгээ";
                                        item.code = "8";
                                        item.unit = "ш";
                                        item.quantity = "1";
                                        item.total_price = amount;
                                        item.price = amount;
                                        items.Add(item);
                                        utrans.items = items;
                                        int sttCode1 = 0;
                                        string resp1 = string.Empty;

                                        string jjson = serializer.Serialize(utrans);
                                        if (httpWorker.http_POST("http://192.168.10.136/api/Transaction", jjson, out sttCode1, out resp1))
                                        {
                                            LogWriter._error(TAG, string.Format(@"request: {0}, response: {1}", jjson, resp1));
                                            upointTransResponse transResponse = serializer.Deserialize<upointTransResponse>(resp1);
                                            if (transResponse.issuccess == "true")
                                            {
                                                string desc = string.Format(@"[Charge Account] Mobile App upoint");
                                                if (dbconn.chargeAccountInTrans(userCardNo, amount, userAdminNo, desc, trnId))
                                                {
                                                    response.isSuccess = true;
                                                    response.resultCode = HttpStatusCode.OK.ToString();
                                                    response.resultMessage = string.Format(@"Таны үндсэн данс Upoint оноогоор амжилттай цэнэглэгдлээ. Таны upoint онооны үлдэгдэл {0} байна.", transResponse.point_balance);
                                                }
                                                else
                                                {
                                                    response.isSuccess = false;
                                                    response.resultCode = HttpStatusCode.Conflict.ToString();
                                                    response.resultMessage = "Данс цэнэглэхэд алдаа гарлаа";
                                                }
                                            }
                                            else
                                            {
                                                response.isSuccess = false;
                                                response.resultCode = HttpStatusCode.NotFound.ToString();
                                                response.resultMessage = "Upoint-с зарцуулалт хийхэд алдаа гарлаа.";
                                            }
                                        }
                                        else
                                        {
                                            response.isSuccess = false;
                                            response.resultCode = HttpStatusCode.NotFound.ToString();
                                            response.resultMessage = "Upoint-с зарцуулалт хийхэд алдаа гарлаа.";
                                        }
                                    }
                                    else
                                    {
                                        response.isSuccess = false;
                                        response.resultCode = HttpStatusCode.NotFound.ToString();
                                        response.resultMessage = "Upoint хэрэглэгч олдсонгүй.";
                                    }
                                }
                                else
                                {
                                    response.isSuccess = false;
                                    response.resultCode = HttpStatusCode.NotFound.ToString();
                                    response.resultMessage = "Upoint хэрэглэгч олдсонгүй.";
                                }
                            }
                            else
                            {
                                response.isSuccess = false;
                                response.resultCode = HttpStatusCode.NotFound.ToString();
                                response.resultMessage = "Хэрэглэгч шалгахад алдаа гарлаа.";
                            }
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = "Дараа төлбөрт хэрэглэгч данс цэнэглэх боломжгүй.";
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
            LogWriter._chargeProd(TAG, string.Format("[<<] IP: [{0}], Response: [{1}], Token: [{2}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), serializer.Serialize(response), token));
            return message;
        }

        /// <summary>
        /// Upoint оноогоороо кино захиалах сервис
        /// </summary>
        /// <param name="productId">VOD cувгийн productId (mandatory)</param>
        /// <param name="smsCode">VOD cуваг захиалах smsCode (mandatory)</param>
        /// <param name="inDate">Тухайн контент гарах өдөр (mandatory)</param>
        /// <param name="amount">Тухайн Контентын үнийн дүн</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{productId}/{smsCode}/{inDate}/{amount}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<defaultResponseModel>))]
        public async Task<HttpResponseMessage> Get(string productId, string smsCode, string inDate, string amount)
        {
            HttpResponseMessage message = new HttpResponseMessage();
            defaultResponseModel response = new defaultResponseModel();
            upointCheckRequest upoint = new upointCheckRequest();
            string token = HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "").Trim();
            string req = string.Format(@"AMOUNT: [{0}], PRODUCT_ID: [{1}], SMSCODE: [{2}], IN_DATE: [{3}]", amount, productId, smsCode, inDate);
            LogWriter._chargeProd(TAG, string.Format(@"[>>] Request: ({0}), ", req));
            try
            {
                if (dbconn.idbCheck(out dbres))
                {
                    string userCardNo = string.Empty;
                    string userAdminNo = string.Empty;

                    if (dbconn.checkToken(token, out userCardNo, out userAdminNo))
                    {
                        if (checkCustomType(userCardNo))
                        {
                            upoint.card_number = userCardNo;
                            upoint.mobile = "";
                            int sttCode = 0;
                            string resp = string.Empty;
                            string reqst = serializer.Serialize(upoint);
                            if (httpWorker.http_POST("http://192.168.10.136/api/check_info", reqst, out sttCode, out resp))
                            {
                                LogWriter._error(TAG, string.Format(@"request: {0}, response: {1}", reqst, resp.Replace("\"{", "{").Replace("}\"", "}").Replace("\\", "")));
                                if (resp.Length > 5)
                                {
                                    string chresp = resp.Replace("\"{", "{").Replace("}\"", "}").Replace("\\", "");
                                    upointCheckModel upnt = serializer.Deserialize<upointCheckModel>(chresp);
                                    if (upnt.result == 0)
                                    {
                                        DataTable dt = dbconn.getTable("SELECT UNI_DISH.SEQ_TRANSACTION.NEXTVAL as TRANSID FROM DUAL");
                                        string trnId = dt.Rows[0]["TRANSID"].ToString();
                                        upointTransactionRequest utrans = new upointTransactionRequest();
                                        utrans.card_number = userCardNo;
                                        utrans.mobile = "";
                                        utrans.spend_amount = int.Parse(amount);
                                        utrans.total_amount = amount;
                                        utrans.cash_amount = int.Parse(amount);
                                        utrans.regdate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                                        utrans.trans_id = trnId;
                                        List<Item> items = new List<Item>();
                                        List<Bank> banks = new List<Bank>();
                                        utrans.bank = banks;
                                        Item item = new Item();
                                        item.name = "Данс цэнэглэх үйлчилгээ";
                                        item.code = "8";
                                        item.unit = "ш";
                                        item.quantity = "1";
                                        item.total_price = amount;
                                        item.price = amount;
                                        items.Add(item);
                                        utrans.items = items;
                                        int sttCode1 = 0;
                                        string resp1 = string.Empty;

                                        string jjson = serializer.Serialize(utrans);
                                        if (httpWorker.http_POST("http://192.168.10.136/api/Transaction", jjson, out sttCode1, out resp1))
                                        {
                                            LogWriter._error(TAG, string.Format(@"request: {0}, response: {1}", jjson, resp1));
                                            upointTransResponse transResponse = serializer.Deserialize<upointTransResponse>(resp1);
                                            if (transResponse.issuccess == "true")
                                            {
                                                string desc = string.Format(@"[Charge Account] Mobile App upoint");
                                                if (dbconn.chargeAccountInTrans(userCardNo, amount, userAdminNo, desc, trnId))
                                                {
                                                    //response.isSuccess = true;
                                                    //response.resultCode = HttpStatusCode.OK.ToString();
                                                    //response.resultMessage = string.Format(@"Таны үндсэн данс Upoint оноогоор амжилттай цэнэглэгдлээ. Таны upoint онооны үлдэгдэл {0} байна.", transResponse.point_balance);
                                                    string resMon = string.Empty;
                                                    string resEng = string.Empty;
                                                    string resCry = string.Empty;
                                                    if (dbconn.addNvodByCounter(userCardNo, userAdminNo, inDate, smsCode, productId, out resEng, out resMon, out resCry))
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
                                                    response.resultCode = HttpStatusCode.Conflict.ToString();
                                                    response.resultMessage = "Данс цэнэглэхэд алдаа гарлаа";
                                                }
                                            }
                                            else
                                            {
                                                response.isSuccess = false;
                                                response.resultCode = HttpStatusCode.NotFound.ToString();
                                                response.resultMessage = "Upoint-с зарцуулалт хийхэд алдаа гарлаа.";
                                            }
                                        }
                                        else
                                        {
                                            response.isSuccess = false;
                                            response.resultCode = HttpStatusCode.NotFound.ToString();
                                            response.resultMessage = "Upoint-с зарцуулалт хийхэд алдаа гарлаа.";
                                        }
                                    }
                                    else
                                    {
                                        response.isSuccess = false;
                                        response.resultCode = HttpStatusCode.NotFound.ToString();
                                        response.resultMessage = "Upoint хэрэглэгч олдсонгүй.";
                                    }
                                }
                                else
                                {
                                    response.isSuccess = false;
                                    response.resultCode = HttpStatusCode.NotFound.ToString();
                                    response.resultMessage = "Upoint хэрэглэгч олдсонгүй.";
                                }
                            }
                            else
                            {
                                response.isSuccess = false;
                                response.resultCode = HttpStatusCode.NotFound.ToString();
                                response.resultMessage = "Хэрэглэгч шалгахад алдаа гарлаа.";
                            }
                        }
                        else
                        {
                            response.isSuccess = false;
                            response.resultCode = HttpStatusCode.NotFound.ToString();
                            response.resultMessage = "Дараа төлбөрт хэрэглэгч данс цэнэглэх боломжгүй.";
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
            LogWriter._chargeProd(TAG, string.Format("[<<] IP: [{0}], Response: [{1}], Token: [{2}]", httpUtil.GetClientIPAddress(HttpContext.Current.Request), serializer.Serialize(response), token));
            return message;
        }


        private bool checkCustomType(string cardNo)
        {
            bool res = false;
            try
            {
                DataTable dt = dbconn.getTable(appServiceQry.checkCard(cardNo));
                if (dt.Rows.Count != 0)
                {
                    string type = dt.Rows[0]["IS_PREPAID"].ToString();
                    if (type == "1")
                    {
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionManager.ManageException(ex, TAG);
            }
            return res;
        }

    }
}

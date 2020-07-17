using AppServiceUtil.DBControl;
using AppServiceUtil.TabletModel;
using AppServiceUtil.Utils;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using AppServiceUtil.AppserviceModel;

namespace AppInstallerService.Controllers
{
    [RoutePrefix("v1/UserInfo")]
    public class userInfoController : ApiController
    {
        private string TAG = "userInfoController";
        DBControlNew dbconn = new DBControlNew();
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// Хэрэглэгчийн мэдээлэл авах сервис
        /// </summary>
        /// <param name="searchVal">admin or cardno</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{searchVal}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<UserInfoResponse>))]
        public async Task<HttpResponseMessage> Get(string searchVal)
        {
            LogWriter._other(TAG, string.Format(@"[>>] Request: [{0}]", searchVal));
            HttpResponseMessage message = new HttpResponseMessage();
            UserInfoResponse response = new UserInfoResponse();
            string ip = httpUtil.GetClientIPAddress(HttpContext.Current.Request);
            string secToken = string.Empty;
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
                        if (dt.Rows.Count == 1)
                        {
                            string cardNo = dt.Rows[0]["CARD_NO"].ToString();
                            DataTable dtUInfo = dbconn.getTable(tabletQuery.getUserInfo(cardNo));
                            DataTable dtAcc = dbconn.getTable(tabletQuery.getAccountInfo(cardNo));
                            DataTable dtProd = dbconn.getTable(appServiceQry._getRefProduct(cardNo));
                            DataTable dtMem = dbconn.getTable(tabletQuery.getMembers(cardNo));

                            response.customerId = dtUInfo.Rows[0]["SUBSCRIBER_CODE"].ToString();
                            response.firstName = dtUInfo.Rows[0]["SUBSCRIBER_FNAME"].ToString();
                            response.lastName = dtUInfo.Rows[0]["SUBSCRIBER_LNAME"].ToString();
                            response.certificateNo = dtUInfo.Rows[0]["CERTIFICATE_NO"].ToString();
                            response.homeAddress = string.Format(@"{0} {1} {2}", dtUInfo.Rows[0]["AIMAGNAME"].ToString(), dtUInfo.Rows[0]["SUMNAME"].ToString(), dtUInfo.Rows[0]["BAGNAME"].ToString());
                            response.telephone = dtUInfo.Rows[0]["TELEPHONE"].ToString();
                            response.createdDate = dtUInfo.Rows[0]["CDATE"].ToString();
                            response.createdUser = dtUInfo.Rows[0]["OPERATION_ID"].ToString();
                            response.cardNo = dtUInfo.Rows[0]["CARD_NO"].ToString();
                            response.stbNo = dtUInfo.Rows[0]["STB_NO"].ToString();
                            response.adminNo = dtUInfo.Rows[0]["PHONE_NO"].ToString();
                            response.password = dtUInfo.Rows[0]["SUBSCRIBER_PASS"].ToString();
                            response.customerType = dtUInfo.Rows[0]["TYPE_NAME"].ToString();
                            response.ccType = dtUInfo.Rows[0]["IS_PREPAID"].ToString();
                            List<Product> prds = new List<Product>();
                            List<AccountInfo> accs = new List<AccountInfo>();
                            List<Member> mmbrs = new List<Member>();
                            foreach (DataRow item in dtProd.Rows)
                            {
                                Product prd = new Product();
                                prd.productName = item["PRODUCT_NAME_MON"].ToString();
                                prd.endDate = item["ENDTIME"].ToString();
                                prds.Add(prd);
                            }
                            foreach(DataRow item in dtAcc.Rows)
                            {
                                AccountInfo acc = new AccountInfo();
                                acc.accountName = item["NAME"].ToString();
                                acc.amount = double.Parse(item["COUNTER_AMOUNT"].ToString());
                                acc.expireDate = item["EXDATE"].ToString();
                                accs.Add(acc);
                            }
                            foreach(DataRow item in dtMem.Rows)
                            {
                                Member mm = new Member();
                                mm.memberNo = item["MSISDN"].ToString();
                                mmbrs.Add(mm);
                            }
                            response.activeProducts = prds;
                            response.activeAccounts = accs;
                            response.members = mmbrs;
                            response.isSuccess = true;
                            response.errorCode = Convert.ToString((int)HttpStatusCode.OK);
                            response.resultMessage = "success";

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
            LogWriter._other(TAG, string.Format(@"[<<] ClientIp: [{0}], Response: [{1}]", ip, serializer.Serialize(response)));
            message = Request.CreateResponse(HttpStatusCode.OK, response);
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

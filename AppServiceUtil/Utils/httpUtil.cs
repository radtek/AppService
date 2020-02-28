using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AppServiceUtil.Utils
{
    public class httpUtil
    {
        public static bool httpCall_GET(string _phone, string _userName, string _districtCode, string _type, out string _response)
        {
            bool funcStatus = false;
            _response = string.Empty;
            try
            {
                string url = string.Format(@"http://192.168.10.88:9000/index.php/call_account/NewOrderfromMolieApp?number={0}&name={1}&duureg={2}&type={3}", _phone, _userName, _districtCode, _type);
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender1, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(string.Format(url));
                httpRequest.Method = "GET";
                httpRequest.ContentType = "application/json; encoding='utf-8'";
                httpRequest.Timeout = 3 * 60 * 1000;
                string post_response = string.Empty;
                HttpWebResponse osdresponse = (HttpWebResponse)httpRequest.GetResponse();
                using (StreamReader responseStream = new StreamReader(osdresponse.GetResponseStream()))
                {
                    post_response = responseStream.ReadToEnd();
                    responseStream.Close();
                }
                _response = post_response;
                funcStatus = true;
            }
            catch(Exception ex)
            {
                exceptionManager.ManageException(ex, "httpCall_GET");
                funcStatus = false;
            }
            return funcStatus;
        }
        public static bool httpCall_POST_google(string _request, out string _response)
        {
            bool funcStatus = false;
            _response = string.Empty;
            try
            {
                string url = ConfigurationManager.AppSettings["fireBaseUrl"];
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender1, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(string.Format(url));
                httpRequest.Method = "POST";
                httpRequest.ContentType = "application/json; encoding='utf-8'";
                httpRequest.Timeout = 3 * 60 * 1000;
                httpRequest.Headers["authorization"] = ConfigurationManager.AppSettings["fireBaseToken"];
                httpRequest.Headers["sender"] = ConfigurationManager.AppSettings["fireBaseSender"];
                byte[] postBytes = Encoding.UTF8.GetBytes(_request);
                httpRequest.ContentLength = postBytes.Length;
                Stream requestStream = httpRequest.GetRequestStream();
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();
                string post_response = string.Empty;
                HttpWebResponse osdresponse = (HttpWebResponse)httpRequest.GetResponse();
                using (StreamReader responseStream = new StreamReader(osdresponse.GetResponseStream()))
                {
                    post_response = responseStream.ReadToEnd();
                    responseStream.Close();
                }
                _response = post_response;
                funcStatus = true;
            }
            catch (Exception ex)
            {
                exceptionManager.ManageException(ex, "httpCall_POST_Google");
                funcStatus = false;
            }
            return funcStatus;
        }
        public static bool httpCall_POST_local(string _request, string _controller, out string _response)
        {
            bool funcStatus = false;
            _response = string.Empty;
            try
            {
                string url = string.Format("{0}/{1}", ConfigurationManager.AppSettings["localUrl"], _controller);
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(string.Format(url));
                httpRequest.Method = "POST";
                httpRequest.ContentType = "application/json; encoding='utf-8'";
                httpRequest.Timeout = 3 * 60 * 1000;
                byte[] postBytes = Encoding.UTF8.GetBytes(_request);
                httpRequest.ContentLength = postBytes.Length;
                Stream requestStream = httpRequest.GetRequestStream();
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();
                string post_response = string.Empty;
                HttpWebResponse osdresponse = (HttpWebResponse)httpRequest.GetResponse();
                using (StreamReader responseStream = new StreamReader(osdresponse.GetResponseStream()))
                {
                    post_response = responseStream.ReadToEnd();
                    responseStream.Close();
                }
                _response = post_response;
                funcStatus = true;
            }
            catch (Exception ex)
            {
                exceptionManager.ManageException(ex, "httpCall_POST_LocalAPI");
                funcStatus = false;
            }
            return funcStatus;
        }

        public static string GetClientIPAddress(HttpRequest httpRequest)
        {
            string OriginalIP = string.Empty;
            string RemoteIP = string.Empty;

            OriginalIP = httpRequest.ServerVariables["HTTP_X_FORWARDED_FOR"];

            RemoteIP = httpRequest.ServerVariables["REMOTE_ADDR"];

            if (OriginalIP != null && OriginalIP.Trim().Length > 0)
            {
                return OriginalIP + "(" + RemoteIP + ")";
            }

            return RemoteIP;
        }
        public static string getProductLogoUrl(string productId)
        {
            string url = string.Empty;
            switch (productId)
            {
                case "28":
                    url = "http://my.ddishtv.mn:808/products/S01.png";
                    break;
                case "27":
                    url = "http://my.ddishtv.mn:808/products/L01.png";
                    break;
                case "29":
                    url = "http://my.ddishtv.mn:808/products/M01.png";
                    break;
                case "73":
                    url = "http://my.ddishtv.mn:808/products/XL01.png";
                    break;
                case "25":
                    url = "http://my.ddishtv.mn:808/nvod/800.png";
                    break;
                case "17":
                    url = "http://my.ddishtv.mn:808/nvod/801.png";
                    break;
                case "18":
                    url = "http://my.ddishtv.mn:808/nvod/802.png";
                    break;
                case "19":
                    url = "http://my.ddishtv.mn:808/nvod/803.png";
                    break;
                case "3":
                    url = "http://my.ddishtv.mn:808/products/SPS.png";
                    break;
                case "26":
                    url = "http://my.ddishtv.mn:808/products/playboy_tv.png";
                    break;
                case "70":
                    url = "http://my.ddishtv.mn:808/products/tsuwral.png";
                    break;
                case "67":
                    url = "http://my.ddishtv.mn:808/products/kinoBagts.jpg";
                    break;
                case "79":
                    url = "http://my.ddishtv.mn:808/products/HitsKids.png";
                    break;
                default:
                    url = string.Empty;
                    break;
            }
            return url;
        }
    }
}

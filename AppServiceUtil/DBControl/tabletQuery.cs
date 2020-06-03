using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.DBControl
{
    public class tabletQuery
    {
        public static string getLogin(string loginName, string password)
        {
            string qry = string.Format(@"SELECT USERID, USER_PHONE, USER_NAME FROM MPP_TABLET_USERS WHERE USER_STATUS=0 AND USER_PHONE='{0}' AND PASSWORD='{1}'", loginName, password);
            return qry;
        }
        public static string getLoggedUser(string token)
        {
            string qry = string.Format(@"SELECT USERID, USER_PHONE FROM MPP_TABLET_TOKEN WHERE TOKEN = '{0}' AND EXPIRE_DATE > SYSDATE", token);
            return qry;
        }
        public static string getNoifList(string userId)
        {
            string qry = string.Format(@"SELECT NOTIF_ID, NOTIF_NAME, NOTIF_TXT, TO_CHAR(CREATED_DATE, 'YYYY-MM-DD HH24:MI') AS CDATE FROM MPP_TABLET_NOTIFICATION WHERE READ_USER IN (0, {0}) ORDER BY NOTIF_ID DESC", userId);
            return qry;
        }
        public static string getReadList(string userId)
        {
            string qry = string.Format(@"SELECT NOTIF_ID, READ_USER FROM MPP_TABLET_NOTIF_ISREAD WHERE READ_USER={0}", userId);
            return qry;
        }
        public static string setReadList(string notifId, string userId)
        {
            string qry = string.Format(@"INSERT INTO MPP_TABLET_NOTIF_ISREAD (NOTIF_ID, READ_USER) VALUES ('{0}', '{1}')", notifId, userId);
            return qry;
        }
        public static string setDeletedList(string notifId, string userId)
        {
            string qry = string.Format(@"INSERT INTO MPP_TABLET_NOTIF_ISDELETED (NOTIF_ID, DELETE_USER) VALUES ('{0}', '{1}')", notifId, userId);
            return qry;
        }
        public static string getUserInfoByAdminNo(string adminNo)
        {
            string qry = string.Format(@"SELECT AA.SUBSCRIBER_CODE, AA.SUBSCRIBER_FNAME, AA.SUBSCRIBER_LNAME, AA.STB_NO, BB.PHONE_NO, AA.CERTIFICATE_NO, AA.SUBSCRIBER_PASS, BB.CARD_NO, AA.IS_PREPAID, CC.COUNTER_AMOUNT FROM T_DISH_CUSTOM AA LEFT JOIN ADMIN_NUMBER BB ON AA.CARD_NO = BB.CARD_NO 
LEFT JOIN D_ACCOUNT_COUNTERS CC ON CC.CARD_NO = BB.CARD_NO
WHERE BB.PHONE_NO = '{0}' AND AA.SUBSCRIBER_STATUS=1 AND CC.COUNTER_ID = 1001", adminNo);
            return qry;
        }
        public static string getUserInfoByCardNo(string cardNo)
        {
            string qry = string.Format(@"SELECT AA.SUBSCRIBER_CODE, AA.SUBSCRIBER_FNAME, AA.SUBSCRIBER_LNAME, AA.STB_NO, BB.PHONE_NO, AA.CERTIFICATE_NO, AA.SUBSCRIBER_PASS, AA.CARD_NO, AA.IS_PREPAID, CC.COUNTER_AMOUNT FROM T_DISH_CUSTOM AA LEFT JOIN ADMIN_NUMBER BB ON AA.CARD_NO = BB.CARD_NO
LEFT JOIN D_ACCOUNT_COUNTERS CC ON CC.CARD_NO = AA.CARD_NO
WHERE AA.CARD_NO = '{0}' AND AA.SUBSCRIBER_STATUS=1 AND CC.COUNTER_ID =1001", cardNo);
            return qry;
        }
    }
}

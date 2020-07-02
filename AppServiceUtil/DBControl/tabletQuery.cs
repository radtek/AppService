using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

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
        public static string getUnReadedCount(string userId)
        {
            string qry = string.Format(@"SELECT COUNT(0) AS COUNTN FROM MPP_TABLET_NOTIFICATION WHERE READ_USER IN (0, {0}) AND NOTIF_ID NOT IN (SELECT NOTIF_ID FROM MPP_TABLET_NOTIF_ISREAD WHERE READ_USER ={0})", userId);
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
LEFT JOIN D_ACCOUNT_COUNTERS CC ON CC.CARD_NO = BB.CARD_NO AND CC.COUNTER_ID = 1001
WHERE BB.PHONE_NO = '{0}' AND AA.SUBSCRIBER_STATUS=1", adminNo);
            return qry;
        }
        public static string getUserInfoByCardNo(string cardNo)
        {
            string qry = string.Format(@"SELECT AA.SUBSCRIBER_CODE, AA.SUBSCRIBER_FNAME, AA.SUBSCRIBER_LNAME, AA.STB_NO, BB.PHONE_NO, AA.CERTIFICATE_NO, AA.SUBSCRIBER_PASS, AA.CARD_NO, AA.IS_PREPAID, CC.COUNTER_AMOUNT FROM T_DISH_CUSTOM AA LEFT JOIN ADMIN_NUMBER BB ON AA.CARD_NO = BB.CARD_NO
LEFT JOIN D_ACCOUNT_COUNTERS CC ON CC.CARD_NO = AA.CARD_NO AND CC.COUNTER_ID =1001
WHERE AA.CARD_NO = '{0}' AND AA.SUBSCRIBER_STATUS=1", cardNo);
            return qry;
        }
        public static string getUserInfo(string cardNo)
        {
            string qry = string.Format(@"SELECT AA.SUBSCRIBER_CODE, AA.SUBSCRIBER_FNAME, AA.SUBSCRIBER_LNAME, AA.STB_NO, AA.CERTIFICATE_NO, AA.SUBSCRIBER_PASS, AA.CARD_NO, AA.IS_PREPAID, AA.OPERATION_ID, AA.TELEPHONE, BB.TYPE_NAME, CC.PHONE_NO, DD.NAME AS AIMAGNAME, EE.NAME AS SUMNAME, FF.NAME AS BAGNAME, AA.HOME_ADDRESS, TO_CHAR(AA.CREATE_DATE, 'YYYY.MM.DD') AS CDATE  FROM T_DISH_CUSTOM AA LEFT JOIN T_DISH_CUSTOM_TYPE BB  ON AA.SUBSCRIBER_TYPE = BB.TYPE_ID 
LEFT JOIN ADMIN_NUMBER CC ON AA.CARD_NO = CC.CARD_NO
LEFT JOIN T_DISH_AIMAG DD ON AA.ADDRESS_AIMAG = DD.ID
LEFT JOIN T_DISH_SUM EE ON AA.ADDRESS_SUM = EE.ID
LEFT JOIN T_DISH_BAG FF ON AA.ADDRESS_BAG = FF.ID
WHERE AA.CARD_NO = '{0}' AND AA.SUBSCRIBER_STATUS=1", cardNo);
            return qry;
        }
        public static string getAccountInfo(string cardNo)
        {
            string qry = string.Format(@"SELECT BB.NAME, AA.COUNTER_AMOUNT, AA.COUNTER_ID, TO_CHAR(AA.COUNTER_EXPIRE_DATE, 'YYYY.MM.DD') AS EXDATE FROM (SELECT COUNTER_AMOUNT, COUNTER_ID, COUNTER_EXPIRE_DATE FROM D_ACCOUNT_COUNTERS WHERE CARD_NO ='{0}' AND COUNTER_ID=1001
UNION ALL 
SELECT COUNTER_AMOUNT, COUNTER_ID, COUNTER_EXPIRE_DATE FROM D_ACCOUNT_COUNTERS WHERE CARD_NO ='{0}' AND COUNTER_EXPIRE_DATE >SYSDATE) AA LEFT JOIN D_COUNTERS BB ON AA.COUNTER_ID = BB.COUNTER_ID", cardNo);
            return qry;
        }
        public static string getMembers(string cardNo)
        {
            string qry = string.Format("SELECT MSISDN FROM D_MEMBER WHERE CARD_NO='{0}'", cardNo);
            return qry;
        }
    }
}

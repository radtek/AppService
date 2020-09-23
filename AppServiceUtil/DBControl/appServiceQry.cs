using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.DBControl
{
    public class appServiceQry
    {
        public static string _getUserInfoByCardNo(string cardNo)
        {
            string qry = string.Format(@"SELECT AA.SUBSCRIBER_CODE, AA.SUBSCRIBER_FNAME, AA.SUBSCRIBER_LNAME, AA.STB_NO, BB.PHONE_NO, AA.CERTIFICATE_NO, AA.SUBSCRIBER_PASS, BB.CARD_NO FROM T_DISH_CUSTOM AA LEFT JOIN ADMIN_NUMBER BB ON AA.CARD_NO = BB.CARD_NO WHERE BB.CARD_NO = '{0}' AND AA.SUBSCRIBER_STATUS=1", cardNo);
            return qry;
        }
        public static string _getUserInfoByAdminNo(string PhoneNo)
        {
            string qry = string.Format(@"SELECT AA.SUBSCRIBER_CODE, AA.SUBSCRIBER_FNAME, AA.SUBSCRIBER_LNAME, AA.STB_NO, BB.PHONE_NO, AA.CERTIFICATE_NO, AA.SUBSCRIBER_PASS, BB.CARD_NO FROM T_DISH_CUSTOM AA LEFT JOIN ADMIN_NUMBER BB ON AA.CARD_NO = BB.CARD_NO WHERE BB.PHONE_NO = '{0}' AND AA.SUBSCRIBER_STATUS=1", PhoneNo);
            return qry;
        }
        public static string _getUserInfoByMemberNo(string PhoneNo)
        {
            string qry = string.Format(@"SELECT AA.SUBSCRIBER_CODE, AA.SUBSCRIBER_FNAME, AA.SUBSCRIBER_LNAME, AA.STB_NO, BB.MSISDN AS PHONE_NO, AA.CERTIFICATE_NO, AA.SUBSCRIBER_PASS, BB.CARD_NO FROM T_DISH_CUSTOM AA LEFT JOIN D_MEMBER BB ON AA.CARD_NO = BB.CARD_NO WHERE BB.MSISDN = '{0}' AND AA.SUBSCRIBER_STATUS=1", PhoneNo);
            return qry;
        }
        public static string _getPromoCounters(string cardNo)
        {
            string qry = string.Format(@"SELECT BB.NAME, AA.COUNTER_AMOUNT, TO_CHAR(AA.COUNTER_EXPIRE_DATE, 'YYYY-MM-DD HH24:MI:SS') AS EXPIREDATE, AA.COUNTER_ID, BB.MEASUREUNIT FROM D_ACCOUNT_COUNTERS AA, D_COUNTERS BB WHERE AA.CARD_NO ='{0}' AND AA.COUNTER_EXPIRE_DATE >= SYSDATE AND AA.COUNTER_ID = BB.COUNTER_ID ORDER BY AA.COUNTER_ID ASC", cardNo);
            return qry;
        }
        public static string _getMainCounters(string cardNo)
        {
            string qry = string.Format(@"SELECT BB.NAME, AA.COUNTER_AMOUNT, TO_CHAR(AA.COUNTER_EXPIRE_DATE, 'YYYY-MM-DD HH24:MI:SS') AS EXPIREDATE, AA.COUNTER_ID, BB.MEASUREUNIT FROM D_ACCOUNT_COUNTERS AA, D_COUNTERS BB WHERE AA.CARD_NO ='{0}' AND AA.COUNTER_ID = 1001 AND AA.COUNTER_ID = BB.COUNTER_ID ORDER BY AA.COUNTER_ID ASC", cardNo);
            return qry;
        }
        public static string _getProducts(string cardNo)
        {
            string qry = string.Format(@"SELECT  BB.PRODUCT_NAME_MON, AA.PRODUCT_ID, TO_CHAR(AA.END_DATE, 'YYYY-MM-DD HH24:MI:SS') AS ENDDATE, BB.ORDERING FROM ACCOUNT_SERVICE AA, PRODUCT_CATALOG BB WHERE AA.IS_ACTIVE=0 AND AA.CARD_NUMBER='{0}' AND AA.END_DATE > SYSDATE AND AA.PRODUCT_ID = BB.PRODUCT_ID", cardNo);
            return qry;
        }
        public static string _registerFBUser(string cardNo, string adminNo, string fbId)
        {
            string qry = string.Format(@"INSERT INTO APP_FB_USER(CARD_NO, ADMIN_NO, FACEBOOK_ID) VALUES ('{0}', '{1}', '{2}')", cardNo, adminNo, fbId);
            return qry;
        }
        public static string _getFBUser(string fbId)
        {
            string qry = string.Format(@"SELECT CARD_NO, ADMIN_NO FROM APP_FB_USER WHERE FACEBOOK_ID='{0}'", fbId);
            return qry;
        }
        public static string _checkRToken(string refreshToken)
        {
            string qry = string.Format(@"SELECT CARD_NO, PHONE_NO FROM APP_AUTH_REFRESH_TOKEN WHERE REFRESH_TOKEN = '{0}' AND EXPIRE_DATE>SYSDATE", refreshToken);
            return qry;
        }
        public static string _vodChannelList()
        {
            string qry = string.Format(@"SELECT PRODUCT_ID, PRODUCT_NAME_MON, TV_CHANNEL FROM PRODUCT_CATALOG WHERE PRODUCT_ID IN (SELECT PRODUCT_ID FROM PRODUCT_CATALOG_VOD WHERE CHANNEL_TYPE ='NVOD') ORDER BY TV_CHANNEL ASC");
            return qry;
        }
        public static string _vodProgramList(string begin, string productid)
        {
            string qry = string.Format(@"SELECT BB.SMS_CODE, BB.NAME, BB.NAME_ENGLISH, BB.BEGINDATE, BB.ENDDATE, BB.INDATE, BB.PRODUCT_ID, BB.ID, BB.PRICE, AA.IMAGE_WEB FROM TV_CONTENT AA, (SELECT AAA.SMS_CODE, AAA.NAME, AAA.NAME_ENGLISH, TO_CHAR(AAA.BEGIND,'YYYY-MM-DD HH24:MI:SS') AS BEGINDATE, TO_CHAR(AAA.ENDD,'YYYY-MM-DD HH24:MI:SS') AS ENDDATE, TO_CHAR(AAA.BEGIND,'YYYY-MM-DD') AS INDATE, AAA.PRODUCT_ID, AAA.ID, AAA.PRICE FROM (SELECT SMS_CODE, NAME, NAME_ENGLISH,  TO_DATE(IN_DATE || START_TIME, 'YYYYMMDDHH24MISS') AS BEGIND, TO_DATE(IN_DATE || STOP_TIME, 'YYYYMMDDHH24MISS') AS ENDD, PRODUCT_ID, ID, PRICE FROM TV_PROGRAM WHERE IN_DATE ='{0}' AND TO_DATE(IN_DATE || START_TIME, 'YYYYMMDDHH24MISS') > SYSDATE AND IS_DELETED='N' AND PRODUCT_ID={1}) AAA) BB WHERE AA.ID = BB.ID ORDER BY BB.BEGINDATE ASC", begin, productid);
            return qry;
        }
        public static string _vodProgramSearch(string begin, string _searchValue)
        {
            string qry = string.Format(@"SELECT BB.SMS_CODE, BB.NAME, BB.NAME_ENGLISH, BB.BEGINDATE, BB.ENDDATE, BB.INDATE, BB.PRODUCT_ID, BB.ID, BB.PRICE, AA.IMAGE_WEB FROM TV_CONTENT AA, (SELECT AAA.SMS_CODE, AAA.NAME, AAA.NAME_ENGLISH, TO_CHAR(AAA.BEGIND,'YYYY-MM-DD HH24:MI:SS') AS BEGINDATE, TO_CHAR(AAA.ENDD,'YYYY-MM-DD HH24:MI:SS') AS ENDDATE, TO_CHAR(AAA.BEGIND,'YYYY-MM-DD') AS INDATE, AAA.PRODUCT_ID, AAA.ID, AAA.PRICE FROM (SELECT * FROM (SELECT SMS_CODE, NAME, NAME_ENGLISH,  TO_DATE(IN_DATE || START_TIME, 'YYYYMMDDHH24MISS') AS BEGIND, TO_DATE(IN_DATE || STOP_TIME, 'YYYYMMDDHH24MISS') AS ENDD, PRODUCT_ID, ID, PRICE FROM TV_PROGRAM WHERE IN_DATE >= '{0}' AND PRODUCT_ID IN (SELECT PRODUCT_ID FROM PRODUCT_CATALOG_VOD WHERE PRODUCT_ID NOT IN (65,58,59,60)) AND IS_DELETED='N') FFF WHERE FFF.BEGIND >SYSDATE AND LOWER(FFF.NAME) LIKE LOWER('%{1}%') OR LOWER(FFF.NAME_ENGLISH) LIKE LOWER('%{1}%')) AAA WHERE AAA.BEGIND>SYSDATE) BB WHERE AA.ID = BB.ID ORDER BY BB.BEGINDATE ASC", begin, _searchValue);
            return qry;
        }
        public static string _getContentDetial(string contentId)
        {
            string qry = string.Format(@"SELECT ID, NAME_MON, NAME_ENG, CONTENT_DESCR, PRICE, IMAGE_WEB, YEAR, DIRECTOR, TRAILER, DURATION_TIME FROM TV_CONTENT WHERE ID={0} AND DELETED ='N'", contentId);
            return qry;
        }
        public static string _getActors(string contentId)
        {
            string qry = string.Format(@"SELECT LISTAGG(ACTOR_NAME, ', ') WITHIN GROUP (ORDER BY ACTOR_NAME ASC) AS ACTORS FROM ACTOR_LIST WHERE IS_ACTIVE='Y' AND ID IN (SELECT ACTOR_ID FROM TV_CONTENT_ACTOR WHERE CONTENT_ID={0})", contentId);
            return qry;
        }
        public static string _getGenres(string contentId)
        {
            string qry = string.Format(@"SELECT LISTAGG (NAME_MON, ', ') WITHIN GROUP (ORDER BY NAME_MON ASC) AS GENRES FROM LIST_OF_GENRES WHERE ID IN (SELECT GENRES_ID FROM TV_CONTENT_GENRES WHERE CONTENT_ID={0})", contentId);
            return qry;
        }
        public static string _getMainProducts(string productIds)
        {
            string qry = string.Format(@"SELECT PRODUCT_NAME_MON, PRODUCT_ID, SMS_CODE, UPGRADE_PRODUCTS FROM PRODUCT_CATALOG WHERE PRODUCT_ID IN ({0}) AND IS_ACTIVE='A' ORDER BY ORDERING ASC", productIds);
            return qry;
        }
        public static string _getMainProductPrices(string productId)
        {
            string qry = string.Format(@"SELECT PRODUCT_ID, MONTH, PRICE FROM PRODUCT_PRICE WHERE PRODUCT_ID = {0} AND COMPANY_ID=100 ORDER BY MONTH ASC", productId);
            return qry;
        }
        public static string _getSalesCenters(string _are, string _serv, string _type)
        {
            string qry = string.Format(@"SELECT BRANCH_ID, BRANCH_NAME, ADDRESS, BRANCH_TYPE, LATITUDE, LONGTITUDE, IMAGE, FULLWORKING_DAYS, FULLWORKING_OPENTIME, FULLWORKING_CLOSETIME, HALFWORKING_DAYS, HALFWORKING_OPENTIME, HALFWORKING_CLOSETIME, ZOOM FROM SALES_CENTER WHERE IS_DELETED='N' AND AREA= LOWER('{0}') AND SERVICE LIKE LOWER('%{1}%') AND BRANCH_TYPE LIKE LOWER('%{2}%')",_are, _serv, _type);
            return qry;
        }
        public static string getSalesCenterAre()
        {
            string qry = string.Format(@"SELECT AREA_NAME, AREA_CODE FROM SALES_CENTER_AREA ORDER BY ORDER_NO ASC");
            return qry;
        }
        public static string getSalesCenterService()
        {
            string qry = string.Format(@"SELECT SERVICE_NAME, SERVICE_CODE FROM SALES_CENTER_SERVICE ORDER BY ORDER_NO ASC");
            return qry;
        }
        public static string getSalesCenterType()
        {
            string qry = string.Format(@"SELECT TYPE_NAME, TYPE_CODE FROM SALES_CENTER_TYPE ORDER BY ORDER_NO ASC");
            return qry;
        }
        public static string getAppNotification(string cardNo)
        {
            string qry = string.Format(@"SELECT NOTI_ID, NOTIFICATION_NAME, NOTIFICATION_TEXT, NOTIFICATION_IMG_URL, TO_CHAR(CREATED_DATE,'YYYY-MM-DD') AS NOTIDATE, CARD_NO FROM APP_NOTIFICATION WHERE IS_DELETED='N' AND CARD_NO IN ('0','{0}') ORDER BY NOTI_ID DESC", cardNo);
            return qry;
        }
        public static string getReadNotification(string cardNo)
        {
            string qry = string.Format("SELECT NOTI_ID, READ_USER FROM APP_NOTIFICATION_ISREAD WHERE READ_USER = '{0}'", cardNo);
            return qry;
        }
        public static string setReadList(string notifId, string userId)
        {
            string qry = string.Format(@"INSERT INTO APP_NOTIFICATION_ISREAD (NOTI_ID, READ_USER) VALUES ('{0}', '{1}')", notifId, userId);
            return qry;
        }
        public static string getUnReadedCount(string userId)
        {
            string qry = string.Format(@"SELECT COUNT(0) AS COUNTN FROM APP_NOTIFICATION WHERE CARD_NO IN ('0', '{0}') AND NOTI_ID NOT IN (SELECT NOTI_ID FROM APP_NOTIFICATION_ISREAD WHERE READ_USER ='{0}')", userId);
            return qry;
        }
        public static string saveNotification(string title, string body, string expDate, string cardno)
        {
            string qry = string.Format(@"INSERT INTO APP_NOTIFICATION (NOTIFICATION_NAME, NOTIFICATION_TEXT, EXPIRE_DATE, CARD_NO) VALUES ('{0}', '{1}', TO_DATE('{2}', 'YYYY-MM-DD HH24:MI:SS'), '{3}')", title, body, expDate, cardno);
            return qry;
        }
        public static string vatHistoryList(string cardNo)
        {
            string qry = string.Format(@"select AA.CARD_NO, AA.AMOUNT, AA.BILLID, AA.LOTTERYNO, AA.RESULTDATE, BB.QRDATA, AA.USERCODE from (select CARD_NO, sum(AMOUNT) as amount, BILLID, LOTTERYNO, RESULTDATE, USERCODE, CREATEDATE from MTA_TRANSACTION_LIST where CARD_NO='{0}' and ISORGANIZATION ='N' and ISRETURN='N' and to_char(CREATEDATE, 'yyyy')='{1}' group by CARD_NO, BILLID, LOTTERYNO, RESULTDATE, USERCODE, CREATEDATE) aa, 
MTA_TRANSACTION_ADDITIONAL bb where AA.BILLID = BB.BILL_ID ORDER BY AA.CREATEDATE DESC", cardNo, DateTime.Today.ToString("yyyy"));
            return qry;
        }

        public static string _getVODComing()
        {
            //string qry = string.Format(@"SELECT CONTENT_NAME, CONTENT_IMG_URL FROM APP_VOD_COMING WHERE IS_DELETED='N' AND EXPIRE_DATE>=SYSDATE");
            string qry = string.Format(@"SELECT NAME_MON AS CONTENT_NAME, IMAGE_WEB AS CONTENT_IMG_URL  FROM TV_CONTENT WHERE ID IN (SELECT DISTINCT ID FROM TV_PROGRAM WHERE PRODUCT_ID = 65 AND IN_DATE = TO_CHAR(SYSDATE, 'YYYYMMDD') AND IS_DELETED = 'N')");
            return qry;
        }
        public static string _searchARVOD(string _contentId)
        {
            string qry = string.Format(@"SELECT NAME_ENG, NAME_MON FROM TV_CONTENT WHERE DELETED = 'N' AND ID ={0}", _contentId);
            return qry;
        }
        public static string _getNewPromotion()
        {
            string qry = string.Format(@"SELECT PROMOTION_NAME, PROMOTION_TEXT, PROMOTION_IMG, DETIAL_IMG, PROMOTION_ID FROM APP_NEW_PROMOTION WHERE IS_DELETED='N' AND EXPIRE_DATE >= SYSDATE ORDER BY PROMOTION_ID DESC");
            return qry;
        }
        public static string _getNewPromotionDetial(string promoId)
        {
            string qry = string.Format(@"SELECT PROMOTION_ID, DETIAL_POSTER_URL FROM APP_NEW_PROMOTION_DETIAL WHERE PROMOTION_ID={0}", promoId);
            return qry;
        }
        public static string _getupProdState(string cardNo, string prodId)
        {
            string qry = string.Format(@"SELECT CARD_NUMBER, TO_CHAR(END_DATE, 'YYYY-MM-DD HH24:MI:SS') AS ENDDATE FROM ACCOUNT_SERVICE WHERE CARD_NUMBER='{0}' AND PRODUCT_ID={1} AND END_DATE >=SYSDATE", cardNo, prodId);
            return qry;
        }
        public static string _getOrderedMovie(string cardNo, string contentId)
        {
            string qry = string.Format(@"SELECT CARD_NO, PRODUCT_ID, CONTENT_ID FROM CAS_SENT_NVOD WHERE CARD_NO='{0}' AND CONTENT_ID={1} AND ENDDATE >= SYSDATE", cardNo, contentId);
            return qry;
        }
        public static string _getAccountChargeManual()
        {
            string qry = string.Format(@"SELECT NAME, MANUAL_TEXT,IMAGE_URL FROM APP_ACCOUNT_CHARGE_MANUAL WHERE IS_DELETED='N'");
            return qry;
        }
        public static string _getAtennaInstallationManual()
        {
            string qry = string.Format(@"SELECT IMAGE_URL FROM APP_ANTENN_INSTALLATION_MANUAL WHERE IS_DELETED='N' ORDER BY ORDERING_NO ASC");
            return qry;
        }
        public static string _getAtennaVideoManual()
        {
            string qry = string.Format(@"SELECT VIDEO_ID, VIDEO_NAME FROM APP_ANTENN_VIDEO_MANUAL WHERE IS_DELETED='N' ORDER BY ORDERING_NO ASC");
            return qry;
        }
        public static string _getRefNvod(string _cardNo)
        {
            string qry = string.Format(@"SELECT AA.CARD_NO, AA.PRODUCT_ID, AA.ENDTIME, AA.CONTENT_ID, BB.NAME_MON FROM (SELECT CARD_NO, PRODUCT_ID, TO_CHAR(ENDDATE, 'yyyy-mm-dd hh24:mi:ss') ENDTIME, CONTENT_ID FROM CAS_SENT_NVOD WHERE CARD_NO='{0}' AND ENDDATE>SYSDATE AND ISSUCCESS !='N') AA, TV_CONTENT BB WHERE AA.CONTENT_ID = BB.ID", _cardNo);
            return qry;
        }
        public static string _getRefProduct(string _cardNo)
        {
            string qry = string.Format(@"SELECT BB.PRODUCT_NAME_MON, AA.PRODUCT_ID, AA.ENDTIME FROM (SELECT PRODUCT_ID, TO_CHAR(END_DATE, 'YYYY-MM-DD HH24:MI:SS') ENDTIME FROM ACCOUNT_SERVICE WHERE CARD_NUMBER='{0}' AND END_DATE > SYSDATE) AA, PRODUCT_CATALOG BB WHERE BB.PRODUCT_ID = AA.PRODUCT_ID", _cardNo);
            return qry;
        }
        public static string _getRefLive(string _cardNo)
        {
            string qry = string.Format(@"SELECT BB.CONTENT_NAME, AA.EVENT_ID, TO_CHAR(AA.ENDD, 'YYYY-MM-DD HH24:MI:SS') ENDTIME FROM (SELECT EVENT_ID, TO_DATE(END_TIME, 'YYYYMMDDHH24MISS') ENDD FROM D_PPV_EVENT WHERE CARD_NO='{0}' AND RESULT_YN != 'N' AND TO_DATE(END_TIME,'YYYYMMDDHH24MISS') >SYSDATE) AA, D_PPV_EVENT_ID_NAME BB WHERE BB.ID = AA.EVENT_ID", _cardNo);
            return qry;
        }
        public static string checkCard(string sn)
        {
            string qry = string.Format("SELECT SUBSCRIBER_FNAME, CARD_NO, IS_PREPAID FROM T_DISH_CUSTOM WHERE SUBSCRIBER_STATUS=1 AND CARD_NO='{0}'", sn);
            return qry;
        }
        public static string checkZuslanProduct(string sn)
        {
            string qry = string.Format("SELECT CARD_NUMBER, END_DATE FROM ACCOUNT_SERVICE WHERE CARD_NUMBER={0} AND PRODUCT_ID=86 AND END_DATE > SYSDATE", sn);
            return qry;
        }
        public static string setQpayInvoice(string invoiceNo)
        {
            string qry = string.Format("INSERT INTO QPAY_INVOICE_STATUS (INVOICE_NO) VALUES ('{0}')", invoiceNo);
            return qry;
        }
        public static string getQpayInvoice(string invoiceNo)
        {
            string qry = string.Format("SELECT INVOICE_NO, STATUS FROM QPAY_INVOICE_STATUS WHERE INVOICE_NO = '{0}'", invoiceNo);
            return qry;
        }
    }
}

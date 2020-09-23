using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.DBControl
{
    public class chatbotQry
    {
        public static string _getCardNo(string admin)
        {
            string qry = string.Format(@"SELECT CARD_NO, PHONE_NO FROM ADMIN_NUMBER WHERE PHONE_NO='{0}'", admin);
            return qry;
        }
        public static string _sendSMS(string admin, string smsVal)
        {
            string qry = string.Format(@"INSERT INTO T_DISH_SENT_SMS (PHONE, CONTENT, SOURCE1, SOURCE2, TYPE) VALUES ('{0}', '{1}', 1, 1, '2')", admin, smsVal);
            return qry;
        }
        public static string _getActiveProducts(string admin)
        {
            string qry = string.Format(@"SELECT AA.CARD_NUMBER, AA.PRODUCT_ID, AA.ENDD, BB.PRODUCT_NAME_MON FROM (SELECT CARD_NUMBER, PRODUCT_ID, TO_CHAR(END_DATE, 'YYYY-MM-DD HH24:MI:SS') ENDD  FROM ACCOUNT_SERVICE WHERE CARD_NUMBER IN (SELECT CARD_NO FROM ADMIN_NUMBER WHERE PHONE_NO = '{0}' AND ROWNUM <=1) AND END_DATE>SYSDATE AND PRODUCT_ID IN (27,28,29,73)) AA, PRODUCT_CATALOG BB WHERE AA.PRODUCT_ID = BB.PRODUCT_ID", admin);
            return qry;
        }
        public static string _addNewPromo(string _promoName, string _promoText, string _promoPoster, string _expireDate)
        {
            string qry = string.Format(@"INSERT INTO APP_NEW_PROMOTION (PROMOTION_NAME, PROMOTION_TEXT, PROMOTION_IMG, EXPIRE_DATE) VALUES ('{0}', '{1}', '{2}', TO_DATE('{3}', 'YYYY-MM-DD HH24:MI:SS')) RETURNING PROMOTION_ID INTO :CONTENT_ID", _promoName, _promoText, _promoPoster, _expireDate);
            return qry;
        }
        public static string _addNewPromoDetial(string _promoId, string _promoDetialImg)
        {
            string qry = string.Format(@"INSERT INTO APP_NEW_PROMOTION_DETIAL(PROMOTION_ID, DETIAL_POSTER_URL) VALUES ({0}, '{1}')", _promoId, _promoDetialImg);
            return qry;
        }

        public static string _saveFbId(string fbid, string adminNo)
        {
            string qry = string.Format(@"UPDATE T_DISH_CUSTOM SET FB_ID = '{0}' WHERE CARD_NO IN (SELECT CARD_NO FROM ADMIN_NUMBER WHERE PHONE_NO = '{1}')", fbid, adminNo);
            return qry;
        }
    }
}

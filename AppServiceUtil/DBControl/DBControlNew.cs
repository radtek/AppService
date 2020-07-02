using AppServiceUtil.Utils;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.DBControl
{
    public class DBControlNew
    {
        OracleConnection conn;
        private string TAG = "DBControl";
        public DBControlNew()
        {
            string dbip1 = ConfigurationManager.AppSettings["dbIP1"];
            string dbip2 = ConfigurationManager.AppSettings["dbIP2"];
            string dbIns = ConfigurationManager.AppSettings["dbInstance"];
            string conStr = string.Format(@"Data Source=(DESCRIPTION=(LOAD_BALANCE=on)(FAILOVER=off)(ADDRESS_LIST=(SOURCE_ROUTE=yes)(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521))(ADDRESS=(PROTOCOL=TCP)(HOST={1})(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME={2}))); User Id=uni_dish;Password=Qk6cGhLSWmpL;", dbip1, dbip2, dbIns);
            conn = new OracleConnection(conStr);
        }
        public bool iOpen()
        {
            bool retVal = false;
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                    retVal = true;
                }
                else
                {
                    retVal = true;
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                LogWriter._error(TAG, ex.ToString());
            }
            return retVal;
        }
        public bool iClose()
        {
            bool retVal = false;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    retVal = true;
                }
                else
                {
                    conn.Close();
                    retVal = true;
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                LogWriter._error(TAG, ex.ToString());
            }
            return retVal;
        }
        public bool idbStatOK()
        {
            bool res = false;
            if (iOpen())
            {
                if (iClose())
                {
                    res = true;
                }
            }
            return res;
        }
        public bool idbCommand(string qry)
        {
            bool res = false;
            try
            {
                iOpen();
                OracleCommand cmd = new OracleCommand(qry, conn);
                int stt = cmd.ExecuteNonQuery();
                iClose();
                res = true;
            }
            catch (Exception ex)
            {
                LogWriter._error(TAG, ex.ToString());
                res = false;
            }
            return res;
        }
        public DataTable getTable(string qry)
        {
            DataTable dt = new DataTable();
            try
            {
                OracleDataAdapter da = new OracleDataAdapter(qry, conn);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                LogWriter._error(TAG, string.Format(@"SQL: {0}, Exception: {1}", qry, ex.ToString()));
                dt.Clear();
            }
            return dt;
        }
        public bool registerToken(string userId, string userPhone, string token, string expDate)
        {
            bool retValue = false;

            try
            {
                iOpen();
                OracleCommand ocm = new OracleCommand("UNI_DISH.PKD_MPP_TABLET.CREATETOKEN", conn);
                ocm.CommandType = CommandType.StoredProcedure;
                ocm.Parameters.Add("userId", OracleDbType.Varchar2, 30).Value = userId;
                ocm.Parameters.Add("userPhone", OracleDbType.Varchar2, 30).Value = userPhone;
                ocm.Parameters.Add("accessToken", OracleDbType.Varchar2, 3000).Value = token;
                ocm.Parameters.Add("expiredate", OracleDbType.Varchar2, 30).Value = expDate;
                ocm.Parameters.Add("pOut", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.Parameters.Add("pOutMessage", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.ExecuteNonQuery();
                iClose();
                string result = string.Empty;
                result = ocm.Parameters["pOut"].Value.ToString();
                if (result == "000")
                {
                    retValue = true;
                }
                else
                {
                    retValue = false;

                }
            }
            catch (Exception ex)
            {
                LogWriter._error(TAG, ex.ToString());
                retValue = false;
            }
            return retValue;
        }
        public bool tabletCheckToken(string _token, out string _userId, out string _userPhone)
        {
            bool res = false;
            _userId = string.Empty;
            _userPhone = string.Empty;
            try
            {
                DataTable dt = getTable(tabletQuery.getLoggedUser(_token));
                if(dt.Rows.Count != 0)
                {
                    _userId = dt.Rows[0]["USERID"].ToString();
                    _userPhone = dt.Rows[0]["USER_PHONE"].ToString();
                    res = true;
                }
            }
            catch(Exception ex)
            {
                LogWriter._error(TAG, string.Format(@"Token: [{0}], Exception: [{1}]", _token, ex.ToString()));
            }
            return res;
        }
        public bool refreshService(string _cardNo, string _user)
        {
            bool res = false;
            try
            {
                iOpen();
                OracleCommand ocm = new OracleCommand("UNI_DISH.PKD_MPP_TABLET.REFRESHSERVICE", conn);
                ocm.CommandType = CommandType.StoredProcedure;
                ocm.Parameters.Add("cardNo", OracleDbType.Varchar2, 30).Value = _cardNo;
                ocm.Parameters.Add("userId", OracleDbType.Varchar2, 30).Value = _user;
                ocm.Parameters.Add("pOut", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.Parameters.Add("pOutMessage", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.ExecuteNonQuery();
                iClose();
                string result = string.Empty;
                result = ocm.Parameters["pOut"].Value.ToString();
                if (result == "000")
                {
                    res = true;
                }
                else
                {
                    res = false;

                }
            }
            catch(Exception ex)
            {
                LogWriter._error(TAG, ex.ToString());
                res = false;
            }
            return res;
        }
    }
}

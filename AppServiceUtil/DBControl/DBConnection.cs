using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using AppServiceUtil.Utils;

namespace AppServiceUtil.DBControl
{
    public class DBConnection
    {
        private string TAG = "DBCONNECTION";
        OracleConnection conn;
        public DBConnection()
        {
            string dbip1 = ConfigurationManager.AppSettings["dbIP1"];
            string dbip2 = ConfigurationManager.AppSettings["dbIP2"];
            string dbIns = ConfigurationManager.AppSettings["dbInstance"];
            string constr = string.Format(@"Data Source=(DESCRIPTION=(LOAD_BALANCE=on)(FAILOVER=off)(ADDRESS_LIST=(SOURCE_ROUTE=yes)(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521))(ADDRESS=(PROTOCOL=TCP)(HOST={1})(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME={2}))); User Id=uni_dish;Password=Qk6cGhLSWmpL;", dbip1, dbip2, dbIns);
            conn = new OracleConnection(constr);
        }
        public string iOpen()
        {
            string retVal = string.Empty;
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                    retVal = "0000";
                }
                else
                {
                    retVal = "0000";
                }
            }
            catch (Exception ex)
            {
                retVal = string.Format("FFFFx[{0}]", ex.Message);
            }
            return retVal;
        }
        public string iClose()
        {
            string retVal = string.Empty;
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    retVal = "0000";
                }
                else
                {
                    conn.Close();
                    retVal = "0000";
                }
            }
            catch (Exception ex)
            {
                retVal = string.Format("FFFFx[{0}]", ex.Message);
            }
            return retVal;
        }
        public bool idbCheck(out string sttCode)
        {
            sttCode = string.Empty;
            bool retVal = false;
            try
            {
                string stateCode = iOpen();
                if (stateCode == "0000")
                {
                    string cclose = iClose();
                    if (cclose == "0000")
                    {
                        sttCode = string.Empty;
                        retVal = true;
                    }
                    else
                    {
                        retVal = false;
                        sttCode = cclose;
                    }
                }
                else
                {
                    retVal = false;
                    sttCode = stateCode;
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                sttCode = string.Format("FFFFx[{0}]", ex.Message);
            }
            return retVal;
        }
        public string iDBCommand(string qry)
        {
            string retVal = string.Empty;
            try
            {
                string op = iOpen();
                OracleCommand cmd = new OracleCommand(qry, conn);
                int stt = cmd.ExecuteNonQuery();
                string cl = iClose();
                retVal = string.Format(@"{0}{1}", stt, "000");
            }
            catch (Exception ex)
            {
                retVal = string.Format("FFFFx[{0}]", ex.Message);
            }
            return retVal;
        }
        public bool iDBCommandRetID(string qry, out int retID)
        {
            bool retVal = false;
            retID = 0;
            try
            {
                string op = iOpen();
                OracleCommand cmd = new OracleCommand(qry, conn);
                cmd.Parameters.Clear();
                cmd.Parameters.Add("CONTENT_ID", OracleDbType.Int32, 10).Direction = ParameterDirection.Output;
                int stt = cmd.ExecuteNonQuery();
                retID = int.Parse(cmd.Parameters["CONTENT_ID"].Value.ToString());
                string cl = iClose();
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                LogWriter._error(TAG, ex.ToString());
            }
            return retVal;
        }
        public DataTable getTable(string qry)
        {
            DataTable dt = new DataTable();
            try
            {
                if (qry.Length > 0)
                {
                    OracleDataAdapter da = new OracleDataAdapter(qry, conn);
                    da.Fill(dt);
                }
                else
                {
                    dt.Clear();
                }
            }
            catch (Exception ex)
            {
                dt.Clear();
            }
            return dt;
        }

        public bool registerToken(string _cardNo, string _phoneNo, string _token, string _refreshToken, string _ip, string _expDate)
        {
            bool retValue = false;
            
            try
            {
                iOpen();
                OracleCommand ocm = new OracleCommand("UNI_DISH.PK_NEWAPP_SERV.REGISTER_TOKEN", conn);
                ocm.CommandType = CommandType.StoredProcedure;
                ocm.Parameters.Add("iCardNo", OracleDbType.Varchar2, 30).Value = _cardNo;
                ocm.Parameters.Add("iPhoneNo", OracleDbType.Varchar2, 30).Value = _phoneNo;
                ocm.Parameters.Add("iToken", OracleDbType.Varchar2, 3000).Value = _token;
                ocm.Parameters.Add("iRefreshToken", OracleDbType.Varchar2, 3000).Value = _refreshToken;
                ocm.Parameters.Add("iIp", OracleDbType.Varchar2, 30).Value = _ip;
                ocm.Parameters.Add("iExpDate", OracleDbType.Varchar2, 30).Value = _expDate;
                ocm.Parameters.Add("pOutMsg", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.ExecuteNonQuery();
                iClose();
                string result = string.Empty;
                result = ocm.Parameters["pOutMsg"].Value.ToString();
                if (result == "0000")
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
        public bool checkToken(string _token, out string _cardNo, out string _phoneNo)
        {
            bool retValue = false;
            _cardNo = string.Empty;
            _phoneNo = string.Empty;
            try
            {
                iOpen();
                OracleCommand ocm = new OracleCommand("UNI_DISH.PK_NEWAPP_SERV.CHECK_TOKEN", conn);
                ocm.CommandType = CommandType.StoredProcedure;
                ocm.Parameters.Add("iToken", OracleDbType.Varchar2, 3000).Value = _token;
                ocm.Parameters.Add("pCardNo", OracleDbType.Varchar2, 30).Direction = ParameterDirection.Output;
                ocm.Parameters.Add("pPhoneNo", OracleDbType.Varchar2, 30).Direction = ParameterDirection.Output;
                ocm.Parameters.Add("pOutMsg", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.ExecuteNonQuery();
                iClose();
                string result = string.Empty;
                result = ocm.Parameters["pOutMsg"].Value.ToString();
                if (result == "0000")
                {
                    _cardNo = ocm.Parameters["pCardNo"].Value.ToString();
                    _phoneNo = ocm.Parameters["pPhoneNo"].Value.ToString();
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
        public bool refreshToken(string _refreshToken, string _cardNo, string _newExpDate, out string _accessToken)
        {
            bool retValue = false;
            _accessToken = string.Empty;
            try
            {
                iOpen();
                OracleCommand ocm = new OracleCommand("UNI_DISH.PK_NEWAPP_SERV.REFRESH_TOKEN", conn);
                ocm.CommandType = CommandType.StoredProcedure;
                ocm.Parameters.Add("iRefToken", OracleDbType.Varchar2, 3000).Value = _refreshToken;
                ocm.Parameters.Add("iCardNo", OracleDbType.Varchar2, 30).Value = _cardNo;
                ocm.Parameters.Add("iExpDate", OracleDbType.Varchar2, 30).Value = _newExpDate;
                ocm.Parameters.Add("pAccessToken", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                ocm.Parameters.Add("pOutMsg", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.ExecuteNonQuery();
                iClose();
                string result = string.Empty;
                result = ocm.Parameters["pOutMsg"].Value.ToString();
                if (result == "0000")
                {
                    _accessToken = ocm.Parameters["pAccessToken"].Value.ToString();
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
        public bool chargeProductByCounter(string _cardNo, string _prodId, string _month, string _admin, out string _resEng, out string _resMon, out string _resCry)
        {
            bool retValue = false;
            _resEng = string.Empty;
            _resMon = string.Empty;
            _resCry = string.Empty;
            try
            {
                iOpen();
                OracleCommand ocm = new OracleCommand("UNI_DISH.PK_NEWAPP_SERV.PAYTV", conn);
                ocm.CommandType = CommandType.StoredProcedure;
                ocm.Parameters.Add("vcard_no", OracleDbType.Varchar2, 30).Value = _cardNo;
                ocm.Parameters.Add("vproduct_id", OracleDbType.Varchar2, 30).Value = _prodId;
                ocm.Parameters.Add("vmonth", OracleDbType.Varchar2, 30).Value = _month;
                ocm.Parameters.Add("vUsername", OracleDbType.Varchar2, 30).Value = _admin;
                ocm.Parameters.Add("isSuccess", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.Parameters.Add("outMsgENG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.Parameters.Add("outMsgMON", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.Parameters.Add("outMsgCRY", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.ExecuteNonQuery();
                iClose();
                string result = string.Empty;
                result = ocm.Parameters["isSuccess"].Value.ToString();
                if (result == "000")
                {
                    
                    retValue = true;
                }
                else
                {
                    retValue = false;

                }
                _resEng = ocm.Parameters["outMsgENG"].Value.ToString();
                _resMon = ocm.Parameters["outMsgMON"].Value.ToString();
                _resCry = ocm.Parameters["outMsgCRY"].Value.ToString();
                //LogWriter._error(TAG, string.Format("RESULT: [{0}], MESG: [{1}]", result, _resMon));
            }
            catch (Exception ex)
            {
                LogWriter._error(TAG, ex.ToString());
                retValue = false;
            }
            return retValue;
        }
        public bool addNvodByCounter(string _cardNo, string _admin, string _indDate, string _smsCode, string _prodId, out string _resEng, out string _resMon, out string _resCry)
        {
            bool retValue = false;
            _resEng = string.Empty;
            _resMon = string.Empty;
            _resCry = string.Empty;
            try
            {
                iOpen();
                OracleCommand ocm = new OracleCommand("UNI_DISH.PK_NEWAPP_SERV.ADDNVOD_MOBILE", conn);
                ocm.CommandType = CommandType.StoredProcedure;
                ocm.Parameters.Add("vcard_no", OracleDbType.Varchar2, 30).Value = _cardNo;
                ocm.Parameters.Add("vAdmin", OracleDbType.Varchar2, 30).Value = _admin;
                ocm.Parameters.Add("inDate", OracleDbType.Varchar2, 30).Value = _indDate;
                ocm.Parameters.Add("smscode", OracleDbType.Varchar2, 30).Value = _smsCode;
                ocm.Parameters.Add("vproduct_id", OracleDbType.Varchar2, 30).Value = _prodId;
                ocm.Parameters.Add("isSuccess", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.Parameters.Add("outMsgENG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.Parameters.Add("outMsgMON", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.Parameters.Add("outMsgCRY", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.ExecuteNonQuery();
                iClose();
                string result = string.Empty;
                result = ocm.Parameters["isSuccess"].Value.ToString();
                if (result == "000")
                {
                    retValue = true;
                }
                else
                {
                    retValue = false;
                }
                _resEng = ocm.Parameters["outMsgENG"].Value.ToString();
                _resMon = ocm.Parameters["outMsgMON"].Value.ToString();
                _resCry = ocm.Parameters["outMsgCRY"].Value.ToString();
            }
            catch (Exception ex)
            {
                LogWriter._error(TAG, ex.ToString());
                retValue = false;
            }
            return retValue;
        }
        public bool callARProcedure(string _cardNo, string _movieId, string _beginDate, string _endDate, string _loto, string _userName, out string msgENG, out string msgMON, out string msgCRY)
        {
            bool retValue = false;
            msgENG = string.Empty;
            msgMON = string.Empty;
            msgCRY = string.Empty;
            try
            {
                iOpen();
                OracleCommand ocm = new OracleCommand("UNI_DISH.ARVODAPI", conn);
                ocm.CommandType = CommandType.StoredProcedure;
                ocm.Parameters.Add("CardNo", OracleDbType.Varchar2, 30).Value = _cardNo;
                ocm.Parameters.Add("MovieID", OracleDbType.Varchar2, 30).Value = _movieId;
                ocm.Parameters.Add("Startdate", OracleDbType.Varchar2, 30).Value = _beginDate;
                ocm.Parameters.Add("Enddate", OracleDbType.Varchar2, 30).Value = _endDate;
                ocm.Parameters.Add("Loto", OracleDbType.Varchar2, 30).Value = _loto;
                ocm.Parameters.Add("ProID", OracleDbType.Varchar2, 30).Value = "65";
                ocm.Parameters.Add("Username", OracleDbType.Varchar2, 30).Value = _userName;
                ocm.Parameters.Add("orderType", OracleDbType.Int32, 30).Value = 1;
                ocm.Parameters.Add("checkSum", OracleDbType.Varchar2, 30).Value = null;
                ocm.Parameters.Add("isSuccess", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                ocm.Parameters.Add("outMsgENG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.Parameters.Add("outMsgMON", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.Parameters.Add("outMsgCRY", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.ExecuteNonQuery();
                iClose();
                string result = string.Empty;
                result = ocm.Parameters["isSuccess"].Value.ToString();
                msgENG = ocm.Parameters["outMsgENG"].Value.ToString();
                msgMON = ocm.Parameters["outMsgMON"].Value.ToString();
                msgCRY = ocm.Parameters["outMsgCRY"].Value.ToString();
                if (result == "0000")
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
                msgENG = "INTERNAL ERROR";
                msgMON = "ДОТООД АЛДАА";
                msgCRY = "DOTOOD ALDAA";
                retValue = false;
            }
            return retValue;
        }
        public bool upgradeProductByCounter(string _cardNo, string _admin, string _oldProductId, string _newProductId, string _amount, string _month, out string _resEng, out string _resMon, out string _resCry)
        {
            bool retValue = false;
            _resEng = string.Empty;
            _resMon = string.Empty;
            _resCry = string.Empty;
            try
            {
                iOpen();
                OracleCommand ocm = new OracleCommand("UNI_DISH.PK_NEWAPP_SERV.UPGRADE_PRODUCT", conn);
                ocm.CommandType = CommandType.StoredProcedure;
                ocm.Parameters.Add("pcard_no", OracleDbType.Varchar2, 30).Value = _cardNo;
                ocm.Parameters.Add("pOld_proId", OracleDbType.Varchar2, 30).Value = _oldProductId;
                ocm.Parameters.Add("pNew_proId", OracleDbType.Varchar2, 30).Value = _newProductId;
                ocm.Parameters.Add("pAmount", OracleDbType.Int32, 30).Value = Convert.ToInt32(_amount);
                ocm.Parameters.Add("pMonth", OracleDbType.Int32, 30).Value = Convert.ToInt32(_month);
                ocm.Parameters.Add("pAdmin", OracleDbType.Varchar2, 30).Value = _admin;
                ocm.Parameters.Add("isSuccess", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.Parameters.Add("outMsgENG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.Parameters.Add("outMsgMON", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.Parameters.Add("outMsgCRY", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.ExecuteNonQuery();
                iClose();
                string result = string.Empty;
                result = ocm.Parameters["isSuccess"].Value.ToString();
                if (result == "000")
                {
                    retValue = true;
                }
                else
                {
                    retValue = false;
                }
                _resEng = ocm.Parameters["outMsgENG"].Value.ToString();
                _resMon = ocm.Parameters["outMsgMON"].Value.ToString();
                _resCry = ocm.Parameters["outMsgCRY"].Value.ToString();
            }
            catch (Exception ex)
            {
                LogWriter._error(TAG, ex.ToString());
                retValue = false;
            }
            return retValue;
        }
        public bool registerClientToken(string _cardNo, string _phoneNo, string _token)
        {
            bool retValue = false;

            try
            {
                iOpen();
                OracleCommand ocm = new OracleCommand("UNI_DISH.PK_NEWAPP_SERV.REGISTER_CLIENT_TOKEN", conn);
                ocm.CommandType = CommandType.StoredProcedure;
                ocm.Parameters.Add("iCardNo", OracleDbType.Varchar2, 30).Value = _cardNo;
                ocm.Parameters.Add("iPhoneNo", OracleDbType.Varchar2, 30).Value = _phoneNo;
                ocm.Parameters.Add("iToken", OracleDbType.Varchar2, 3000).Value = _token;
                ocm.Parameters.Add("pOutMsg", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.ExecuteNonQuery();
                iClose();
                string result = string.Empty;
                result = ocm.Parameters["pOutMsg"].Value.ToString();
                if (result == "0000")
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
        public bool refreshSC(string _adminNo)
        {
            bool retValue = false;
            try
            {
                iOpen();
                OracleCommand ocm = new OracleCommand("UNI_DISH.AUTO_REFRESH", conn);
                ocm.CommandType = CommandType.StoredProcedure;
                ocm.Parameters.Add("pphone_no", OracleDbType.Varchar2, 30).Value = _adminNo;
                ocm.Parameters.Add("pSource", OracleDbType.Int32, 30).Value = 4;
                ocm.Parameters.Add("POUTMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.ExecuteNonQuery();
                iClose();
                string result = string.Empty;
                result = ocm.Parameters["pOutMsg"].Value.ToString();
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
        public bool chargeProduct(string _prodId, string _month, string _userName, string _amount, string _desc, string _cardNo, string channel)
        {
            bool retVal = false;
            try
            {
                iOpen();
                OracleCommand ocm = new OracleCommand("PKD_CHARGE_PRODUCT.chargeProduct_month_bank", conn);
                ocm.CommandType = CommandType.StoredProcedure;
                ocm.Parameters.Add("pproductId", OracleDbType.Int32, 30).Value = Convert.ToInt32(_prodId);
                ocm.Parameters.Add("pmonth", OracleDbType.Int32, 30).Value = Convert.ToInt32(_month);
                ocm.Parameters.Add("puser", OracleDbType.Varchar2, 100).Value = _userName;
                ocm.Parameters.Add("pbranch", OracleDbType.Varchar2, 30).Value = "286";
                ocm.Parameters.Add("pamount", OracleDbType.Decimal, 30).Value = Convert.ToDecimal(_amount);
                ocm.Parameters.Add("pchannelType", OracleDbType.Varchar2, 30).Value = channel;
                ocm.Parameters.Add("pcallType", OracleDbType.Int32, 30).Value = 9;
                ocm.Parameters.Add("pdescription", OracleDbType.Varchar2, 500).Value = _desc;
                ocm.Parameters.Add("pcardNumber", OracleDbType.Varchar2, 30).Value = _cardNo;
                ocm.Parameters.Add("pproTypeId", OracleDbType.Int32, 30).Value = 3001;
                ocm.Parameters.Add("ptransgroupid", OracleDbType.Varchar2, 30).Value = string.Empty;
                ocm.Parameters.Add("pbankid", OracleDbType.Varchar2, 30).Value = string.Empty;
                ocm.Parameters.Add("pOutTransId", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
                ocm.Parameters.Add("pOutMsg", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.ExecuteNonQuery();
                iClose();
                string res = string.Empty;
                res = ocm.Parameters["pOutMsg"].Value.ToString();
                if (res == "000")
                {
                    retVal = true;
                }
                else
                {
                    LogWriter._error(TAG, string.Format(@"ProResult:[{0}], CardNo:[{1}], Amount:[{2}]", res, _cardNo, _amount));
                    retVal = false;
                }
            }
            catch (Exception ex)
            {
                LogWriter._error(TAG, ex.ToString());
                retVal = false;
            }
            return retVal;
        }
        public bool chargeProductInTrans(string _prodId, string _month, string _userName, string _amount, string _desc, string _cardNo, string channel, string transId)
        {
            bool retVal = false;
            try
            {
                iOpen();
                OracleCommand ocm = new OracleCommand("PKD_CHARGE_PRODUCT.chargeProduct_month_inTrans", conn);
                ocm.CommandType = CommandType.StoredProcedure;
                ocm.Parameters.Add("pproductId", OracleDbType.Int32, 30).Value = Convert.ToInt32(_prodId);
                ocm.Parameters.Add("pmonth", OracleDbType.Int32, 30).Value = Convert.ToInt32(_month);
                ocm.Parameters.Add("puser", OracleDbType.Varchar2, 100).Value = _userName;
                ocm.Parameters.Add("pbranch", OracleDbType.Varchar2, 30).Value = "286";
                ocm.Parameters.Add("pamount", OracleDbType.Decimal, 30).Value = Convert.ToDecimal(_amount);
                ocm.Parameters.Add("pchannelType", OracleDbType.Varchar2, 30).Value = channel;
                ocm.Parameters.Add("pcallType", OracleDbType.Int32, 30).Value = 9;
                ocm.Parameters.Add("pdescription", OracleDbType.Varchar2, 500).Value = _desc;
                ocm.Parameters.Add("pcardNumber", OracleDbType.Varchar2, 30).Value = _cardNo;
                ocm.Parameters.Add("pproTypeId", OracleDbType.Int32, 30).Value = 3001;
                ocm.Parameters.Add("ptransgroupid", OracleDbType.Varchar2, 30).Value = string.Empty;
                ocm.Parameters.Add("ptransId", OracleDbType.Int32, 30).Value = int.Parse(transId);
                ocm.Parameters.Add("pOutMsg", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                ocm.ExecuteNonQuery();
                iClose();
                string res = string.Empty;
                res = ocm.Parameters["pOutMsg"].Value.ToString();
                if (res == "000")
                {
                    retVal = true;
                }
                else
                {
                    LogWriter._error(TAG, string.Format(@"ProResult:[{0}], CardNo:[{1}], Amount:[{2}]", res, _cardNo, _amount));
                    retVal = false;
                }
            }
            catch (Exception ex)
            {
                LogWriter._error(TAG, ex.ToString());
                retVal = false;
            }
            return retVal;
        }
        public bool chargeAccount(string _cardNo, string _amount, string _userName, string _desc)
        {
            bool retVal = false;
            try
            {
                iOpen();
                OracleCommand ocm = new OracleCommand("PKD_COUNTER.add_counter_fromNumberUni1", conn);
                ocm.CommandType = CommandType.StoredProcedure;
                ocm.Parameters.Add("pcard_no", OracleDbType.Varchar2, 30).Value = _cardNo;
                ocm.Parameters.Add("pcounter_id", OracleDbType.Varchar2, 30).Value = "1001";
                ocm.Parameters.Add("pcounter_value", OracleDbType.Varchar2, 30).Value = _amount;
                ocm.Parameters.Add("pexpire_date", OracleDbType.Date, 30).Value = null;
                ocm.Parameters.Add("pupdate_user", OracleDbType.Varchar2, 100).Value = _userName;
                ocm.Parameters.Add("pchannel", OracleDbType.Varchar2, 30).Value = "6";
                ocm.Parameters.Add("pcalltype", OracleDbType.Varchar2, 30).Value = "1";
                ocm.Parameters.Add("pdescription", OracleDbType.Varchar2, 500).Value = _desc;
                ocm.Parameters.Add("pwithTrans", OracleDbType.Varchar2, 10).Value = "1";
                ocm.Parameters.Add("pbranch", OracleDbType.Varchar2, 10).Value = "286";
                ocm.Parameters.Add("pBankId", OracleDbType.Int32, 30).Value = null;
                ocm.Parameters.Add("pTRANSGROUPID", OracleDbType.Varchar2, 30).Value = string.Empty;
                ocm.Parameters.Add("pOutMsg", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
                ocm.ExecuteNonQuery();
                iClose();
                string res = string.Empty;
                res = ocm.Parameters["pOutMsg"].Value.ToString();
                if (res == "000")
                {
                    retVal = true;
                }
                else
                {
                    retVal = false;
                }
            }
            catch (Exception ex)
            {
                LogWriter._error(TAG, ex.ToString());
                retVal = false;
            }
            return retVal;
        }

        public bool chargeAccountInTrans(string _cardNo, string _amount, string _userName, string _desc, string _transId)
        {
            bool retVal = false;
            try
            {
                iOpen();
                OracleCommand ocm = new OracleCommand("PKD_COUNTER.add_counter_inTrans", conn);
                ocm.CommandType = CommandType.StoredProcedure;
                ocm.Parameters.Add("pcard_no", OracleDbType.Varchar2, 30).Value = _cardNo;
                ocm.Parameters.Add("pcounter_id", OracleDbType.Varchar2, 30).Value = "1001";
                ocm.Parameters.Add("pcounter_value", OracleDbType.Varchar2, 30).Value = _amount;
                ocm.Parameters.Add("pexpire_date", OracleDbType.Date, 30).Value = null;
                ocm.Parameters.Add("pupdate_user", OracleDbType.Varchar2, 100).Value = _userName;
                ocm.Parameters.Add("pchannel", OracleDbType.Varchar2, 30).Value = "6";
                ocm.Parameters.Add("pcalltype", OracleDbType.Varchar2, 30).Value = "1";
                ocm.Parameters.Add("pdescription", OracleDbType.Varchar2, 500).Value = _desc;
                ocm.Parameters.Add("pwithTrans", OracleDbType.Varchar2, 10).Value = _transId;
                ocm.Parameters.Add("pbranch", OracleDbType.Varchar2, 10).Value = "286";
                ocm.Parameters.Add("pBankId", OracleDbType.Int32, 30).Value = null;
                ocm.Parameters.Add("pTRANSGROUPID", OracleDbType.Varchar2, 30).Value = string.Empty;
                ocm.Parameters.Add("pOutMsg", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
                ocm.ExecuteNonQuery();
                iClose();
                string res = string.Empty;
                res = ocm.Parameters["pOutMsg"].Value.ToString();
                if (res == "000")
                {
                    retVal = true;
                }
                else
                {
                    retVal = false;
                }
            }
            catch (Exception ex)
            {
                LogWriter._error(TAG, ex.ToString());
                retVal = false;
            }
            return retVal;
        }

    }
}

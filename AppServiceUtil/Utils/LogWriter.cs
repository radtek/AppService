using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;

namespace AppServiceUtil.Utils
{
    public class LogWriter
    {
        public static void _error(string TAG, string logtxt)
        {
            try
            {
                var logfolder = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\Error";
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(logfolder);
                FileSystemAccessRule fsar = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity ds = null;
                if (!di.Exists)
                {
                    Directory.CreateDirectory(logfolder);
                }
                ds = di.GetAccessControl();
                ds.AddAccessRule(fsar);
                StreamWriter sw = new StreamWriter(logfolder + "\\error" + DateTime.Today.ToString("yyyyMMdd") + ".txt", true);
                sw.WriteLine(string.Format(@"{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), TAG, logtxt));
                sw.Close();
                logtxt = string.Empty;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public static void _other(string TAG, string logtxt)
        {
            try
            {
                var logfolder = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\Other";
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(logfolder);
                FileSystemAccessRule fsar = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity ds = null;
                if (!di.Exists)
                {
                    Directory.CreateDirectory(logfolder);
                }
                ds = di.GetAccessControl();
                ds.AddAccessRule(fsar);
                StreamWriter sw = new StreamWriter(logfolder + "\\other" + DateTime.Today.ToString("yyyyMMdd") + ".txt", true);
                sw.WriteLine(string.Format(@"{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), TAG, logtxt));
                sw.Close();
                logtxt = string.Empty;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public static void _userInfo(string TAG, string logtxt)
        {
            try
            {
                var logfolder = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\UserInfo";
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(logfolder);
                FileSystemAccessRule fsar = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity ds = null;
                if (!di.Exists)
                {
                    Directory.CreateDirectory(logfolder);
                }
                ds = di.GetAccessControl();
                ds.AddAccessRule(fsar);
                StreamWriter sw = new StreamWriter(logfolder + "\\info" + DateTime.Today.ToString("yyyyMMdd") + ".txt", true);
                sw.WriteLine(string.Format(@"{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), TAG, logtxt));
                sw.Close();
                logtxt = string.Empty;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public static void _login(string TAG, string logtxt)
        {
            try
            {
                var logfolder = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\Login";
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(logfolder);
                FileSystemAccessRule fsar = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity ds = null;
                if (!di.Exists)
                {
                    Directory.CreateDirectory(logfolder);
                }
                ds = di.GetAccessControl();
                ds.AddAccessRule(fsar);
                StreamWriter sw = new StreamWriter(logfolder + "\\login" + DateTime.Today.ToString("yyyyMMdd") + ".txt", true);
                sw.WriteLine(string.Format(@"{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), TAG, logtxt));
                sw.Close();
                logtxt = string.Empty;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public static void _vodList(string TAG, string logtxt)
        {
            try
            {
                var logfolder = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\VodList";
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(logfolder);
                FileSystemAccessRule fsar = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity ds = null;
                if (!di.Exists)
                {
                    Directory.CreateDirectory(logfolder);
                }
                ds = di.GetAccessControl();
                ds.AddAccessRule(fsar);
                StreamWriter sw = new StreamWriter(logfolder + "\\list" + DateTime.Today.ToString("yyyyMMdd") + ".txt", true);
                sw.WriteLine(string.Format(@"{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), TAG, logtxt));
                sw.Close();
                logtxt = string.Empty;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public static void _channelList(string TAG, string logtxt)
        {
            try
            {
                var logfolder = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\ChannelList";
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(logfolder);
                FileSystemAccessRule fsar = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity ds = null;
                if (!di.Exists)
                {
                    Directory.CreateDirectory(logfolder);
                }
                ds = di.GetAccessControl();
                ds.AddAccessRule(fsar);
                StreamWriter sw = new StreamWriter(logfolder + "\\list" + DateTime.Today.ToString("yyyyMMdd") + ".txt", true);
                sw.WriteLine(string.Format(@"{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), TAG, logtxt));
                sw.Close();
                logtxt = string.Empty;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public static void _newOrder(string TAG, string logtxt)
        {
            try
            {
                var logfolder = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\Order";
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(logfolder);
                FileSystemAccessRule fsar = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity ds = null;
                if (!di.Exists)
                {
                    Directory.CreateDirectory(logfolder);
                }
                ds = di.GetAccessControl();
                ds.AddAccessRule(fsar);
                StreamWriter sw = new StreamWriter(logfolder + "\\order" + DateTime.Today.ToString("yyyyMMdd") + ".txt", true);
                sw.WriteLine(string.Format(@"{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), TAG, logtxt));
                sw.Close();
                logtxt = string.Empty;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public static void _chargeProd(string TAG, string logtxt)
        {
            try
            {
                var logfolder = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\Charge";
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(logfolder);
                FileSystemAccessRule fsar = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity ds = null;
                if (!di.Exists)
                {
                    Directory.CreateDirectory(logfolder);
                }
                ds = di.GetAccessControl();
                ds.AddAccessRule(fsar);
                StreamWriter sw = new StreamWriter(logfolder + "\\charge" + DateTime.Today.ToString("yyyyMMdd") + ".txt", true);
                sw.WriteLine(string.Format(@"{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), TAG, logtxt));
                sw.Close();
                logtxt = string.Empty;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public static void _getBranches(string TAG, string logtxt)
        {
            try
            {
                var logfolder = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\Branch";
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(logfolder);
                FileSystemAccessRule fsar = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity ds = null;
                if (!di.Exists)
                {
                    Directory.CreateDirectory(logfolder);
                }
                ds = di.GetAccessControl();
                ds.AddAccessRule(fsar);
                StreamWriter sw = new StreamWriter(logfolder + "\\branch" + DateTime.Today.ToString("yyyyMMdd") + ".txt", true);
                sw.WriteLine(string.Format(@"{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), TAG, logtxt));
                sw.Close();
                logtxt = string.Empty;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public static void _addNvod(string TAG, string logtxt)
        {
            try
            {
                var logfolder = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\NVOD";
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(logfolder);
                FileSystemAccessRule fsar = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity ds = null;
                if (!di.Exists)
                {
                    Directory.CreateDirectory(logfolder);
                }
                ds = di.GetAccessControl();
                ds.AddAccessRule(fsar);
                StreamWriter sw = new StreamWriter(logfolder + "\\nvod" + DateTime.Today.ToString("yyyyMMdd") + ".txt", true);
                sw.WriteLine(string.Format(@"{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), TAG, logtxt));
                sw.Close();
                logtxt = string.Empty;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public static void _noti(string TAG, string logtxt)
        {
            try
            {
                var logfolder = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\Notification";
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(logfolder);
                FileSystemAccessRule fsar = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity ds = null;
                if (!di.Exists)
                {
                    Directory.CreateDirectory(logfolder);
                }
                ds = di.GetAccessControl();
                ds.AddAccessRule(fsar);
                StreamWriter sw = new StreamWriter(logfolder + "\\noti" + DateTime.Today.ToString("yyyyMMdd") + ".txt", true);
                sw.WriteLine(string.Format(@"{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), TAG, logtxt));
                sw.Close();
                logtxt = string.Empty;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public static void _pushVod(string TAG, string logtxt)
        {
            try
            {
                var logfolder = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\PushVod";
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(logfolder);
                FileSystemAccessRule fsar = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity ds = null;
                if (!di.Exists)
                {
                    Directory.CreateDirectory(logfolder);
                }
                ds = di.GetAccessControl();
                ds.AddAccessRule(fsar);
                StreamWriter sw = new StreamWriter(logfolder + "\\pushvod" + DateTime.Today.ToString("yyyyMMdd") + ".txt", true);
                sw.WriteLine(string.Format(@"{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), TAG, logtxt));
                sw.Close();
                logtxt = string.Empty;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public static void _upgradeProduct(string TAG, string logtxt)
        {
            try
            {
                var logfolder = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\UpgradeProduct";
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(logfolder);
                FileSystemAccessRule fsar = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity ds = null;
                if (!di.Exists)
                {
                    Directory.CreateDirectory(logfolder);
                }
                ds = di.GetAccessControl();
                ds.AddAccessRule(fsar);
                StreamWriter sw = new StreamWriter(logfolder + "\\upgrade" + DateTime.Today.ToString("yyyyMMdd") + ".txt", true);
                sw.WriteLine(string.Format(@"{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), TAG, logtxt));
                sw.Close();
                logtxt = string.Empty;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public static void _promo(string TAG, string logtxt)
        {
            try
            {
                var logfolder = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\Promo";
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(logfolder);
                FileSystemAccessRule fsar = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity ds = null;
                if (!di.Exists)
                {
                    Directory.CreateDirectory(logfolder);
                }
                ds = di.GetAccessControl();
                ds.AddAccessRule(fsar);
                StreamWriter sw = new StreamWriter(logfolder + "\\promo" + DateTime.Today.ToString("yyyyMMdd") + ".txt", true);
                sw.WriteLine(string.Format(@"{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), TAG, logtxt));
                sw.Close();
                logtxt = string.Empty;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public static void _chatBot(string TAG, string logtxt)
        {
            try
            {
                var logfolder = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\chatBot";
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(logfolder);
                FileSystemAccessRule fsar = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity ds = null;
                if (!di.Exists)
                {
                    Directory.CreateDirectory(logfolder);
                }
                ds = di.GetAccessControl();
                ds.AddAccessRule(fsar);
                StreamWriter sw = new StreamWriter(logfolder + "\\chatbot" + DateTime.Today.ToString("yyyyMMdd") + ".txt", true);
                sw.WriteLine(string.Format(@"{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), TAG, logtxt));
                sw.Close();
                logtxt = string.Empty;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public static void _qPay(string TAG, string logtxt)
        {
            try
            {
                var logfolder = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\qpay";
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(logfolder);
                FileSystemAccessRule fsar = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity ds = null;
                if (!di.Exists)
                {
                    Directory.CreateDirectory(logfolder);
                }
                ds = di.GetAccessControl();
                ds.AddAccessRule(fsar);
                StreamWriter sw = new StreamWriter(logfolder + "\\qpay" + DateTime.Today.ToString("yyyyMMdd") + ".txt", true);
                sw.WriteLine(string.Format(@"{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), TAG, logtxt));
                sw.Close();
                logtxt = string.Empty;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public static void _merchant(string TAG, string logtxt)
        {
            try
            {
                var logfolder = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\Merchant";
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(logfolder);
                FileSystemAccessRule fsar = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity ds = null;
                if (!di.Exists)
                {
                    Directory.CreateDirectory(logfolder);
                }
                ds = di.GetAccessControl();
                ds.AddAccessRule(fsar);
                StreamWriter sw = new StreamWriter(logfolder + "\\merchant" + DateTime.Today.ToString("yyyyMMdd") + ".txt", true);
                sw.WriteLine(string.Format(@"{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), TAG, logtxt));
                sw.Close();
                logtxt = string.Empty;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

    }
    public class exceptionManager
    {
        public static void ManageException(Exception _ex_, string _tag_)
        {
            StackTrace st = new StackTrace(_ex_, true);
            StackFrame[] frames = st.GetFrames();
            foreach (StackFrame frame in frames)
            {
                string fileName = frame.GetFileName();
                string methodName = frame.GetMethod().Name;
                int line = frame.GetFileLineNumber();
                int col = frame.GetFileColumnNumber();
                string log = string.Format("FILENAME: [{0}], METHOD: [{1}], LINE: [{2}], COLUMN: [{3}], MESSAGE: [{4}]", fileName, methodName, line, col, _ex_.Message);
                string __tag = string.Format("EXCEPTION ON {0}", _tag_);
                LogWriter._error(__tag, log);
            }
        }
    }
}

using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sshDBWork.Utils;

namespace sshDBWork
{
    class Program
    {
        static void Main(string[] args)
        {
            sshWork ssh = new sshWork();
            //Console.ReadKey();
            //sshTestResult ssh = new sshTestResult();
            //Console.ReadKey();
        }
    }
    public class sshWork
    {
        private string  TAG = "SSH";
        public sshWork()
        {
            if (runshLive())
            {
                if(sshCommand2())
                {
                    Console.WriteLine("DWH is OK");
                }
                else
                {
                    Console.WriteLine("Can not import database.");
                }
            }
            else
            {
                Console.WriteLine("Can not export database.");
            }
        }
        private bool runshLive()
        {
            bool retVal = false;
            try
            {
                SshClient cSSH = new SshClient("192.168.10.40", 22, "oracle", "oracle");
                cSSH.Connect();
                LogWriter._ssh(TAG, string.Format("Connected to LIVE DB from SSH"));
                string shCommand = string.Format("sh /installs/runexpdp.sh");
                LogWriter._ssh(TAG, string.Format("Started command: {0}", shCommand));
                SshCommand x = cSSH.RunCommand(shCommand);
                cSSH.Disconnect();
                cSSH.Dispose();
                LogWriter._ssh(TAG, "Command executed.");
                retVal = true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogWriter._error(TAG, ex.ToString());
                retVal = false;
            }
            return retVal;
        }
        private bool sshCommand2()
        {
            bool retVal = false;
            try
            {
                SshClient cSSH = new SshClient("192.168.10.211", 22, "oracle", "admin@123");
                cSSH.Connect();
                LogWriter._ssh(TAG, string.Format("Connected to DWH DB from SSH"));
                string shCommand = string.Format("sh /dmps/transferDMP.sh");
                LogWriter._ssh(TAG, string.Format("Started command: {0}", shCommand));
                SshCommand x = cSSH.RunCommand(shCommand);
                string rmDMP_DWH = string.Format("rm - f /dmps/dmp{0}.dmp", DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"));
                LogWriter._ssh(TAG, string.Format("Started command: {0}", rmDMP_DWH));
                SshCommand rmDmp = cSSH.RunCommand(rmDMP_DWH);
                cSSH.Disconnect();
                cSSH.Dispose();
                LogWriter._ssh(TAG, "DWH Command executed.");
                // -- delete Command LIVE DB
                SshClient lSSH = new SshClient("192.168.10.40", 22, "oracle", "oracle");
                string rmDMP_LIVE = string.Format("rm - f /installs/dmp{0}.dmp", DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"));
                LogWriter._ssh(TAG, string.Format("Started command: {0}", rmDMP_LIVE));
                SshCommand rmDmpLive = lSSH.RunCommand(rmDMP_LIVE);
                lSSH.Disconnect();
                lSSH.Dispose();
                LogWriter._ssh(TAG, "LIVEDB Command executed.");
                retVal = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogWriter._error(TAG, ex.ToString());
                retVal = false;
            }
            return retVal;
        }
    }
    public class sshTestResult
    {
        public sshTestResult()
        {
            CommandResult();
        }
        private void CommandResult()
        {
            try
            {
                SshClient cSSH = new SshClient("192.168.10.211", 22, "oracle", "admin@123");
                cSSH.Connect();
                string shCommand = string.Format("cat /dmps/transferDMP.sh");
                SshCommand x = cSSH.RunCommand(shCommand);
                Console.WriteLine(x.CommandText);
                if (!string.IsNullOrWhiteSpace(x.Result))
                    Console.WriteLine(x.Result);
                if (!string.IsNullOrWhiteSpace(x.Error))
                    Console.WriteLine(x.Error);
                cSSH.Disconnect();
                cSSH.Dispose();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

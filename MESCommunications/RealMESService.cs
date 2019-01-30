using Common;
using System;
using System.Data;

namespace MESCommunications
{
    public class RealMESService : IMESService
    {
        private string DBUserName;
        private string DBPasswordEncrypt;
        private string InsiteHost;
        private string InsitePort;
        private string InsiteUser;
        private string InsitePwd;

        private string DBServerName;
        private string DatabaseName;
        private string strLogPath;
        private string strLogFilePrefix;

        #region PUBLIC METHODS

        public bool Initialize(string resourceName)
        {
            // I dont see a use for these?
            DBUserName = Properties.Settings.Default.DBUserName;
            DBPasswordEncrypt = Properties.Settings.Default.DBPasswordEncrypt;
            InsiteHost = Properties.Settings.Default.InsiteHost;
            InsitePort = Properties.Settings.Default.InsitePort;
            InsiteUser = Properties.Settings.Default.InsiteUser;
            InsitePwd = Properties.Settings.Default.InsitePwd;

            // I do see a use for these...
            DBServerName = Properties.Settings.Default.DBServerName;
            DatabaseName = Properties.Settings.Default.DatabaseName;
            strLogPath = Properties.Settings.Default.strLogPath;
            strLogFilePrefix = Properties.Settings.Default.strLogFilePrefix;

            // Trying to connect to host??? 
            var cuServer = new Camstar.Utility.ServerConnection().Connect(InsiteHost, Int32.Parse(InsitePort));
            //cuServer.Host = InsiteHost;
            //cuServer.Port = Int32.Parse(InsitePort);
            // cuServer.Submit(); 
            try
            {
                var cmActions = new ShermanMes.IMESActions().GetResourceStatus(resourceName, DBServerName);
            }
            catch (Exception ex)
            {

            }
            return true;
        }

        // SHMValidateEmployee
        public string ValidateEmployee(string strEmployeeName)
        {
            string result = "Invalid User";
            try
            {
                result = new ShermanMes.IMESActions().ValidateEmployee(strEmployeeName, DBServerName);
            }
            catch (Exception ex)
            {
                result = $"Exception:" + ex.Message;
            }
            return result;
        }

        // SHMMoveIn(
        // string StrContainerName
        // string StrErrorMsg
        // bool RequiredCertification
        // Optional string Strcomments
        // string ResourceName
        // string Factory
        public bool LotMoveInCamstar(string lot, string employee, string comments, string errorMsgBack)
        {
            bool bRet = false; 
            string result = "Invalid User";            
            string resourceName = ""; 
            try
            {
                result = new ShermanMes.IMESActions().MoveIn(employee, lot, resourceName, comments, DBServerName, InsiteHost);
                // TODO: Need to parse this
                errorMsgBack = result; 
                bRet = true; 
            }
            catch (Exception ex)
            {
                errorMsgBack = $"Exception:" + ex.Message;
            }
            return bRet;
        }

        // StrContainerName 
        public DataTable GetContainerStatus(string resourceName)
        {
            DataSet dts;
            DataTable dt = null; 
            string result; 
            try
            {
                dts = new ShermanMes.IMESActions().GetContainerStatus(resourceName, DBServerName);
                dt = dts.Tables[0];
            }
            catch (Exception ex)
            {
                result = $"GetContainerStatus():Exception-" + ex.Message;
            }
            return dt; 
        }

        public Tool GetTool(string toolName)
        {
            var tool = new Tool();
            tool.Id = GetNextRandom(toolName);
            tool.ToolId = toolName;
            return tool;
        }

        public Lot GetLot(string lotName)
        {
            var lot = new Lot();
            lot.Id = GetNextRandom(lotName);
            lot.Lot1Name = lotName;
            //if (lot.Id < 100)
            //    lot = null;
            return lot;
        }

        public string GetToolStatusFromCamstar(string toolName)
        {
            throw new NotImplementedException();
        }
       
        #endregion

        private int GetNextRandom(string s)
        {
            int seed = 0;
            foreach (char c in s)
            {
                seed += (int)c;
            }
            Random r = new Random(seed);
            return r.Next(999);
        }

        public Operator GetOperator(string operatorName)
        {

            return  new Operator() { OperatorName = operatorName };
        }

        public DataTable GetLotStatus(string lotId)
        {
            return null; // _mesService.GetLotStatus(lotId);
        }
      
    }

}

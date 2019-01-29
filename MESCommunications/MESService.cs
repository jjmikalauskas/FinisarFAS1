using Common;
using ShermanMes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESCommunications
{
    public class MESService : IMESService
    {
        private IMESService _mesService;

        public MESService(IMESService iService)
        {
            _mesService = iService;
        }

        public bool Initialize(string resourceName)
        {
            return _mesService.Initialize(resourceName);
        }        
      
        public string GetToolStatusFromCamstar(string toolName)
        {
            return _mesService.GetToolStatusFromCamstar(toolName);
        }       

        public string LotMoveInCamstar(string lot)
        {
            return _mesService.LotMoveInCamstar(lot);
        }      

        public Operator GetOperator(string operatorName)
        {
            return _mesService.GetOperator(operatorName);
        }

        public DataTable GetLotStatus(string lotId)
        {
            return _mesService.GetLotStatus(lotId);
        }

        public DataTable GetResourceStatus(string resourceName, string dbServerName)
        {
            return _mesService.GetResourceStatus(resourceName, dbServerName);
        }

        public Tool GetTool(string toolName)
        {
            throw new NotImplementedException();
        }

        public Lot GetLot(string lotName)
        {
            throw new NotImplementedException();
        }
    }


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
            var cuServer = new Camstar.Utility.ServerConnection();
            cuServer.Host = InsiteHost;
            cuServer.Port = Int32.Parse(InsitePort);
            // cuServer.Submit(); 
            //try
            //{
            //    var cmActions = new IMESActions().GetResourceStatus(resourceName, DBServerName);
            //}
            //catch (Exception ex)
            //{

            //}
            return true;
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

        public string LotMoveInCamstar(string lot)
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
            return new Operator() { OperatorName = operatorName };
        }

        public DataTable GetLotStatus(string lotId)
        {
            return null; // _mesService.GetLotStatus(lotId);
        }

        public DataTable GetResourceStatus(string resourceName, string dbServerName)
        {
            return null; // _mesService.GetResourceStatus(resourceName, dbServerName);
        }
    }

}

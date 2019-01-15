using Common;
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

        #region PUBLIC METHODS             
       
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
    }
 
}

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
    public class MESService
    { 
        private IMESService _mesService;

        public MESService(IMESService mesService)
        {
            _mesService = mesService; 
        }

        public bool Initialize(string resourceName)
        {
            return _mesService.Initialize(resourceName);
        }        
      
        public string GetToolStatusFromCamstar(string toolName)
        {
            return _mesService.GetToolStatusFromCamstar(toolName);
        }

        public bool LotMoveInCamstar(string lot, string employee, string comments, string errorMsgBack)
        {
            return _mesService.LotMoveInCamstar(lot, employee, comments, errorMsgBack);
        }      

        public Operator GetOperator(string operatorName)
        {
            return _mesService.GetOperator(operatorName);
        }

        public DataTable GetLotStatus(string lotId)
        {
            return _mesService.GetLotStatus(lotId);
        }

        public DataTable GetContainerStatus(string resourceName)
        {
            return _mesService.GetContainerStatus(resourceName);
        }

        public Tool GetTool(string toolName)
        {
            throw new NotImplementedException();
        }

        public Lot GetLot(string lotName)
        {
            throw new NotImplementedException();
        }

        public string ValidateEmployee(string empName)
        {
            return _mesService.ValidateEmployee(empName); 
        }

    }


  
}

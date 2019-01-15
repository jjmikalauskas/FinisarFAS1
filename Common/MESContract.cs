using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IMESService
    {
        Operator GetOperator(string operatorName);
        Tool GetTool(string toolName);
        Lot GetLot(string lotName);

        // Operator ValidateUserFromCamstar(string userName);

        string GetToolStatusFromCamstar(string toolName); 
        DataTable GetLotStatus(string lotId);
        string LotMoveInCamstar(string lot);

        DataTable GetResourceStatus(string resourceName, string dbServerName);


    }

    // Seem to need to do this for Moq'ing

    public interface IOperatorRepository
    {
        // bool ValidateUserFromCamstar(string opName); 
        Operator GetOperator(string operatorName);
        DataTable GetLotStatus(string lotId); 
    }



}

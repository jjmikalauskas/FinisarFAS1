using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IMESService
    {
        Operator GetOperator(string operatorName, int id);
        Tool GetTool(string toolName, int id);
        Lot GetLot(string lotName, int id);
        bool ValidateUserFromCamstar(string userName);
        string GetToolStatusFromCamstar(string toolName); 
        bool GetLotOrWaferInfoFromCamstar(string lotId, int currentCassette);
        string LotMoveInCamstar(string lot);

    }

    // Seem to need to do this for Moq'ing

    public interface IOperatorRepository
    {
        Operator GetOperator(string opName); 
    }


   
}

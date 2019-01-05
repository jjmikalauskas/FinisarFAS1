using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IMESService
    {
        // Operator GetOperator(string operatorName);
        Tool GetTool(string toolName);
        Lot GetLot(string lotName);

        bool ValidateUserFromCamstar(string userName);
        string GetToolStatusFromCamstar(string toolName); 
        bool GetLotOrWaferInfoFromCamstar(string lotId, int currentCassette);
        string LotMoveInCamstar(string lot);

    }

    // Seem to need to do this for Moq'ing

    public interface IOperatorRepository
    {
        bool ValidateUserFromCamstar(string opName); 
    }


   
}

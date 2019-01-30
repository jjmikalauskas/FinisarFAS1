using System.Data;

namespace Common
{
    public interface IMESService
    {
        bool Initialize(string resourceName);

        // Per the MESDLL document from ZH
        string ValidateEmployee(string strEmployeeName);

        bool LotMoveInCamstar(string lot, string employee, string comments, string errorMsgBack );

        DataTable GetContainerStatus(string resourceName);

        Operator GetOperator(string operatorName);
        Tool GetTool(string toolName);
        Lot GetLot(string lotName);

        string GetToolStatusFromCamstar(string toolName); 
        DataTable GetLotStatus(string lotId);
    }

    // Seem to need to do this for Moq'ing

    public interface IOperatorRepository
    {
        Operator GetOperator(string operatorName);
        DataTable GetLotStatus(string lotId); 
    }

}

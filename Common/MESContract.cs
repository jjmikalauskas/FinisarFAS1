using System.Data;

namespace Common
{
    public interface IMESService
    {
        bool Initialize(string resourceName);

        Operator GetOperator(string operatorName);
        Tool GetTool(string toolName);
        Lot GetLot(string lotName);

        string GetToolStatusFromCamstar(string toolName); 
        DataTable GetLotStatus(string lotId);
        string LotMoveInCamstar(string lot);

        DataTable GetResourceStatus(string resourceName, string dbServerName);
    }

    // Seem to need to do this for Moq'ing

    public interface IOperatorRepository
    {
        Operator GetOperator(string operatorName);
        DataTable GetLotStatus(string lotId); 
    }

}

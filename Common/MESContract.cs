using System.Data;

namespace Common
{
    public interface IMESService
    {
        bool Initialize(string configFile, string hostName);

        // Per the MESDLL document from ZH
        AuthorizationLevel ValidateEmployee(string employee);

        DataTable GetContainerStatus(string container);

        DataTable GetResourceStatus(string resourceName);

        bool MoveIn(string container, string errorMsg, bool somebool,
                            string employee, string comment, string resourceName, string factoryName);

        bool MoveOut(string container, string errorMsg, bool somebool,
                            string employee, string comment);

        bool Hold(string container, string errorMsg,
             string employee, string comment, string resourceName,
             string factory, string holdReason);

        //bool LotMoveInCamstar(string lot, string employee, string comments, string errorMsgBack);
    }

    public interface IOperatorRepository
    {
        // Operator GetOperator(string operatorName);
        //DataTable GetLotStatus(string lotId); 
    }

}

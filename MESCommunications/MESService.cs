using Common;
using System.Data;

namespace MESCommunications
{
    public class MESService
    { 
        private IMESService _mesService;

        public MESService()
        {
            _mesService = new MoqMESService(); // MESDLL(); 
        }

        public MESService(IMESService service)
        {
            _mesService = service; 
        }

        public bool Initialize(string configFile, string hostName)
        {
            return _mesService.Initialize(configFile, hostName);
        }

        // MoveIn(string container, string errorMsg, bool requiredCertification,
        // string employee, string comment, string resourceName, string factoryName);
        public bool MoveIn( string container, ref string errorMsg, bool requiredCertification,
                            string employee, string comment, string resourceName, string factoryName)
        {
            return _mesService.MoveIn(container, ref errorMsg, requiredCertification, employee, comment, resourceName, factoryName);
        }

        public bool MoveOut(string container, ref string errorMsg, bool validateData,
                            string employee, string comment)
        {
            return _mesService.MoveOut(container, ref errorMsg, validateData, employee, comment);
        }

        public bool Hold(string container, string holdReason, ref string errorMsg,
                         string comment, string factory, string employee, string resourceName)
        {
            return _mesService.Hold(container,  holdReason, ref errorMsg, comment, factory, employee, resourceName);
        }

        public DataTable GetContainerStatus(string container)
        {
            return _mesService.GetContainerStatus(container);
        }

        public DataTable GetResourceStatus(string resourceName)
        {
            return _mesService.GetResourceStatus(resourceName);
        }

        public AuthorizationLevel ValidateEmployee(string empName)
        {
            return _mesService.ValidateEmployee(empName); 
        }

    }
  
}

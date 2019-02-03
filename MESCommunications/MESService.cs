using Common;
using System.Data;

namespace MESCommunications
{
    public class MESService
    { 
        private IMESService _mesService;

        public MESService()
        {
            _mesService = new MESDLL(); 
        }

        public MESService(IMESService service)
        {
            _mesService = service; 
        }

        public bool Initialize(string configFile, string hostName)
        {
            return _mesService.Initialize(configFile, hostName);
        }

        // MoveIn(string container, string errorMsg, bool somebool,
        // string employee, string comment, string resourceName, string factoryName);
        public bool MoveIn( string container, string errorMsg, bool somebool,
                            string employee, string comment, string resourceName, string factoryName)
        {
            return _mesService.MoveIn(container, errorMsg, somebool, employee, comment, resourceName, factoryName);
        }

        public bool MoveOut(string container, string errorMsg, bool somebool,
                            string employee, string comment)
        {
            return _mesService.MoveOut(container, errorMsg, somebool, employee, comment);
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

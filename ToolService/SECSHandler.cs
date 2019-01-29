using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SECSInterface;

namespace ToolService
{
    public class SECSHandler<T> : ISECSHandler<T> where T : Tool
    {
        private T _tool;
        public SECSHandler(T tool)
        {
            _tool = tool;
        }

        public void InitializeTool(string eqSvr, int timeout)
        {
            // call needed set of SECS messages to initialize this specific tool
            _tool.Initialize(eqSvr, timeout);
        }

        public void EstablishComm()
        {
            throw new NotImplementedException();
        }
    }
}

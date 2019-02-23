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
        
        public bool InitializeTool()
        {
            return _tool.Initialize();
        }
        public void StartProcessing(string Port, string[] LotIds, string Recipe)
        {
            _tool.StartProcessing(Port, LotIds, Recipe);
        }

    }
}

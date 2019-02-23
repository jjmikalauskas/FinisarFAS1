using SECSInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolService
{
    public interface ISECSHandler<T>
    {
        bool InitializeTool();
        void StartProcessing(string Port, string[] LotIds, string Recipe);


    }
}

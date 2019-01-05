using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Tool
    {
        public Tool()
        {
            Id = 0;
            ToolName = "Tool Number: " + Id.ToString();
            ToolInfo = "This is a cool tool to process wafers ";
        }

        public int Id { get; set; }
        public string ToolName { get; set; }
        public string ToolInfo { get; set; }
    }

    public class Ports
    {
        public Ports() {  }

        public Ports(int numberOfLoadPorts, bool loadLock, string loadPort1Name)
        {
            NumberOfLoadPorts = numberOfLoadPorts;
            LoadLock = loadLock;
            LoadPort1Name = loadPort1Name;
        }

        public int NumberOfLoadPorts { get; set; } = 0;
        public bool LoadLock { get; set; } = true;
        public string LoadPort1Name { get; set; } = "Def Port A";
        public string LoadPort2Name { get; set; } = "Def Port B";
        public string LoadPort3Name { get; set; }
        public string LoadPort4Name { get; set; }
    }
}

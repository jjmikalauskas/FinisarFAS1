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
            ToolId = "1-1-TOOL-001";
            ToolBrand = "COOL TOOL";
            Ports = new Ports(); 
        }

        public int Id { get; set; }
        public string ToolId { get; set; }
        public string ToolBrand { get; set; }
        //public string ToolInfo { get; set; }

        public int NumberOfLoadPorts { get; set; } = 0;
        public bool LoadLock { get; set; } = true;
        public Ports Ports { get; set; }
    }

    public class Ports
    {
        public Ports() {  }

        public Ports(string loadPort1Name)
        {
            LoadPort1Name = loadPort1Name;
        }
        
        public string LoadPort1Name { get; set; } = "Def Port A";
        public string LoadPort2Name { get; set; } = "Def Port B";
        public string LoadPort3Name { get; set; }
        public string LoadPort4Name { get; set; }
    }
}

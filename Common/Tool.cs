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
}

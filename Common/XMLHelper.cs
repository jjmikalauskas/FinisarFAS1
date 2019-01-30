
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Common;

namespace Common
{
    public class XMLHelper
    {
        public static ToolConfig ReadToolConfigXml(string filename) 
        {
            XmlSerializer ser = new XmlSerializer(typeof(ToolConfig));

            ToolConfig myTool = ser.Deserialize(new FileStream(filename, FileMode.Open)) as ToolConfig;

            if (myTool != null)
            {
                // do whatever you want with your "tool"
            }
            return myTool;
        }

        public static SystemConfig ReadSysConfigXml(string filename)
        {
            XmlSerializer ser = new XmlSerializer(typeof(SystemConfig));
            SystemConfig mysys2 = new SystemConfig();

            SystemConfig mysys = ser.Deserialize(new FileStream(filename, FileMode.Open)) as SystemConfig;

            if (mysys != null)
            {
                // do whatever you want with your "tool"
            }
            return mysys;
        }
    }
}

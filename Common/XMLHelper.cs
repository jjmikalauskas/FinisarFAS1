
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Common;

namespace Common
{
    public class XMLHelper
    {
        private const string pathToolConfigs = @"\Common\ToolConfigs\";
        private readonly static string currentDir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.Parent.Parent.Parent.FullName;
        private readonly static string xmlDirectory = currentDir + pathToolConfigs;

        public static T ReadXmlConfig<T>(string filename)
        {
            T config = default(T);
            XmlSerializer ser = new XmlSerializer(typeof(T));
            
            try
            {
                config = (T)ser.Deserialize(new FileStream(xmlDirectory + filename, FileMode.Open, FileAccess.Read));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return config;
        }

    }
}

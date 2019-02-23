﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoShellMessaging;
 

namespace Common
{
        public static class Globals 
        {
        //    public static bool IsLive;
        //    public static string FusionEnvironment;

        //    public static string ConnectionString;
        //    public static string Catalog;

        //    public static string UserName;

        //    public static void SetGlobals()
        //    {
        //        FusionEnvironment =  CheckGleanDS.Properties.Settings.Default.Environment;
        //        IsLive = FusionEnvironment == "PROD" ? true : false;
        //        ConnectionString = IsLive ? CheckGleanDS.Properties.Settings.Default.FUSIONPRODConnectionString : CheckGleanDS.Properties.Settings.Default.FUSIONPROTOConnectionString;

        //        try
        //        {
        //            string[] s1 = ConnectionString.Split(';');

        //            if (s1.Length > 0 && s1[1].IndexOf("Initial Catalog") == 0)
        //            {
        //                int i;
        //                string s2, sCat = s1[1];
        //                i = sCat.IndexOf('=');
        //                if (i > 1)
        //                {
        //                    s2 = sCat.Substring(i + 1);
        //                    Catalog = s2;
        //                }
        //            }
        //        }
        //        catch
        //        {
        //            Catalog = "Unknown";
        //        }

        //        UserName = Environment.UserName;

        //    }
               

        public const string VALIDUSER = "Valid User";

        public const string xmlDirectory = @"E:\Data\";
        public const string MESDefaultConfigDir = @"C:\FinTest\Config\";
        public const string MESDefaultConfigFile = @"MESConfig.ini";

        public const string ToolConfigFile = @"ToolConfig.xml";
        public const string SystemConfigFile = @"SystemConfig.xml";

        public static ToolConfig CurrentToolConfig = null;
        public static SystemConfig CurrentSystemConfig = null;

        public static AshlServerLite AshlServer = null;
        public static TraceDataCollector TheTraceDataCollector = null;

        public static void ReadXmlConfigs()
        {
            CurrentToolConfig = XMLHelper.ReadXmlConfig<ToolConfig>(@"Evatec\ToolConfig.xml");
            CurrentSystemConfig = XMLHelper.ReadXmlConfig<SystemConfig>("SystemConfig.xml");
        }
            

        public static FASLog MyLog; 

        static public string GetColor(string currentStatus)
        {
            string retColor = "Azure";
            if (currentStatus == null) return retColor;
            currentStatus = currentStatus.ToUpper();
            if (currentStatus.Contains("COMPLETE"))
                retColor = "Lime";
            else if (currentStatus.Contains("ERROR") || currentStatus.Contains("ABORT"))
                retColor = "Red";
            else if (currentStatus.Contains("MOVED IN"))
                retColor = "Yellow";
            else if (currentStatus.Contains("PROCESS"))
                retColor = "DodgerBlue";
            else if (currentStatus.Equals("READY"))
                retColor = "Azure"; 
            return retColor;
        }

        public enum ProcessStates { NOTREADY, READY, EXECUTING, UNDEFINED  };

        public class ProcessState
        {
            public ProcessStates State { get; private set; }
            public string Description { get; private set; }
            public ProcessState(ProcessStates State, string Description)
            {
                this.State = State;
                this.Description = Description;
            }
        }
        public enum ControlStates { OFFLINE, LOCAL, REMOTE, UNDEFINED };
        public class ControlState
        {
            public ControlStates State { get; private set; }
            public string Description { get; private set; }
            public ControlState(ControlStates State, string Description)
            {
                this.State = State;
                this.Description = Description;
            }
        }

    }

}

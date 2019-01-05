using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
        //public class Globals
        //{
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
        //}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Common.Globals;

namespace FinisarFAS1.Utility
{
    public class SecMessageHelpers
    {
        internal static bool SendSECSStart()
        {
            bool bRet = true;
            MyLog.Debug("SendSECSStart () called...");
            return bRet; 
        }

        internal static bool SendSECSStop()
        {
            bool bRet = true;
            MyLog.Debug("SendSECSStop() called...");
            return bRet;
        }

        internal static bool SendSECSPause()
        {
            bool bRet = true;
            MyLog.Debug("SendSECSPause() called...");
            return bRet;
        }

        internal static bool SendSECSAbort()
        {
            bool bRet = true;
            MyLog.Debug("SendSECSAbort() called...");
            return bRet;
        }

        internal static bool SendSECSGoLocal()
        {
            bool bRet = true;
            MyLog.Debug("SendSECGoLocal() called...");
            return bRet;
        }

        internal static bool SendSECSGoRemote()
        {
            bool bRet = true;
            MyLog.Debug("SendSECSGoRemote() called...");
            return bRet;
        }
    }
}

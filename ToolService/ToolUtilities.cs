using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Threading;
using AutoShellMessaging;
using Common;
using static Common.Globals;
using GalaSoft.MvvmLight.Messaging;
using SECSInterface;
using Serilog;
using System.Text;

namespace ToolService
{
    public class ToolUtilities
    {
        public static void logMessageSendExceptions(string description, Exception e)
        {
            if (e is BoundMessageTimeoutException)
            {
                string message = "Timeout receiving reply to " + description;
                Messenger.Default.Send(new EventMessage(DateTime.Now, "E", message));
                Messenger.Default.Send(new EventMessage(new LogEntry(message)));
            }
            else { 
                 if (e is BoundMessageSendException)
                {
                    string message = "Error sending " + description + " message; details:" + e.Message;
                    Messenger.Default.Send(new EventMessage(DateTime.Now, "E", message));
                    Messenger.Default.Send(new EventMessage(new LogEntry(message)));
                }
                else
                {
                    string message = "Unexpected exception sending " + description + " message; details:" + e.Message;
                    Messenger.Default.Send(new EventMessage(DateTime.Now, "E", message));
                    Messenger.Default.Send(new EventMessage(new LogEntry(message)));
                }
            }

        }

        public static bool checkAck(string ack)
        {
            if (ack != null && (ack.Equals("00") || ack.Equals("0")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}

using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Common.Globals;
using GalaSoft.MvvmLight.Messaging;


namespace Common
{
    public class LogEntry
    {
        public LogEntry() { }
        public LogEntry(string Message)
        {
            EventDateTime = DateTime.Now;
            EventType = "L";
            this.Message = Message;
        }
        public DateTime EventDateTime { get; set; }
        public string EventType { get; set; }
        public string Message { get; set; }
    }
 
    public class FASLog
    {
        // TODO: Log naming convention
        public FASLog(string logFile = @"C:\Logs\Log.Txt")
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(logFile,
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true)
                .CreateLogger();

            Messenger.Default.Register<EventMessage>(this, EventMessageHandler);
        }

        public void Debug(string debugText)
        {
            Log.Debug(debugText);
        }

        public void Information(string infoText)
        {
            Log.Information(infoText);
        }

        public void Warning(string errorText)
        {
            Log.Warning(errorText);
        }

        public void Error(string errorText)
        {
            Log.Error(errorText);
        }

        public void Error(Exception ex, string errorText)
        {
            Log.Error(ex, errorText);
        }

        public void Timing(string traceText)
        {
            DateTime dtNow = DateTime.Now;
            Log.Debug($"{dtNow}-{traceText}");
        }

        private void EventMessageHandler(EventMessage msg)
        {
            if (msg.MsgType == "L")
            {
                Log.Information(msg.Message);
            }
        }

        }
    }

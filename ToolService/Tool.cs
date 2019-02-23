using System;
using System.Collections.Generic;
using System.Text;
using Serilog;
using AutoShellMessaging;
using Common;
using GalaSoft.MvvmLight.Messaging;
using SECSInterface;
using static Common.Globals;


namespace ToolService
{
    public abstract class Tool : AshlMessageHandler
    {
        //public abstract string eqName { get; set; }
        public string EqSrv { get; set; }
         
        public abstract List<Port> Ports { get; set; }
        protected Dictionary<int, ControlState> ControlStateDict = new Dictionary<int, ControlState>();
        protected Dictionary<int, ProcessState> ProcessStateDict = new Dictionary<int, ProcessState>();
        protected Dictionary<ulong, List<ToolConfigReport2>> EventReportDict = new Dictionary<ulong, List<ToolConfigReport2>>();
        protected Dictionary<string, ToolConfigReportVid[]> TraceReportDict = new Dictionary<string, ToolConfigReportVid[]> ();
        protected abstract string ProcessStateChangeCEID { get; set; }
        protected string ToolConfigProcessStateReport { get; set; }
        protected int ToolConfigProcessStateVIDIndex { get; set; }
        protected static List<String> ProcessCompletedCEID { get; set; }
 
        protected Tool()
        {
            InitializeControlStates();
            InitializeProcessStates();
            InitializeToolEventsAndReports();
            InitializeTraceReportDict();
            EqSrv = CurrentToolConfig.Toolid + "srv";

            SetUpComms();
        }

        // Initialize AutoShell
        private void SetUpComms()
        {
            string MyName = System.Environment.GetEnvironmentVariable("COMPUTERNAME") + "FAS";
            string AciConf = System.Environment.GetEnvironmentVariable("COMPUTERNAME") + ":1500";
            MessagingSettings _messagingSettings = new MessagingSettings()
            {
                //TODO CHange to using SystemConfig
                AciConf = AciConf,
                CheckDuplicateRegistration = true,
                UseInterfaceSelectionMethod = InterfaceSelectionMethod.DISCOVER,                 
                Name = MyName
            };

            // Instantiate an AshlServerLite instance
            AshlServer = AshlServerLite.getInstanceUsingParameters(_messagingSettings);
            AshlServer.registerHandler(this);

            AshlMessage monitorMessage = AshlMessage.newInstanceCommand(EqSrv, "fr=" + MyName + " to=" + EqSrv + " eq=" + CurrentToolConfig.Toolid + " do=monitor all", 20);
            monitorMessage.send();

            if (!AshlServer.isActive())
            {
                Messenger.Default.Send(new EventMessage(DateTime.Now, "E", "Unable to start AutoShell communications with name '" + MyName + "' ACI_CONF '" + AciConf));

            }
            else
            {
                Messenger.Default.Send(new EventMessage(new LogEntry("AutoShell server started with name '" + MyName)));
            }

        }
        /// <summary>
        /// Set up the Control State dictionary
        /// </summary>
        protected abstract void InitializeControlStates();
        /// <summary>
        /// Set up the Process State dictionary
        /// </summary>
        protected abstract void InitializeProcessStates();
 
        /// <summary>
        /// Perform the tool initialization sequence (typically, S1F13, S1F1 and S1F3 status query for control state and process State
        /// </summary>
        /// <param name="eqSvr"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public abstract bool Initialize();
        public abstract bool QueryProcessAndControlState();
        public abstract bool PossiblyHandleControlStateChangeEvent(string CEID);

        /// <summary>
        /// See if this CEID is a process state change CEID; if so, see if it has a report that gives us the new
        /// process state; if yes, send notification to the UI.
        /// If it doesn't have a report, query the tool for process state so that the query can send notification to the UI.
        /// <br/><br/>
        /// This can be overridden for tools that have more than one process state change CEID.
        /// </summary>
        /// <param name="CEID"></param>
        /// <param name="eventMessage"></param>
        /// <returns></returns>
        protected bool PossiblyHandleProcessStateChangeEvent(string CEID, S6F11 eventMessage)
        {
             // If this is the CEID for ProcessStateChanged, go see if it has a report containing the processstate
            if (CEID.Equals(ProcessStateChangeCEID))
            {
                string newProcessState = null;

                if (eventMessage.Reports.Count > 0)     //Okay, we have some reports--are any of them the one we want
                {
                    if (ToolConfigProcessStateReport != null && eventMessage.Reports.ContainsKey(ToolConfigProcessStateReport))
                    {
                        if (eventMessage.Reports[ToolConfigProcessStateReport].Count > ToolConfigProcessStateVIDIndex)
                        {
                            SECSData vidValue = eventMessage.Reports[ToolConfigProcessStateReport][ToolConfigProcessStateVIDIndex];
                            newProcessState = vidValue.Value;
                        }
                    }
                }
                // If, after all that, you don't have a newProcessState, you'll have to query the tool.
                if (newProcessState == null)
                {
                    QueryProcessAndControlState();
                } else
                {
                    try {
                         int processState = Int32.Parse(newProcessState);
                        if (ProcessStateDict.ContainsKey(processState))
                        {
                            ProcessState newValue = ProcessStateDict[processState];
                            Messenger.Default.Send(new ProcessStateChangeMessage(newValue.State, newValue.Description));
                            Messenger.Default.Send(new EventMessage(new LogEntry("Process State changed to " + newValue.Description)));
                           }
                        else
                        {
                           string message = "Process State changed to undefined state " + newProcessState;
                           Messenger.Default.Send(new EventMessage(DateTime.Now, "E", message));
                           Messenger.Default.Send(new EventMessage(new LogEntry(message)));

                        }
                    } catch (Exception)
                    {
                        string message = "Process State changed to undefined state " + newProcessState;
                        Messenger.Default.Send(new EventMessage(DateTime.Now, "E", message));
                        Messenger.Default.Send(new EventMessage(new LogEntry(message)));
                    }
                }
                return true;
            } else {
                return false;
            }
        }
        public void StartProcessing(string Port, string[] LotIds, string Recipe)
        {
            if (DoEventReportSetup(true))
            {
                if (DoToolSpecificStartSequence(Port, LotIds, Recipe))
                {
                    StartOrStopToolTraces(Port, LotIds, Recipe, true);
                }
            }
        }

        protected bool SendAreYouThere()
        {
            S1F1 areYouThereMessage = new S1F1(CurrentToolConfig.Toolid);
             Messenger.Default.Send(new EventMessage(new LogEntry("Sending S1F1(areyouthere)")));

            try
            {
                areYouThereMessage.send(EqSrv, CurrentToolConfig.CommunicationTimeout);

                if (string.IsNullOrWhiteSpace(areYouThereMessage.MDLN) && !string.IsNullOrWhiteSpace(areYouThereMessage.SOFTREV))
                {
                     Messenger.Default.Send(new EventMessage(new LogEntry("S1F1 returned no values for MDLN or SOFTREV")));
                }
                else
                {
                     Messenger.Default.Send(new EventMessage(new LogEntry("S1F1 returned MDLN '" + areYouThereMessage.MDLN + "' SOFTREV '" + areYouThereMessage.SOFTREV + "'")));

                }
                return true;
            }
            catch (Exception e)
            {
                ToolUtilities.logMessageSendExceptions("S1F1 (areyouthere)", e);
                return false;
            }

        }


        protected bool SendCommunicate()
        {
            S1F13 communicateMessage = new S1F13(CurrentToolConfig.Toolid);
            Messenger.Default.Send(new EventMessage(new LogEntry("Sending S1F13 (communicate request)")));
            try
            {
                communicateMessage.send(EqSrv, CurrentToolConfig.CommunicationTimeout);
                if (ToolUtilities.checkAck(communicateMessage.COMMACK))
                {
                    if (string.IsNullOrWhiteSpace(communicateMessage.MDLN) && !string.IsNullOrWhiteSpace(communicateMessage.SOFTREV))
                    {
                         Messenger.Default.Send(new EventMessage(new LogEntry("S1F13 returned no values for MDLN or SOFTREV")));

                    }
                    else
                    {
                        Messenger.Default.Send(new EventMessage(new LogEntry("S1F13 returned MDLN '" + communicateMessage.MDLN + "' SOFTREV '" + communicateMessage.SOFTREV + "'")));
                    }
                    return true;
                }
                else
                {
                    Messenger.Default.Send(new EventMessage(new LogEntry("S1F13 returned COMMACK=" + communicateMessage.COMMACK)));

                    return false;
                }
            }
            catch (Exception e)
            {
                ToolUtilities.logMessageSendExceptions("S1F13 (communicate)", e);
                return false;
            }
        }
        protected bool SendEnableAlarms()
        {
            S5F3 s5f3 = new S5F3(CurrentToolConfig.Toolid);
            s5f3.setALED("01", DataType.BOOL);

            if (!CurrentToolConfig.EnableAllAlarms)
            {
                foreach (var alarm in CurrentToolConfig.AlarmList)
                {
                    s5f3.addALID(alarm.id.ToString(), DataType.UI4);
                }
            }
            try
            {
                Messenger.Default.Send(new EventMessage(new LogEntry("Sending S5F3 to enable " + (CurrentToolConfig.EnableAllAlarms ? "all" :
                    CurrentToolConfig.AlarmList.Length.ToString()) + " alarms")));
                s5f3.send(EqSrv, CurrentToolConfig.CommunicationTimeout);
                Messenger.Default.Send(new EventMessage(new LogEntry("S5F3 returned ACKC5=" + s5f3.ACKC5)));

                if (ToolUtilities.checkAck(s5f3.ACKC5))
                {
                    return true;
                }
                else
                {
                   return false;
                }
            }
             catch (Exception e)
            {
                ToolUtilities.logMessageSendExceptions("S5F3 enable alarms", e);
                return false;
            }

        }


        /// <summary>
        /// Defines, links and optionally enables events and reports
        /// </summary>
        /// <param name="EnableEvents">Set to false to define and link reports but not enable events</param>
        /// <returns></returns>
        protected bool DoEventReportSetup(bool EnableEvents)
        {
            char[] trimChars = new char[] { ',' };
            string whereAmI = "";
            try {
                StringBuilder logMessage = new StringBuilder();
                S2F37 enableDisableEvents = new S2F37(CurrentToolConfig.Toolid);
                whereAmI = "disable events";
                logMessage.Append("Sending " + whereAmI +"; CEID=");
                enableDisableEvents.setCEED("00", DataType.BOOL);
                foreach (int ceid in EventReportDict.Keys)
                {
                    enableDisableEvents.addCEID(ceid.ToString(), DataType.UI4);
                    logMessage.Append(ceid);
                    logMessage.Append(",");
                }
                enableDisableEvents.send(EqSrv, CurrentToolConfig.CommunicationTimeout);
                Messenger.Default.Send(new EventMessage(new LogEntry(logMessage.ToString().TrimEnd(trimChars))));


                //TODO some DRY is needed on this
                logMessage = new StringBuilder();
                S2F35 linkOrUnlinkReports = new S2F35(CurrentToolConfig.Toolid);
                linkOrUnlinkReports.setDATAID("1", DataType.UI4);
                whereAmI = "unlink event reports";
                logMessage.Append("Sending " + whereAmI + "; CEID=");
                foreach (int ceid in EventReportDict.Keys)
                {
                    linkOrUnlinkReports.addCEID(ceid.ToString(), DataType.UI4, new List<string>(), DataType.UI4);
                    logMessage.Append(ceid);
                    logMessage.Append(",");
                }
                linkOrUnlinkReports.send(EqSrv, CurrentToolConfig.CommunicationTimeout);
                Messenger.Default.Send(new EventMessage(new LogEntry(logMessage.ToString().TrimEnd(trimChars))));


                logMessage = new StringBuilder();
                S2F33 defineOrUndefineReports = new S2F33(CurrentToolConfig.Toolid);
                defineOrUndefineReports.setDATAID("1", DataType.UI4);
                whereAmI = "undefine event reports";
                logMessage.Append("Sending " + whereAmI + "; RPTID=");
                foreach (ToolConfigReport2 eventReport in CurrentToolConfig.eventReports)
                {
                    defineOrUndefineReports.addReport(eventReport.id.ToString(), DataType.UI4, new List<string>(), DataType.UI4);
                    logMessage.Append(eventReport.id);
                    logMessage.Append(",");
                }
                defineOrUndefineReports.send(EqSrv, CurrentToolConfig.CommunicationTimeout);
                Messenger.Default.Send(new EventMessage(new LogEntry(logMessage.ToString().TrimEnd(trimChars))));


                logMessage = new StringBuilder();
                defineOrUndefineReports = new S2F33(CurrentToolConfig.Toolid);
                defineOrUndefineReports.setDATAID("1", DataType.UI4);
                whereAmI = "define event reports";
                logMessage.Append("Sending " + whereAmI + "; RPTID=");
                foreach (ToolConfigReport2 eventReport in CurrentToolConfig.eventReports)
                {
                    List<string> vids = new List<string>();
                    foreach (ToolConfigReportVid1 vid in eventReport.vids)
                    {
                        vids.Add(vid.Value.ToString());
                    }
                    defineOrUndefineReports.addReport(eventReport.id.ToString(), DataType.UI4, vids, DataType.UI4);
                    logMessage.Append(eventReport.id);
                    logMessage.Append(",");
                }
                defineOrUndefineReports.send(EqSrv, CurrentToolConfig.CommunicationTimeout);
                Messenger.Default.Send(new EventMessage(new LogEntry(logMessage.ToString().TrimEnd(trimChars))));


                logMessage = new StringBuilder();
                linkOrUnlinkReports = new S2F35(CurrentToolConfig.Toolid);
                linkOrUnlinkReports.setDATAID("1", DataType.UI4);
                whereAmI = "link event reports";
                logMessage.Append("Sending " + whereAmI + "; CEID=");
                foreach (ulong ceid in EventReportDict.Keys)
                {
                    List<string> reportIds = new List<string>();
                    foreach (ToolConfigReport2 report in EventReportDict[ceid])
                    {
                        reportIds.Add(report.id.ToString());
                    }
                    linkOrUnlinkReports.addCEID(ceid.ToString(), DataType.UI4, reportIds, DataType.UI4);
                    logMessage.Append(ceid);
                    logMessage.Append(",");
                } 
                linkOrUnlinkReports.send(EqSrv, CurrentToolConfig.CommunicationTimeout);
                 Messenger.Default.Send(new EventMessage(new LogEntry(logMessage.ToString().TrimEnd(trimChars))));


                logMessage = new StringBuilder();
                enableDisableEvents = new S2F37(CurrentToolConfig.Toolid);
                whereAmI = "enable events";
                logMessage.Append("Sending " + whereAmI + "; CEID=");
                enableDisableEvents.setCEED("01", DataType.BOOL);
                foreach (int ceid in EventReportDict.Keys)
                {
                    enableDisableEvents.addCEID(ceid.ToString(), DataType.UI4);
                       logMessage.Append(ceid);
                    logMessage.Append(",");
                }
 
                enableDisableEvents.send(EqSrv, CurrentToolConfig.CommunicationTimeout); 
                 Messenger.Default.Send(new EventMessage(new LogEntry(logMessage.ToString().TrimEnd(trimChars))));


                return true;
            } catch (Exception e)
            {
                ToolUtilities.logMessageSendExceptions(whereAmI, e);
                return false;
            }
        }
        protected bool StartOrStopToolTraces(string Port, string[] LotIds, string Recipe, bool start)
        {
            foreach (ToolConfigReport trace in CurrentToolConfig.TraceReports)
            {
                S2F23 startTraceMessage = new S2F23(CurrentToolConfig.Toolid);
                startTraceMessage.SetTRID(trace.id.ToString(), DataType.UI4);
                startTraceMessage.SetDSPER(String.Format("{0,6:D6}", trace.dsper), DataType.ASC);  //TODO fix this when tool config corrected
                startTraceMessage.SetREPGSZ("1", DataType.UI4);
                if (start)
                {
                    startTraceMessage.SetTOTSMP(trace.totsmp.ToString(), DataType.UI4);
                    foreach (ToolConfigReportVid vid in trace.vids)
                    {
                        startTraceMessage.addSVID(vid.Value.ToString(), DataType.UI4);
                    }
                }
                else
                {
                    startTraceMessage.SetTOTSMP("0", DataType.UI4);
                }

                string logStartStop = start ? "Start" : "Stop";
                Messenger.Default.Send(new EventMessage(new LogEntry(logStartStop + " trace ID " + trace.id + " with " + trace.vids.Length + " SVIDs")));

                try
                {
                    startTraceMessage.send(EqSrv, CurrentToolConfig.CommunicationTimeout);
                    if (ToolUtilities.checkAck(startTraceMessage.TIAACK))
                    {
                        Messenger.Default.Send(new EventMessage(new LogEntry(logStartStop + " Trace ID " + trace.id + " returned good ACK")));

                    }
                    else
                    {
                        string message = logStartStop + " Trace ID " + trace.id + " failed with ACK=" + startTraceMessage.TIAACK;
                        Messenger.Default.Send(new EventMessage(DateTime.Now, "E", message));
                        Messenger.Default.Send(new EventMessage(new LogEntry(message)));

                        return false;
                    }
                } 
                catch (Exception e)
                {
                   ToolUtilities.logMessageSendExceptions(logStartStop, e);
                    return false;
                }
            }

            if (start)
                {
                if (TheTraceDataCollector == null)
                    TheTraceDataCollector = new TraceDataCollector(Port, LotIds, Recipe);
                }
            else
                 if (TheTraceDataCollector != null)
                    {
                        TheTraceDataCollector.CloseDataFile();
                        TheTraceDataCollector = null;
                    }

            return true;
        }

        protected abstract bool DoToolSpecificStartSequence(String Port, String[] LotIds, String recipe);

        protected bool PossiblyHandleProcessCompletedEvent(string CEID, S6F11 eventMessage)
        {
            // If this is the CEID for Completed
            if (ProcessCompletedCEID.Contains(CEID))
            {
                //TODO: unsure yet if we want some DVID
                Messenger.Default.Send(new ProcessCompletedMessage("Processing Completed"));
                  Messenger.Default.Send(new EventMessage(new LogEntry("Processing Completed event (" + CEID + ") received")));

                StartOrStopToolTraces(null, null, null, false);  // I can do this for Evatec because it only has a single port.
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary
        /// Reformat the reports in ToolConfig to make them indexed by event ID, for easier 
        /// use in the incoming event handler.
        /// It will also set up the ToolConfigProcessStateReport and ToolConfigProcessStateVIDIndex variables.
        /// </summary>
        private void InitializeToolEventsAndReports()
        {
           
            foreach (ToolConfigReport2 configReport in CurrentToolConfig.eventReports)
            {
                foreach (ToolConfigReportCeid ceid in configReport.events)
                {
                    if (EventReportDict.ContainsKey(ceid.Value))
                    {
                        EventReportDict[ceid.Value].Add(configReport);
                    }
                    else
                    {
                        List<ToolConfigReport2> reports = new List<ToolConfigReport2>();
                        reports.Add(configReport);
                        EventReportDict.Add(ceid.Value, reports);
                    }
                    try
                    {
                        // This stuff happens until we find a the ProcessStateChangeCEID. Then it is skipped.
                        if (ToolConfigProcessStateReport == null && ulong.Parse(ProcessStateChangeCEID) == ceid.Value) { }
                        {
                            foreach (ToolConfigReport2 report in EventReportDict[ceid.Value])
                            {
                                for (int index = 0; index < report.vids.Length && ToolConfigProcessStateReport == null; index++)
                                {
                                    if (report.vids[index].Value == CurrentToolConfig.ProcessStateSVID)
                                    {
                                        ToolConfigProcessStateReport = report.id.ToString();
                                        ToolConfigProcessStateVIDIndex = index;
                                    }
                                }
                                if (ToolConfigProcessStateReport != null)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Probably not interesting; we have a backup plan
                    }
                }
            }
        }
        private void InitializeTraceReportDict()
        {
            foreach (ToolConfigReport trace in CurrentToolConfig.TraceReports)
            {
                TraceReportDict.Add(trace.id.ToString(), trace.vids);
            }
        }

    /// <summary>
    /// This is the entry point for all unsolicited messages from the tool: events, reports, alarms.
    /// </summary>
    /// <param name="message"></param>
        void AshlMessageHandler.handleDataMessage(DecodedMsg message)
        {
            string streamFunction = message.GetValue("SF");
            if (streamFunction != null) {
                switch (streamFunction)
                {
                    case "S5F1":
                        handleAlarm(message);
                        break;
                    case "S6F11":
                        handleEvent(message);
                        break;
                    case "S6F1":
                        handleTrace(message);
                        break;
                    default:
                        //Do nothing
                        break;
                }
            }
        }
        public void handleAlarm(DecodedMsg message)
        {
            S5F1 alarm = new S5F1(message);
            Messenger.Default.Send(new EventMessage(DateTime.Now, "A", alarm.ALTX));
        }
        public void handleEvent(DecodedMsg message)
        {
            S6F11 eventReport = new S6F11(message);
            string CEID = eventReport.CEID;
            // If it's a control state change, handle it below and quit
            if (PossiblyHandleControlStateChangeEvent(CEID))
            {
                return;
            }
            if (PossiblyHandleProcessStateChangeEvent(CEID, eventReport))
            {
                return;
            }
            if (PossiblyHandleProcessCompletedEvent(CEID, eventReport))
            {
                return;
            }
            //TODO  handle all other events the same way?
            // Use the ToolEvent class
  
        }
        public void handleTrace(DecodedMsg message)
        {
            S6F1 traceReport = new S6F1(message);
            string traceId = traceReport.TRID;
            string sampleNumber = traceReport.SMPLN;
            string sampleTimeFromTool = traceReport.STIME;   // We typically ignore this becuase tool clocks drift

            if (TraceReportDict.ContainsKey(traceId))
            {
                ToolConfigReportVid[] vids = TraceReportDict[traceId];
                if (vids.Length != traceReport.SV.Count)
                {
                    Messenger.Default.Send(new EventMessage(new LogEntry("Trace report " + traceId + " contained invalid SV count; ignoring")));

                }
                else
                {
                    if (TheTraceDataCollector != null)
                    {
                        TheTraceDataCollector.AddData(traceReport, vids);
                    }
                }

            }
        }


        void AshlMessageHandler.handleUnsolicitedReplyMessage(DecodedMsg message)
        {
            //TODO: log and ignore
        }
        string AshlMessageHandler.handleCommandMessage(DecodedMsg message)
        {
            //TODO remove this when out of test cycle
            StartProcessing(null, new String[] { "Lot1" }, "DUMMYRECIPE");
            return "";
        }

     }
   
}


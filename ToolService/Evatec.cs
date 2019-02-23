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
    public class Evatec : Tool
    {
        public override List<Port> Ports { get; set; }
        //TODO  get all these magic numbers into one location
        private string ControlStateLocalCEID = "2";
        private string ControlStateRemoteCEID = "3";
        private string ControlStateOfflineCEID = "1";
        protected override string ProcessStateChangeCEID {
            get {
                return "10";
            }
            set
            {
                this.ProcessStateChangeCEID = value;
            }
        }
 
        static Evatec()
        {
            String[] all = { "101" };
            ProcessCompletedCEID = new List<string>();
            ProcessCompletedCEID.AddRange(all);
        }
                 

        protected override void InitializeControlStates()
        {
            ControlStateDict.Add(1, new ControlState(ControlStates.OFFLINE, 
                 "Off-Line/Equipment Off-Line"));
            ControlStateDict.Add(2, new ControlState(ControlStates.OFFLINE,
                 "Off-Line/Attempt On-Line"));
            ControlStateDict.Add(3, new ControlState(ControlStates.OFFLINE,
                  "Off-Line/Host Off-Line"));
            ControlStateDict.Add(4, new ControlState(ControlStates.LOCAL,
                  "On-Line/Local"));
            ControlStateDict.Add(5, new ControlState(ControlStates.REMOTE,
                  "On-Line/Remote"));
        }
        protected override void InitializeProcessStates()
        {
            ProcessStateDict.Add(0, new ProcessState(ProcessStates.NOTREADY,
                  "Off"));
            ProcessStateDict.Add(1, new ProcessState(ProcessStates.NOTREADY,
                 "Setup"));
            ProcessStateDict.Add(2, new ProcessState(ProcessStates.READY,
                 "Ready"));
            ProcessStateDict.Add(3, new ProcessState(ProcessStates.EXECUTING,
                  "Executing"));
            ProcessStateDict.Add(4, new ProcessState(ProcessStates.NOTREADY,
                  "Wait"));
            ProcessStateDict.Add(5, new ProcessState(ProcessStates.NOTREADY,
                  "Abort"));
        }
        public Evatec()
        {
            var numPorts = 0;
            numPorts = CurrentToolConfig.Loadports.Length; 
            Ports = new List<Port>();
                for(int i = 0; i < numPorts; i++)
                {
                    Ports.Add(new Port()); 
                }
 
            dispatcherTimer.Tick += new EventHandler(startTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            Task.Delay(4000).ContinueWith(_ => StartTimers()); 
        }      

#if DEBUG
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        TestData testWords = new TestData();
        private static bool bStarted = false; 

        public void StartTimers(int seconds=1)
        {
            if (!bStarted)
            {               
                dispatcherTimer.Start();
                bStarted = true; 
            }
        }
        
        private void startTimer_Tick(object sender, EventArgs e)
        {
            //if (++nextStep % 3 == 0)
             //   Messenger.Default.Send(new EventMessage(testWords.GetNewLogEntry()));
            //if (++nextStep % 8 == 0)
            //    Messenger.Default.Send(new EventMessage(testWords.GetNewLogEntry("A")));
          //if (nextStep % 7 == 0)
            //    Messenger.Default.Send(new EventMessage(DateTime.Now, "PM", "Processing..."));
            //if (nextStep % 20 == 0)
            //    Messenger.Default.Send(new EventMessage(DateTime.Now, "C", "Completed"));
        }
#endif
       
        public override bool Initialize()
        {
            if (SendCommunicate())
            {
                if (SendAreYouThere())
                {
                    if(QueryProcessAndControlState())
                    {
                        if (SendEnableAlarms())
                        {
                            // Initialization succeeded!
                            MyLog.Information("Initialization sequence was successful.");
                            return true;
                        }
                    }
                }
            }
            return false;
         }

        protected override bool DoToolSpecificStartSequence(string Port, string[] LotIds, string Recipe)
        {
            char[] trimChars = new char[] { ',' };

            if (LotIds == null || Recipe == null)
            {
                string message = "HCMD for PP-SELECT cannot be run--no LotIds or Recipe were supplied";
                Messenger.Default.Send(new EventMessage(DateTime.Now, "E", message));
                Messenger.Default.Send(new EventMessage(new LogEntry(message)));

                return false;
            }
            StringBuilder logMessage = new StringBuilder("Sending HCMD PP-SELECT with lot IDs ");
            S2F41 hostCommandMessage = new S2F41(CurrentToolConfig.Toolid);
            hostCommandMessage.SetRCMD("PP-SELECT", DataType.ASC);
            int lotSuffix = 1;
            foreach (string lotId in LotIds)
            {
                hostCommandMessage.addCp("LOTID" + lotSuffix++, DataType.ASC, lotId, DataType.ASC);
                logMessage.Append(lotId);
                logMessage.Append(",");
            }
            try
            {
                 Messenger.Default.Send(new EventMessage(new LogEntry(logMessage.ToString().TrimEnd(trimChars))));

                hostCommandMessage.send(EqSrv, CurrentToolConfig.CommunicationTimeout);
                if (ToolUtilities.checkAck(hostCommandMessage.HCACK))
                {
                    Messenger.Default.Send(new EventMessage(new LogEntry("HCMD for PP-SELECT returned good HCACK")));

                }
                else
                {
                    string message = "HCMD for PP-SELECT returned bad HCACK " + hostCommandMessage.HCACK;
                    Messenger.Default.Send(new EventMessage(DateTime.Now, "E", message));
                    Messenger.Default.Send(new EventMessage(new LogEntry(message)));

                }
            } catch (Exception e)
            {
                ToolUtilities.logMessageSendExceptions("HCMD (S2F41) PP-SELECT", e);
                return false;
            }

            // RCMD = START
            hostCommandMessage = new S2F41(CurrentToolConfig.Toolid);
            hostCommandMessage.SetRCMD("START", DataType.ASC);
            try
            {
                  Messenger.Default.Send(new EventMessage(new LogEntry("Sending HCMD START")));

                hostCommandMessage.send(EqSrv, CurrentToolConfig.CommunicationTimeout);
                if (ToolUtilities.checkAck(hostCommandMessage.HCACK))
                {
                   Messenger.Default.Send(new EventMessage(new LogEntry("HCMD for START returned good HCACK")));

                }
                else
                {
                    string message = "HCMD for START returned bad HCACK " + hostCommandMessage.HCACK;
                    Messenger.Default.Send(new EventMessage(DateTime.Now, "E", message));
                    Messenger.Default.Send(new EventMessage(new LogEntry(message)));

                }
            }
            catch (Exception e)
            {
                ToolUtilities.logMessageSendExceptions("HCMD (S2F41) START", e);
                return false;
            }

            return true;
        }

 
        public override bool QueryProcessAndControlState()
        {
            S1F3 s1f3 = new S1F3(CurrentToolConfig.Toolid);
            s1f3.addSVID(CurrentToolConfig.ControlStateSVID.ToString(), DataType.U4, "ControlStateSVID");
            s1f3.addSVID(CurrentToolConfig.ProcessStateSVID.ToString(), DataType.U4, "ProcessStateSVID");
            try
            {
                MyLog.Information("Sending tool status query for SVID " + CurrentToolConfig.ControlStateSVID + ", " +
                    CurrentToolConfig.ProcessStateSVID);
                s1f3.send(EqSrv, CurrentToolConfig.CommunicationTimeout);
                int controlState;
                int processState;

                if (s1f3.SV.Count != 2)
                {
                    MyLog.Warning("Status query response did not include expected values.");
                    return false;
                }
                        
                if (s1f3.SV[0].Type == DataType.L)
                {
                    MyLog.Warning("The configured SVID '" + CurrentToolConfig.ControlStateSVID + "' for Control State is not recognized by the tool");
                    return false;
                }
                else
                {
                    controlState = Int32.Parse(s1f3.SV[0].Value);
                }

                if (s1f3.SV[1].Type == DataType.L)
                {
                    MyLog.Warning("The configured SVID '" + CurrentToolConfig.ProcessStateSVID + "' for Process State is not recognized by the tool");
                    return false;
                }
                else
                {
                    processState = Int32.Parse(s1f3.SV[1].Value);                
                }

                ControlState newControlState;
                if (ControlStateDict.ContainsKey(controlState))
                {
                    newControlState = ControlStateDict[controlState];
                    Messenger.Default.Send(new ControlStateChangeMessage(newControlState.State, newControlState.Description));
                    MyLog.Debug("Control State changed to " + newControlState.Description);
                }
                else
                {
                    MyLog.Warning("Control State changed to undefined state " + controlState);
                }
                ProcessState newProcessState = null;
                if (ProcessStateDict.ContainsKey(processState))
                {
                    newProcessState = ProcessStateDict[processState];
                    Messenger.Default.Send(new ProcessStateChangeMessage(newProcessState.State, newProcessState.Description));
                }
                else
                {
                    MyLog.Warning("Process State changed to undefined state " + processState);

                }
                MyLog.Information("Status query returned Control State '" + controlState + "' Process State '" + processState + "'");
                return true;
                }
            catch (BoundMessageTimeoutException)
            {
                MyLog.Warning("Tool status query (S1F3) timed out.");
            }
            catch (BoundMessageSendException e)
            {
                MyLog.Warning("Tool status query (S1F3) failed - " + e.Message);
            }
            catch (Exception e)
            {
                MyLog.Warning("Unexpected error in tool status query (S1F3); " + e.Message);
            }
            return false;
        }


        public override bool PossiblyHandleControlStateChangeEvent(string CEID)
        {
            ControlState newControlState = null;
            if (CEID == ControlStateLocalCEID)
            {
                newControlState = ControlStateDict[4];  //TODO: get rid of magic numbers
            } else if (CEID == ControlStateOfflineCEID)
            {
                newControlState = ControlStateDict[1];
            } else if (CEID == ControlStateRemoteCEID)
            {
                newControlState = ControlStateDict[5];
            }
            if (newControlState != null)
            {
                Messenger.Default.Send(new ControlStateChangeMessage(newControlState.State, newControlState.Description));
                MyLog.Information("Control State changed to " + newControlState.Description);
                return true;
            } else
            {
                return false;
            }
        }
    }
}

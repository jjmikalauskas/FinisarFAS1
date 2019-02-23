using Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Common
{
    public class BusyIndicatorMessage
    {
        public bool isBusy { get; set; }
        public string busyMsg { get; set; }
        public BusyIndicatorMessage(bool busy, string busyContent)
        {
            isBusy = busy;
            busyMsg = busyContent;
        }
    }

    public class ToggleAlarmViewMessage
    {
        public bool bVisible = false;

        public ToggleAlarmViewMessage(bool? bVisible = null)
        {
            if (!bVisible.HasValue)
                this.bVisible = bVisible.GetValueOrDefault();
        }
    }

    public class RetrievingWafersMessage
    {
        public bool bVisible = false;

        public RetrievingWafersMessage(bool? bVisible = null)
        {
            if (!bVisible.HasValue)
                this.bVisible = bVisible.GetValueOrDefault();
        }
    }

    public class EventMessage
    {
        public DateTime MsgDateTime;
        public string MsgType;      // So far only A=Alarm, C=Completed, PM=Processing are used  (A, T, L, or E  for Alarm, Trace, Log or Event or PM update)
        public string Message;      // I am going to add E=Error for all errors with the tool 
                                    // And "L" for log entries

        public EventMessage(DateTime dt, string msgType, string message)
        {
            MsgDateTime = dt;
            MsgType = msgType;
            Message = message; 
        }

        public EventMessage(LogEntry logEntry)
        {
            MsgDateTime = logEntry.EventDateTime;
            MsgType = logEntry.EventType;
            Message = logEntry.Message;
        }
    }

    public class OperatorResponseMessage
    {
        public AuthorizationLevel authLevel { get; set; }
        public OperatorResponseMessage(AuthorizationLevel al)
        {
            authLevel = al;
        }
    }

    public class CloseAlarmMessage
    {
        public CloseAlarmMessage()
        {
        }
    }

    public class ToggleLogViewMessage
    {
        public bool bVisible = false;

        public ToggleLogViewMessage(bool? bVisible = null)
        {
            if (!bVisible.HasValue)
                this.bVisible = bVisible.GetValueOrDefault();
        }
    }


    public class SecMsgOperationMessage
    {
        public string newText;

        public SecMsgOperationMessage(string s)
        {
            this.newText = s; 
        }
    }

    public class RenumberWafersMessage
    {
        public RenumberWafersMessage()
        {
        }
    }

    public class MoveWafersMessage
    {
        public int SlotToMove { get; internal set; }

        public MoveWafersMessage(Wafer waferToMove)
        {
            SlotToMove = int.Parse(waferToMove.Slot);
        }
    }

    public class WafersConfirmedMessage
    {
        public bool Confirmed { get; internal set; }

        public WafersConfirmedMessage(bool confirmed)
        {
            this.Confirmed = confirmed;
        }
    }

    public class WafersInGridMessage
    {
        public int? NumberOfWafers { get; internal set; }

        public WafersInGridMessage(int? numWafers)
        {
            this.NumberOfWafers = numWafers.GetValueOrDefault();
        }
    }

    public class SelectedWafersInGridMessage
    {
        public List<Wafer> wafers { get; internal set; }

        public SelectedWafersInGridMessage(List<Wafer> selectedWafers)
        {
            this.wafers = selectedWafers;
        }
    }

    public class CamstarStatusMessage
    {
        public string Availability { get; private set; } = "Offline";
        public string ResourceName { get; private set; } = "Error";
        public string ResourceStateName { get; private set; } = "Error";
        public string ResourceSubStateName { get; private set; } = "Error";

        public bool IsAvailable => Availability == "1";

        public CamstarStatusMessage(string availability, string resourceName, string resourceSubstateName)
        {
            this.Availability = availability;
            this.ResourceName = resourceName;
            this.ResourceSubStateName = resourceSubstateName;
        }

        public CamstarStatusMessage(DataTable dtCamstar)
        {
            try
            {
                Availability = dtCamstar.Rows[0]["Availability"].ToString();
                ResourceName = dtCamstar.Rows[0]["ResourceName"].ToString();
                ResourceStateName = dtCamstar.Rows[0]["ResourceStateName"].ToString();
                ResourceSubStateName = dtCamstar.Rows[0]["ResourceSubStateName"].ToString();
            }
            catch
            {

            }
        }
    }

 

    public class ControlStateChangeMessage
    {
       public Globals.ControlStates ControlState { get; private set; }
        public string Description { get; private set; }

        public ControlStateChangeMessage (Globals.ControlStates newState, string description)
        {
            this.ControlState = newState;
            this.Description = description;
         }
    }
     public class ProcessStateChangeMessage
    {
       public Globals.ProcessStates ProcessState { get; private set; }
        public string Description { get; private set; }

        public ProcessStateChangeMessage(Globals.ProcessStates newState, string description)
        {
            this.ProcessState = newState;
            this.Description = description;
        }
    }
    public class ProcessCompletedMessage
    {
        public string Description { get; private set; }

        public ProcessCompletedMessage(string description)
        {
           this.Description = description;
        }
    }

    public class CloseAndSendEmailMessage
    {
        public string SendTo { get; private set; } = "";
        public string Subject { get; private set; } = "";
        public string EmailBody { get; private set; } = "";

        public CloseAndSendEmailMessage(string sendTo, string subject, string emailBody)
        {
            this.SendTo = sendTo;
            this.Subject = subject;
            this.EmailBody = emailBody;
        }
    }  
}
using Common;
using System.Collections.Generic;
using System.Data;

namespace FinisarFAS1.View
{
    internal class Messages           // shows the specified text in a popup control that fades after a few seconds
    {
        Operator op;
        Tool tool;
        Lot lot;

        public Messages(Operator op, Tool tool, Lot lot)       // sending the text is not yet implemented
        {
            this.op = op;
            this.tool = tool;
            this.lot = lot;
        }
    }

    class BusyIndicatorMessage
    {
        public bool isBusy { get; set; }
        public string busyMsg { get; set; }
        public BusyIndicatorMessage(bool busy, string busyContent)
        {
            isBusy = busy;
            busyMsg = busyContent;
        }
    }

    class ToggleAlarmViewMessage
    {
        public bool bVisible = false;

        public ToggleAlarmViewMessage(bool? bVisible = null)
        {
            if (!bVisible.HasValue)
                this.bVisible = bVisible.GetValueOrDefault();
        }
    }

    class CloseAlarmMessage
    {
        public CloseAlarmMessage()
        {
        }
    }

    class ToggleLogViewMessage
    {
        public bool bVisible = false;

        public ToggleLogViewMessage(bool? bVisible = null)
        {
            if (!bVisible.HasValue)
                this.bVisible = bVisible.GetValueOrDefault();
        }
    }

    class RenumberWafersMessage
    {
        public RenumberWafersMessage()
        {
        }
    }

    class MoveWafersMessage
    {
        public int SlotToMove { get; internal set; }

        public MoveWafersMessage(Wafer waferToMove)
        {
            SlotToMove = int.Parse(waferToMove.Slot);
        }
    }

    class WafersConfirmedMessage
    {
        public bool Confirmed { get; internal set; }

        public WafersConfirmedMessage(bool confirmed)
        {
            this.Confirmed = confirmed;
        }
    }

    class WafersInGridMessage
    {
        public int? NumberOfWafers { get; internal set; }

        public WafersInGridMessage(int? numWafers)
        {
            this.NumberOfWafers = numWafers.GetValueOrDefault(); 
        }
    }

    class SelectedWafersInGridMessage
    {
        public List<Wafer> wafers { get; internal set; }

        public SelectedWafersInGridMessage(List<Wafer> selectedWafers)
        {
            this.wafers = selectedWafers; 
        }
    }

    class CamstarStatusMessage
    {
        public string Availability { get; private set; } = "Offline";
        public string ResourceName { get; private set; } = "Error";
        public string ResourceSubStateName { get; private set; } = "Error";

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
                ResourceSubStateName = dtCamstar.Rows[0]["ResourceSubStateName"].ToString();
            } 
            catch
            {

            }
        }
    }

    class EquipmentStatusMessage
    {
        public string Availability { get; private set; } = "Offline";

        public EquipmentStatusMessage(string availability)
        {
            this.Availability = availability;
        }
    }   

    class CloseEmailWindowMessage
    {
        public string SendTo { get; private set; } = "";
        public string Subject { get; private set; } = "";
        public string EmailBody { get; private set; } = "";

        public CloseEmailWindowMessage(string sendTo, string subject, string emailBody)
        {
            this.SendTo = sendTo;
            this.Subject = subject;
            this.EmailBody = emailBody;
        }
    }

    //class GoToMainWindowMessage
    //{
    //    public bool bVisible;
    //    private string op;
    //    private Tool tool;
    //    private Lot lot;

    //    public GoToMainWindowMessage(string op, Tool tool, Lot lot, bool bVisible)
    //    {
    //        this.op = op;
    //        this.tool = tool;
    //        this.lot = lot;
    //        this.bVisible = bVisible;
    //    }
    //}

    //internal class EntryValuesMessage           // message for sneding the main 3 values around
    //{
    //    public string op;
    //    public Tool tool;
    //    public Lot lot;
    //    public List<Wafer> wafers; 

    //    public EntryValuesMessage(string op, Tool tool, Lot lot, List<Wafer> wafers)       
    //    {
    //        this.op = op;
    //        this.tool = tool;
    //        this.lot = lot;
    //        this.wafers = wafers; 
    //    }
    //}

    //class ShowSearchBlurMessage
    //{
    //    public bool blurOn;
    //    public ShowSearchBlurMessage(bool blur)
    //    {
    //        blurOn = blur;
    //    }
    //}
}
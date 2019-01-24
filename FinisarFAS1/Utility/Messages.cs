﻿using Common;
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

    class ShowAlarmWindowMessage
    {
        public bool bVisible;

        public ShowAlarmWindowMessage(bool bVisible)
        {
            this.bVisible = bVisible;
        }
    }

    class CloseAlarmMessage
    {
        public CloseAlarmMessage()
        {
        }
    }

    class ShowLogWindowMessage
    {
        public bool bVisible;

        public ShowLogWindowMessage(bool bVisible)
        {
            this.bVisible = bVisible;
        }
    }

    class CloseLogMessage
    {
        public CloseLogMessage()
        {
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

    class GoToMainWindowMessage
    {
        public bool bVisible;
        private string op;
        private Tool tool;
        private Lot lot;

        public GoToMainWindowMessage(string op, Tool tool, Lot lot, bool bVisible)
        {
            this.op = op;
            this.tool = tool;
            this.lot = lot;
            this.bVisible = bVisible;
        }
    }

    internal class EntryValuesMessage           // message for sneding the main 3 values around
    {
        public string op;
        public Tool tool;
        public Lot lot;
        public List<Wafer> wafers; 

        public EntryValuesMessage(string op, Tool tool, Lot lot, List<Wafer> wafers)       
        {
            this.op = op;
            this.tool = tool;
            this.lot = lot;
            this.wafers = wafers; 
        }
    }

    class ShowSearchBlurMessage
    {
        public bool blurOn;
        public ShowSearchBlurMessage(bool blur)
        {
            blurOn = blur;
        }
    }
}
using Common;

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

    class ShowEntryWindowMessage
    {
        public bool bVisible;

        public ShowEntryWindowMessage(bool bVisible)
        {
            this.bVisible = bVisible;
        }
    }

    class ShowWaferWindowMessage
    {
        public bool bVisible;
        private string op;
        private Tool tool;
        private Lot lot;

        public ShowWaferWindowMessage(string op, Tool tool, Lot lot, bool bVisible)
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

        public EntryValuesMessage(string op, Tool tool, Lot lot)       
        {
            this.op = op;
            this.tool = tool;
            this.lot = lot;
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
using Common;
using FinisarFAS1.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Tests.MoqTests;

namespace FinisarFAS1.ViewModel
{
    public class EntryViewModel : ViewModelBase
    {
        enum InputType
        {
            Operator,
            Tool,
            Lot,
            Error
        }

        private IMESService _mesService; 

        public EntryViewModel()
        {
            _mesService = new MESCommunications.MESService(new MoqMESService());

            OperatorID = "John Smith";
            ToolID = "6-6-EVAP-01";
            LotAID = "61851-003";
            LotBID = "61851-001";

            setupLot1();

            LotBActive = "true"; 
            LotCActive = "";
            LotDActive = "";

            IsMESRetrieveBusy = false; 
            RegisterForMessages(); 
        }

        private void RegisterForMessages()
        {
            Messenger.Default.Register<Tool>(this, UpdateLoadPortsMsg);
        }

        private void UpdateLoadPortsMsg(Tool msg)
        {
            Ports ports = msg?.Ports;
            LoadPortNames = new ObservableCollection<string> { ports.LoadPort1Name };

            // For the EvaTec, we want Port B to show
            //if (msg.NumberOfLoadPorts > 1)
            //{
            //    PortBActive = true;
            //    LoadPortNames.Add(ports.LoadPort2Name);
            //}
            if (msg.NumberOfLoadPorts > 2)
            {
                PortCActive = true;
                LoadPortNames.Add(ports.LoadPort3Name);
            }
            if (msg.NumberOfLoadPorts > 3)
            {
                PortDActive = true;
                LoadPortNames.Add(ports.LoadPort4Name);
            }
        }

        private bool _portBActive;
        public bool PortBActive
        {
            get { return _portBActive; }
            set
            {
                _portBActive = value;
                RaisePropertyChanged(nameof(PortBActive));
            }
        }

        private bool _portCActive;
        public bool PortCActive
        {
            get { return _portCActive; }
            set
            {
                _portCActive = value;
                RaisePropertyChanged(nameof(PortCActive));
            }
        }

        private bool _portDActive;
        public bool PortDActive
        {
            get { return _portDActive; }
            set
            {
                _portDActive = value;
                RaisePropertyChanged(nameof(PortDActive));
            }
        }

        private InputType GetInputType(string s)
        {
            // Test for Operator pattern first, then lot then tool else return error
            // For now, test for 3 simple names
            if (s.All(c => Char.IsLetter(c) || c==' ')) 
                return InputType.Operator;

            // Test for integer
            // 61851-003
            try
            {
                Match match = Regex.Match(s, @"^\d{5}\-*\d{3}$");
                if (match.Success)   //Int32.TryParse(s, out lotNum)) 
                {
                    return InputType.Lot;
                }
            }
            catch (Exception ex)
            {

            }

            // Test for string length == 5 
            // 6-6-EVAP-01
            // 6-1-ASH-02
            //
            try
            {
                var match = Regex.Match(s, @"^\d+\-\d+\-[A-Za-z]+\-\d{2}$");
                if (match.Success)
                    return InputType.Tool;
            }
            catch (Exception ex)
            {

            }
            

            return InputType.Error;
        }

        private void ProcessInputType(InputType sourceField, InputType iType, string vString)
        {
            if (iType==InputType.Operator)
            {
                OperatorID = vString; 
            }
            else if (iType == InputType.Tool)
            {
                ToolID = vString; 
            }
            else if (iType == InputType.Lot)
            {
                LotAID = vString; 
            }
            else // Error
            {
                    
            }
        }

        #region UI BINDINGS
        private string _operator;
        public string OperatorID
        {
            get { return _operator; }
            set {
                if (GetInputType(value) == InputType.Operator) {
                    _operator = value;
                    var op = _mesService.ValidateUserFromCamstar(value);
                    OperatorStatus = op == false ? "../Images/CheckBoxRed.png" : "../Images/CheckBoxGreen.png";
                }
                else
                {
                    _operator = "";
                    ProcessInputType(InputType.Operator, GetInputType(value), value);
                }
                RaisePropertyChanged(nameof(OperatorID));
            }
        }

        private string _tool;
        public string ToolID
        {
            get { return _tool; }
            set {
                if (GetInputType(value) == InputType.Tool)
                {
                    _tool = value;
                    var tool = _mesService.GetTool(value);
                    ToolStatus = tool == null ? "../Images/CheckBoxRed.png" : "../Images/CheckBoxGreen.png";
                }
                else
                {
                    _tool = "";
                    ProcessInputType(InputType.Tool, GetInputType(value), value);
                }
                RaisePropertyChanged(nameof(ToolID));
            }
        }

        private ObservableCollection<string> _loadPortNames;
        public ObservableCollection<string> LoadPortNames
        {
            get { return _loadPortNames; }
            set
            {
                _loadPortNames = value;
            }
        }

        private string _lot;
        public string LotAID
        {
            get { return _lot; }
            set {
                if (GetInputType(value) == InputType.Lot)
                {
                    _lot = value;
                    var lot = _mesService.GetLot(value);
                    LotStatus = lot == null ? "../Images/CheckBoxRed.png" : "../Images/CheckBoxGreen.png";
                }
                else
                {
                    _lot = "";
                    ProcessInputType(InputType.Lot, GetInputType(value), value);
                }
                RaisePropertyChanged(nameof(LotAID));
            }
        }
       
        private string _lotB;
        public string LotBID {
            get { return _lotB; }
            set {
                if (GetInputType(value) == InputType.Lot)
                {
                    _lotB = value;
                    var lot = _mesService.GetLot(value);
                    LotStatus = lot == null ? "../Images/CheckBoxRed.png" : "../Images/CheckBoxGreen.png";
                }
                else
                {
                    _lotB = "";
                    ProcessInputType(InputType.Lot, GetInputType(value), value);
                }
                RaisePropertyChanged(nameof(LotBID));
            }
        }

        private string _lotC;
        public string LotCID {
            get { return _lotC; }
            set {
                if (GetInputType(value) == InputType.Lot)
                {
                    _lotC = value;
                    var lot = _mesService.GetLot(value);
                    LotStatus = lot == null ? "../Images/CheckBoxRed.png" : "../Images/CheckBoxGreen.png";
                }
                else
                {
                    _lotC = "";
                    ProcessInputType(InputType.Lot, GetInputType(value), value);
                }
                RaisePropertyChanged(nameof(LotCID));
            }
        }

        private string _lotD;
        public string LotDID {
            get { return _lotD; }
            set {
                if (GetInputType(value) == InputType.Lot)
                {
                    _lotD = value;
                    var lot = _mesService.GetLot(value);
                    LotStatus = lot == null ? "../Images/CheckBoxRed.png" : "../Images/CheckBoxGreen.png";
                }
                else
                {
                    _lotD = "";
                    ProcessInputType(InputType.Lot, GetInputType(value), value);
                }
                RaisePropertyChanged(nameof(LotDID));
            }
        }

        private string _lotBActive;
        public string LotBActive {
            get { return _lotBActive; }
            set {
                _lotBActive = value;
                RaisePropertyChanged(nameof(LotBActive));
            }
        }

        private string _lotCActive;
        public string LotCActive {
            get { return _lotCActive; }
            set {
                _lotCActive = value;
                RaisePropertyChanged(nameof(LotCActive));
            }
        }

        private string _lotDActive;
        public string LotDActive {
            get { return _lotDActive; }
            set {
                _lotDActive = value;
                RaisePropertyChanged(nameof(LotDActive));
            }
        }

        private string _actualWaferSlotHeaderText;
        public string ActualWaferSlotHeaderText {
            get { return _actualWaferSlotHeaderText; }
            set { _actualWaferSlotHeaderText = value; RaisePropertyChanged(nameof(ActualWaferSlotHeaderText)); }
        }

        private string _operatorStatus;
        public string OperatorStatus {
            get { return _operatorStatus; }
            set { _operatorStatus = value; RaisePropertyChanged(nameof(OperatorStatus)); }
        }

        private string _toolStatus;
        public string ToolStatus {
            get { return _toolStatus; }
            set { _toolStatus = value; RaisePropertyChanged(nameof(ToolStatus)); }
        }

        private string _lotStatus;
        public string LotStatus {
            get { return _lotStatus; }
            set { _lotStatus = value; RaisePropertyChanged(nameof(LotStatus)); }
        }

        private bool _busy;
        public bool IsMESRetrieveBusy
        {
            get { return _busy; }
            set { _busy = value; RaisePropertyChanged(nameof(IsMESRetrieveBusy)); }
        }

        #region COMMANDS AND HANDLERS

        public ICommand btnClear { get { return new RelayCommand(clearEntryView); } }
        public ICommand btnConfirm { get { return new RelayCommand(confirmEntry); } }
        public ICommand SetupLot1Cmd { get { return new RelayCommand(setupLot1); } }
        public ICommand SetupLot2Cmd { get { return new RelayCommand(setupLot2); } }
        public ICommand SetupLot3Cmd { get { return new RelayCommand(setupLot3); } }
        public ICommand SetupLot4Cmd { get { return new RelayCommand(setupLot4); } }

        #endregion 

        #endregion

        private void clearEntryView()
        {
            OperatorID = "";
            ToolID = "";
            LotAID = ""; 
        }

        private void confirmEntry()
        {
            // Confirm all entries exist in the MES
            IsMESRetrieveBusy = true;
            Wait(2); 
            var op = _mesService.ValidateUserFromCamstar(OperatorID);
            var tool = _mesService.GetTool(ToolID);
            var lot = _mesService.GetLot(LotAID);
            if (lot != null)
                lot.Lot2Name = LotBID; 

            Messenger.Default.Send(new EntryValuesMessage(OperatorID, tool, lot, null));
            Messenger.Default.Send(new GoToMainWindowMessage(OperatorID, tool, lot, true));

            IsMESRetrieveBusy = false;
        }

        private void Wait(double seconds)
        {
            var frame = new DispatcherFrame();
            new Thread((ThreadStart)(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(seconds));
                frame.Continue = false;
            })).Start();
            Dispatcher.PushFrame(frame);
        }

        private void setupLot1()
        {
            ActualWaferSlotHeaderText = "Port 1 Wafer Configuration";
        }

        private void setupLot2()
        {
            ActualWaferSlotHeaderText = "Load Port B Wafer Configuration";
        }

        private void setupLot3()
        {
            ActualWaferSlotHeaderText = "Load Port C Wafer Configuration";
        }

        private void setupLot4()
        {
            ActualWaferSlotHeaderText = "Load Port D Wafer Configuration";
        }

    }
}

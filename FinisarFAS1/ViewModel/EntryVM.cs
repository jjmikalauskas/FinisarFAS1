using Common;
using FinisarFAS1.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
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
        }

        private InputType GetInputType(string s)
        {
            // Test for Operator pattern first, then lot then tool else return error
            // For now, test for 3 simple names
            if (s.Contains("John") || s == "bob" || s == "cindy")
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

        #region COMMANDS AND HANDLERS

        public ICommand btnClear { get { return new RelayCommand(clearEntryView); } }
        public ICommand btnConfirm { get { return new RelayCommand(confirmEntry); } }

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
            var mes = _mesService; // .MESService(new MockMESService());
            var op = mes.ValidateUserFromCamstar(OperatorID);
            var tool = mes.GetTool(ToolID);
            var lot = mes.GetLot(LotAID);

            Messenger.Default.Send(new ShowWaferWindowMessage(OperatorID, tool, lot, true));            
            Messenger.Default.Send(new EntryValuesMessage(OperatorID, tool, lot));            
        }

    }
}

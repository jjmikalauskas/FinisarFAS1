using Common;
using FinisarFAS1.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;

namespace FinisarFAS1.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                // CamstarStatusColor = "Red";
                CurrentRecipe = @"Recipe: Production\6Inch\Contpe\V300";
                CurrentAlarm = "Alarm Id: 63-Chamber pressure low";
                TimeToProcess = false; 
            }
            else
            {
                CurrentRecipe = @"Recipe: Production\6Inch\Contpe\V300";
                CurrentAlarm = "Alarm Id: 63-Chamber pressure low";
                TimeToProcess = false; 
            }

            CamstarStatusColor = "Lime";
            EquipmentStatusColor = "Lime";
            ProcessState = "Started";

            // Register for messages 
            RegisterForMessages();

            SetupToolEnvironment(); 
        }

        private void RegisterForMessages()
        {
            Messenger.Default.Register<EntryValuesMessage>(this, UpdateEntryValuesMsg);
            Messenger.Default.Register<Tool>(this, UpdateLoadPortsMsg);
        }

        private void SetupToolEnvironment()
        {
            // Read from Config 
            CurrentTool = new Tool();
            CurrentTool.ToolId = EquipmentCommunications.Properties.Settings.Default.ToolID;
            CurrentTool.ToolBrand = EquipmentCommunications.Properties.Settings.Default.ToolBrand;
            CurrentTool.NumberOfLoadPorts = EquipmentCommunications.Properties.Settings.Default.LoadPorts;
            CurrentTool.LoadLock = EquipmentCommunications.Properties.Settings.Default.LoadLock;

            CurrentTool.Ports.LoadPort1Name = EquipmentCommunications.Properties.Settings.Default.LoadPort1Name;
            CurrentTool.Ports.LoadPort2Name = EquipmentCommunications.Properties.Settings.Default.LoadPort2Name;

            Messenger.Default.Send<Tool>(CurrentTool); 
        }

        #region PUBLIC VARIABLES
        public int NumberOfLoadPorts;
        public bool LoadLock;
        public string Port1Name; 
        public string Port2Name;
        public Tool CurrentTool; 
        #endregion 

        private void UpdateEntryValuesMsg(EntryValuesMessage msg)
        {
            this.Operator = msg.op;
            this.Tool = msg.tool?.ToolId;
            this.Lot = msg.lot?.Lot1Name + ", " + msg.lot?.Lot2Name ;
        }

        private void UpdateLoadPortsMsg(Tool msg)
        {
            Ports ports = msg?.Ports;
            LoadPortNames = new ObservableCollection<string> { ports.LoadPort1Name };

            if (msg.NumberOfLoadPorts > 1)
            {
                PortBActive = true;
                LoadPortNames.Add(ports.LoadPort2Name);
            }
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

        #region UI BINDINGS

        private bool _TimeToProcess;
        public bool TimeToProcess {
            get { return _TimeToProcess; }
            set {
                _TimeToProcess = value;
                RaisePropertyChanged(nameof(TimeToProcess));
            }
        }

        private string _port1Lot1;
        public string Port1Lot1 {
            get { return _port1Lot1; }
            set {
                _port1Lot1 = value;
                RaisePropertyChanged(nameof(Port1Lot1));
            }
        }

        private string _port1Lot2;
        public string Port1Lot2 {
            get { return _port1Lot2; }
            set {
                _port1Lot2 = value;
                RaisePropertyChanged(nameof(Port1Lot2));
            }
        }

        private string _processState;
        public string ProcessState {
            get { return _processState; }
            set {
                _processState = value;
                RaisePropertyChanged(nameof(ProcessState));
            }
        }

        private string gridData;
        public string GridData {
            get { return gridData; }
            set {
                gridData = value;
            }
        }

        private string _operator;
        public string Operator {
            get { return _operator; }
            set {
                _operator = value;
                RaisePropertyChanged(nameof(Operator));
            }
        }

        private string _tool;
        public string Tool {
            get { return _tool; }
            set {
                _tool = value;
                RaisePropertyChanged(nameof(Tool));
            }
        }

        private string _lot;
        public string Lot {
            get { return _lot; }
            set {
                _lot = value;
                RaisePropertyChanged(nameof(Lot));
            }
        }

        private ObservableCollection<string> _loadPortNames;
        public ObservableCollection<string> LoadPortNames {
            get { return _loadPortNames; }
            set {
                _loadPortNames = value;
            }
        }

        private string _currentAlarm;
        public string CurrentAlarm {
            get { return _currentAlarm; }
            set {
                _currentAlarm = value;
                RaisePropertyChanged("CurrentAlarm");
            }
        }

        private string _currentRecipe;
        public string CurrentRecipe {
            get { return _currentRecipe; }
            set { _currentRecipe = value;
                RaisePropertyChanged("CurrentRecipe");
            }
        }

        private string _camstarStatusColor;
        public string CamstarStatusColor {
            get { return _camstarStatusColor; }
            set {
                _camstarStatusColor = value;
                RaisePropertyChanged("CamstarStatusColor");
            }
        }

        private string _equipmentStatusColor;
        public string EquipmentStatusColor {
            get { return _equipmentStatusColor; }
            set {
                _equipmentStatusColor = value;
                RaisePropertyChanged("EquipmentStatusColor");
            }
        }


        // SCREEN 2
        private List<string> _currentWaferSetup;
        public List<string> CurrentWaferSetup {
            get { return _currentWaferSetup; }
            set { _currentWaferSetup = value;
                RaisePropertyChanged("CurrentWaferSetup");
            }
        }

        private List<string> _actualWaferSetup;
        public List<string> ActualWaferSetup {
            get { return _actualWaferSetup; }
            set {
                _actualWaferSetup = value;
                RaisePropertyChanged("ActualWaferSetup");
            }
        }

        private bool _portBActive;
        public bool PortBActive {
            get { return _portBActive; }
            set {
                _portBActive = value;
                RaisePropertyChanged(nameof(PortBActive));
            }
        }

        private bool _portCActive;
        public bool PortCActive {
            get { return _portCActive; }
            set {
                _portCActive = value;
                RaisePropertyChanged(nameof(PortCActive));
            }
        }

        private bool _portDActive;
        public bool PortDActive {
            get { return _portDActive; }
            set {
                _portDActive = value;
                RaisePropertyChanged(nameof(PortDActive));
            }
        }


        public ICommand ConfirmPort1Cmd => new RelayCommand( () => { TimeToProcess = true; ProcessState = "Confirmed"; });
        public ICommand CancelPort1Cmd => new RelayCommand(cancelPort1CmdHandler);
        public ICommand LoadPortACmd => new RelayCommand(loadPortACmdHandler);
        public ICommand StartCmd => new RelayCommand(startCmdHandler);
        public ICommand StopCmd => new RelayCommand(stopCmdHandler);
        public ICommand PauseCmd => new RelayCommand(pauseCmdHandler);
        public ICommand ResetCmd => new RelayCommand(resetCmdHandler);
        public ICommand ExitHostCmd => new RelayCommand(exitHostCmdHandler);
        public ICommand CamstarCmd => new RelayCommand(camstarCmdHandler);


        private void cancelPort1CmdHandler()
        {

        }

        private void loadPortACmdHandler()
        {
            Messenger.Default.Send(new ShowEntryWindowMessage(true));
        }

        private void startCmdHandler()
        {
            //await dialogService.ShowMessage("Test message to Start", "Start Process Title");
            //DialogMessage dialogMsg = new DialogMessage(ex.Message, null);
            //dialogMsg.Icon = System.Windows.MessageBoxImage.Error;
            //Messenger.Default.Send(dialogMsg);
            ProcessState = "Started";
        }

        private void stopCmdHandler()
        {
            ProcessState = "Stopped";
            Messenger.Default.Send(new ShowEntryWindowMessage(true));
        }

        private void backToWaferView()
        {
            Messenger.Default.Send(new GoToMainWindowMessage(null, null, null, true));
        }

        private void pauseCmdHandler()
        {
            ProcessState = "Paused";
        }

        private void resetCmdHandler()
        {
            TimeToProcess = false;
            ProcessState = "Stopped";
        }

        private void exitHostCmdHandler()
        {
        }

        private void camstarCmdHandler()
        {

        }

        #endregion

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

        ////}
    }
}
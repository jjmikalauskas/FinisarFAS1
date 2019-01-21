using Common;
using FinisarFAS1.Utility;
using FinisarFAS1.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using MESCommunications;
using MESCommunications.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using Tests.MoqTests;

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
        private readonly IDialogService2 dialogService;
        private IMESService _mesService;

        const int MAXROWS = 25;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDialogService2 dialogService)
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                // CamstarStatusColor = "Red";
                CurrentRecipe = @"Production\6Inch\Contpe\V300";
                CurrentAlarm = DateTime.Now.ToLongTimeString() + " 63-Chamber pressure low";
                TimeToProcess = false;
            }
            else
            {
                CurrentRecipe = @"Production\6Inch\Contpe\V300";
                CurrentAlarm = DateTime.Now.ToLongTimeString() + " 63-Chamber pressure low";
                TimeToProcess = false;
            }

            this.dialogService = dialogService;
            //
            // DI the MESService, for now use the Moq
            //
            _mesService = new MESCommunications.MESService(new MoqMESService());

            InitializeSystem(); 
           
            Port1Wafers = CreateEmptyPortRows();

            SetupToolEnvironment();
        }

        private void InitializeSystem()
        {
            RegisterForMessages();
            // Default settings
            GetCurrentStatuses();

            // Set UI bindings
            Started = false;
            TimeToProcess = false;
            IsRecipeOverridable = false;
            RaisePropertyChanged(nameof(AreThereWafers));
            Messenger.Default.Send(new WafersInGridMessage(0));
        }

        private void GetCurrentStatuses()
        {
            DataTable dtCamstar = _mesService.GetResourceStatus("Camstar-DEV", "Server IP:1.1.1.1");
            UpdateCamstarStatusHandler(new CamstarStatusMessage(dtCamstar));

            UpdateEquipmentStatusHandler(new EquipmentStatusMessage("Online/Remote"));

            ProcessState = "Idle";
        }

        private void UpdateCamstarStatusHandler(CamstarStatusMessage msg)
        {
            string tempStatus = "Offline";
            string tempColor = "Red";

            if (msg != null)
            {
                tempStatus = msg.Availability;
                if (msg.Availability.Contains("On"))
                    tempColor = "Lime";
                else
                    tempColor = "Yellow";
            }
            else
            {
                tempStatus = "Offline";
                tempColor = "Red";
            }
            // Only update at end so the colors are not flashing...
            CamstarStatus = tempStatus;
            CamstarStatusColor = tempColor;
        }

        private void UpdateEquipmentStatusHandler(EquipmentStatusMessage msg)
        {
            string tempStatus = "Offline";
            string tempColor = "Red";

            if (msg != null)
            {
                tempStatus = msg.Availability;
                if (msg.Availability.Contains("Remote"))
                    tempColor = "Lime";
                else
                    tempColor = "Yellow";
            }
            else
            {
                tempStatus = "Offline";
                tempColor = "Red";
            }
            // Only update at end so the colors are not flashing...
            EquipmentStatus = tempStatus;
            EquipmentStatusColor = tempColor;
        }

        private void RegisterForMessages()
        {
            Messenger.Default.Register<CamstarStatusMessage>(this, UpdateCamstarStatusHandler);
            Messenger.Default.Register<EquipmentStatusMessage>(this, UpdateEquipmentStatusHandler);
            Messenger.Default.Register<Tool>(this, UpdateLoadPortsMsg);
            Messenger.Default.Register<RenumberWafersMessage>(this, RenumberWafersHandler);
            Messenger.Default.Register<MoveWafersMessage>(this, MoveWafersHandler);
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

        private void MoveWafersHandler(MoveWafersMessage msg)
        {
            int idxToMove = MAXROWS  - msg.SlotToMove ;
            Wafer mtWafer = new Wafer();
            // Remove top 1 if moving up
            Port1Wafers.RemoveAt(0); 
            Port1Wafers.Insert(idxToMove, mtWafer);
            RenumberWafersHandler(null);
            RaisePropertyChanged("Port1Wafers");
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

        private ObservableCollection<Wafer> port1Wafers;
        public ObservableCollection<Wafer> Port1Wafers
        {
            get { return port1Wafers; }
            set
            {
                port1Wafers = value;
                RaisePropertyChanged(nameof(Port1Wafers));
                if (AreThereWafers)
                    Messenger.Default.Send(new WafersInGridMessage(port1Wafers?.Count));
                else
                    Messenger.Default.Send(new WafersInGridMessage(0));
            }
        }


        private ObservableCollection<Wafer> CreateEmptyPortRows(int rowCount = MAXROWS)
        {
            ObservableCollection<Wafer> tempList = new ObservableCollection<Wafer>();
            // rowCount = 1; 
            for (int i = rowCount; i > 0; --i)
            {
                tempList.Add(new Wafer() { Slot = i.ToString() });
            }
            return tempList;
        }

        // PORT 1
        private bool _timeToProcess;
        public bool TimeToProcess {
            get { return _timeToProcess; }
            set {
                _timeToProcess = value;
                RaisePropertyChanged(nameof(TimeToProcess));
                RaisePropertyChanged(nameof(CanRightClick));
                Messenger.Default.Send(new WafersConfirmedMessage(value && AreThereWafers));
            }
        }

        private bool _started;
        public bool Started {
            get { return _started; }
            set {
                _started = value;
                RaisePropertyChanged(nameof(Started));
                RaisePropertyChanged(nameof(IsStoppable));
            }
        }

        private bool _localMode;
        public bool LocalMode {
            get { return _localMode; }
            set {
                _localMode = value;
                RaisePropertyChanged(nameof(LocalMode));
                RaisePropertyChanged(nameof(IsLocal));
            }
        }

        public bool IsLocal => LocalMode; 

        public bool IsStoppable => Started;

        public bool AreThereWafers {
            get {
                if (Port1Wafers == null || Port1Wafers.Count == 0)
                    return false;
                else
                {
                    for (int i=0; i<MAXROWS; ++i)
                    {
                        if (!string.IsNullOrEmpty(port1Wafers[i].WaferID))
                            return true;
                    }
                }
                return false; 
            }
        }

        private bool _isRecipeOverridable;
        public bool IsRecipeOverridable {
            get { return _isRecipeOverridable; }
            set {
                _isRecipeOverridable = value;
                RaisePropertyChanged(nameof(IsRecipeOverridable));
            }
        }

        public bool CanRightClick {
            get {
                
                return !TimeToProcess && AreThereWafers; 
            }
        }

        private string _port1Lot1;
        public string Port1Lot1 {
            get { return _port1Lot1; }
            set {
                if (!string.IsNullOrEmpty(value) && _port1Lot1 != value)
                {
                    var dtWafers = _mesService.GetLotStatus(value); 
                    if (dtWafers!=null)
                    {
                        var wafers = DataHelpers.MakeDataTableIntoWaferList(dtWafers); 
                        AddWafersToGrid(wafers);
                        //RaisePropertyChanged("Port1Wafers");
                    }
                }
                _port1Lot1 = value;
                RaisePropertyChanged(nameof(Port1Lot1));
            }
        }
       
        private string _port1Lot2;
        public string Port1Lot2 {
            get { return _port1Lot2; }
            set {
                if (!string.IsNullOrEmpty(value) && _port1Lot2 != value)
                {
                    var dtWafers = _mesService.GetLotStatus(value);
                    if (dtWafers != null)
                    {
                        var wafers = DataHelpers.MakeDataTableIntoWaferList(dtWafers);
                        _port1Lot2 = value;
                        AddWafersToTopGrid(wafers); 
                        RaisePropertyChanged("Port1Wafers");
                    }
                }
                else
                    _port1Lot2 = value;

                RaisePropertyChanged(nameof(Port1Lot2));
            }
        }

        //  GRID MANIPULATION
        private void AddWafersToTopGrid(List<Wafer> wafers)
        {
            ObservableCollection<Wafer> currentWafers = new ObservableCollection<Wafer>(Port1Wafers);
            ObservableCollection<Wafer> goodList = new ObservableCollection<Wafer>();
            ObservableCollection<Wafer> bottomList = new ObservableCollection<Wafer>();
            int slotNo = 0;

            // Find first top index
            int topIdx = 0;
            for (int i = 0; i < MAXROWS; ++i)
            {
                if (!string.IsNullOrEmpty(currentWafers[i].WaferID))
                {
                    topIdx = i;
                    break;
                }
            }

            // Copy good rows to goodList
            if (topIdx > 0)
            {
                for (int i = topIdx; i < MAXROWS; ++i)
                {
                    bottomList.Add(currentWafers[i]);
                }
            }

            int newListCount = wafers.Count;

            if (topIdx - newListCount < 0)
            {
                var vm = new DialogViewModel("There are too many new wafers to add to current list. Continue?", "Yes", "No");

                bool? result = dialogService.ShowDialog(vm);
                if (result.HasValue && result.GetValueOrDefault() == true)
                {
                }
                Port1Wafers = currentWafers;
                RenumberWafersHandler(null);
                Port1Lot2 = ""; 
                return; 
            }
            
            // Set currentWafers to goodList and
            Port1Wafers = new ObservableCollection<Wafer>(wafers);

            // Add currentwafers to bottom then add in empty then renumber slots
            foreach (var tempwafer in bottomList)
            {
                Port1Wafers.Add(tempwafer);
            }

            // Add in empty slots at top
            slotNo = Port1Wafers.Count;
            for (int i = MAXROWS - slotNo; i > 0; --i)
            {
                Port1Wafers.Insert(0, new Wafer());
            }

            RaisePropertyChanged(nameof(CanRightClick));

            // Renumber
            RenumberWafersHandler(null);
        }

        private void AddWafersToGrid(List<Wafer> wafers)
        {
            ObservableCollection<Wafer> currentWafers = new ObservableCollection<Wafer>(Port1Wafers);
            int slotNo = 0;

            var goodList = currentWafers.ToList().Where(w => !string.IsNullOrEmpty(w.WaferID));
            currentWafers = new ObservableCollection<Wafer>(goodList);

            Port1Wafers = new ObservableCollection<Wafer>(wafers);

            // Add currentwafers to bottom then add in empty then renumber slots
            foreach (var tempwafer in currentWafers)
            {
                Port1Wafers.Add(tempwafer);
            }

            // Add in empty slots at top
            slotNo = Port1Wafers.Count;
            for (int i = MAXROWS - slotNo; i > 0; --i)
            {
                Port1Wafers.Insert(0, new Wafer());
            }

            RaisePropertyChanged(nameof(AreThereWafers));
            RaisePropertyChanged(nameof(CanRightClick));
            // Renumber
            RenumberWafersHandler(null);
        }

        private void RenumberWafersHandler(RenumberWafersMessage msg)
        {
            ObservableCollection<Wafer> currentWafers = new ObservableCollection<Wafer>(Port1Wafers);
            int idx = 0; 
            for (int i = MAXROWS-1; i >= 0; --i)
            {
                currentWafers[idx++].Slot = (i+1).ToString(); 
            }
            Port1Wafers = new ObservableCollection<Wafer>(currentWafers);
        }

        private void SetAllWafersToMovedIn()
        {
            ObservableCollection<Wafer> currentWafers = new ObservableCollection<Wafer>(Port1Wafers);
            for (int i=0; i< currentWafers.Count; ++i)
            {
                if (!string.IsNullOrEmpty(currentWafers[i].WaferID))
                    currentWafers[i].Status = "Moved In"; 
            }
            Port1Wafers = new ObservableCollection<Wafer>(currentWafers);
        }


        private string _processState;
        public string ProcessState {
            get { return "Process State: " + _processState; }
            set {
                _processState = value;
                RaisePropertyChanged(nameof(ProcessState));
            }
        }

        private string _camstarStatus;
        public string CamstarStatus
        {
            get { return _camstarStatus; }
            set
            {
                _camstarStatus = value;
                RaisePropertyChanged(nameof(CamstarStatus));
            }
        }

        private string _equipmentStatus;
        public string EquipmentStatus
        {
            get { return _equipmentStatus; }
            set
            {
                _equipmentStatus = value;
                RaisePropertyChanged(nameof(EquipmentStatus));
            }
        }

        // PORT 2
        //private bool _TimeToProcess2;
        //public bool TimeToProcess2 {
        //    get { return _TimeToProcess2; }
        //    set {
        //        _TimeToProcess2 = value;
        //        RaisePropertyChanged(nameof(TimeToProcess2));
        //    }
        //}

        private string _port2Lot1;
        public string Port2Lot1 {
            get { return _port2Lot1; }
            set {
                _port2Lot1 = value;
                RaisePropertyChanged(nameof(Port2Lot1));
            }
        }

        private string _port2Lot2;
        public string Port2Lot2 {
            get { return _port2Lot2; }
            set {
                _port2Lot2 = value;
                RaisePropertyChanged(nameof(Port2Lot2));
            }
        }

        private Operator Operator = null ; 

        private string _operatorID;
        public string OperatorID {
            get { return _operatorID; }
            set {
                _operatorID = value;
                var op = _mesService.GetOperator(value);
                // Set check box 
                OperatorStatus = op == null ? "../Images/CheckBoxRed.png" : "../Images/CheckBoxGreen.png";
                Operator = op;
                if (Operator.AuthLevel != AuthorizationLevel.Operator)
                    IsRecipeOverridable = true;
                else
                    IsRecipeOverridable = false; 
                RaisePropertyChanged(nameof(OperatorID));
                RaisePropertyChanged(nameof(OperatorLevel));
                RaisePropertyChanged(nameof(OperatorColor));
            }
        }

        //private string _opLevel;
        public string OperatorLevel
        {
            get {
                if (Operator?.AuthLevel == AuthorizationLevel.Engineer)
                    return "Engineer"; 
                else
                    if (Operator?.AuthLevel == AuthorizationLevel.Admin)
                    return "Administator";
                return "Operator";
            }
            //set { _opLevel = value;
            //    RaisePropertyChanged(nameof(OperatorLevel));
            //}
        }

        //private string _opColor;
        public string OperatorColor
        {
            get {
                if (OperatorStatus == "../Images/CheckBoxRed.png")
                    return "red";
                else
                    return "lime";
            }
            //set
            //{
            //    _opColor = value;
            //    RaisePropertyChanged(nameof(OperatorColor));
            //}
        }


        private string _tool;
        public string Tool {
            get { return _tool; }
            set {
                _tool = value;
                var tool1 = _mesService.GetLot(value);
                ToolStatus = tool1 == null ? "../Images/CheckBoxRed.png" : "../Images/CheckBoxGreen.png";
                RaisePropertyChanged(nameof(Tool));
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

        

        // PORT 1 CMDS
        public ICommand ConfirmPort1Cmd => new RelayCommand(confirmPort1CmdHandler);
        public ICommand CancelPort1Cmd => new RelayCommand(cancelPort1CmdHandler);
        //public ICommand LoadPortACmd => new RelayCommand(loadPortACmdHandler);
        public ICommand StartCmd => new RelayCommand(startCmdHandler);
        public ICommand StopCmd => new RelayCommand(stopCmdHandler);
        public ICommand PauseCmd => new RelayCommand(pauseCmdHandler);
        public ICommand AbortCmd => new RelayCommand(abortCmdHandler);

        public ICommand GoLocalCmd => new RelayCommand(goLocalCmdHandler);
        public ICommand GoRemoteCmd => new RelayCommand(goRemoteCmdHandler);
        public ICommand ResetCmd => new RelayCommand(resetCmdHandler);

        // PORT 2 CMDS
  //    public ICommand ConfirmPort2Cmd => new RelayCommand(confirmPort2CmdHandler);
  //    public ICommand CancelPort2Cmd => new RelayCommand(cancelPort2CmdHandler);
  //    //public ICommand LoadPort2Cmd => new RelayCommand(loadPort2CmdHandler);
  //    public ICommand StartPort2Cmd => new RelayCommand(startPort2CmdHandler);
  //    public ICommand StopPort2Cmd => new RelayCommand(stopPort2CmdHandler);
  //    public ICommand PausePort2Cmd => new RelayCommand(pausePort2CmdHandler);
  //    public ICommand ResetPort2Cmd => new RelayCommand(resetPort2CmdHandler);

        public ICommand AlarmListingCmd => new RelayCommand(exitHostCmdHandler);
        public ICommand LogListingCmd => new RelayCommand(exitHostCmdHandler);
        public ICommand ExitHostCmd => new RelayCommand(exitHostCmdHandler);
        public ICommand CamstarCmd => new RelayCommand(camstarCmdHandler);
        public ICommand CloseAlarmCmd => new RelayCommand(closeAlarmCmdHandler);

        // PORT 1 CMD HANDLERS
        private void closeAlarmCmdHandler()
        {
            var vm = new DialogViewModel("Are you sure you want to close this alarm?", "Yes", "No");

            bool? result = dialogService.ShowDialog(vm);
            if (result.HasValue && result.GetValueOrDefault()==true)
            {
                Messenger.Default.Send(new CloseAlarmMessage());
                CurrentAlarm = ""; 
            }
            else
            {

            }
        }

        private void confirmPort1CmdHandler()
        {
            var vm = new DialogViewModel("Are you sure you want to Confirm these lots?", "Yes", "No");

            bool? result = dialogService.ShowDialog(vm);
            if (result.HasValue && result.GetValueOrDefault() == true)
            {
                SetAllWafersToMovedIn(); 
                TimeToProcess = true;
                ProcessState = "Idle";
                // Messenger.Default.Send(new WafersConfirmedMessage(true));
            }
            else
            {

            }
        }

        private void cancelPort1CmdHandler()
        {
            var vm = new DialogViewModel("Are you sure you want to Cancel?", "Yes", "No");
           
            bool? result = dialogService.ShowDialog(vm); 
            if (result.HasValue && result==true)
            {
                Port1Wafers = CreateEmptyPortRows();
                Port1Lot1 = Port1Lot2 = "";
                TimeToProcess = false;
                ProcessState = "Idle";
            }
        }

        private void startCmdHandler()
        {
            Started = true; 
            ProcessState = "In Process";
        }

        private void stopCmdHandler()
        {
            Started = false; 
            ProcessState = "Stopped";
            Messenger.Default.Send(new ShowAlarmWindowMessage(true));
        }

        private void pauseCmdHandler()
        {
            Started = false; 
            ProcessState = "Paused";
        }

        private void abortCmdHandler()
        {
            Started = false;
            ProcessState = "Aborted";
        }

        private void goLocalCmdHandler()
        {
            Started = false;
            LocalMode = true; 
            ProcessState = "Local ONLY";
        }

        private void goRemoteCmdHandler()
        {
            Started = false;
            LocalMode = false; 
            ProcessState = "Remote Online";
        }

        private void resetCmdHandler()
        {
            var vm = new DialogViewModel("Are you sure you want to Reset?", "Yes", "No");

            bool? result = dialogService.ShowDialog(vm);
            if (result.HasValue && result.GetValueOrDefault() == true)
            {
                Port1Wafers = CreateEmptyPortRows();
                Port1Lot1 = Port1Lot2 = "";
                TimeToProcess = false;
                ProcessState = "Reset";
            }
            else
            {

            }
        }

        // PORT 2 CMD HANDLERS
        //private void confirmPort2CmdHandler()
        //{
        //    var vm = new DialogViewModel("Are you sure you want to Confirm these lots?", "Yes", "No");

        //    bool? result = dialogService.ShowDialog(vm);
        //    if (result.HasValue && result.GetValueOrDefault() == true)
        //    {
        //        TimeToProcess = true;
        //        ProcessState2 = "Confirmed";
        //    }
        //    else
        //    {

        //    }
        //}

        //private void cancelPort2CmdHandler()
        //{
        //    var vm = new DialogViewModel("Are you sure you want to Cancel?", "Yes", "No");

        //    bool? result = dialogService.ShowDialog(vm);
        //}

        ////private void loadPort2CmdHandler()
        ////{
        ////    Messenger.Default.Send(new ShowEntryWindowMessage(true));
        ////}

        //private void startPort2CmdHandler()
        //{
        //    Started = true; 
        //    ProcessState2 = "In Process...";
        //}

        //private void stopPort2CmdHandler()
        //{
        //    Started = false; 
        //    ProcessState2 = "Stopped";
        //    // Messenger.Default.Send(new ShowEntryWindowMessage(true));
        //}

        //private void pausePort2CmdHandler()
        //{
        //    ProcessState2 = "Paused";
        //}

        //private void resetPort2CmdHandler()
        //{
        //    var vm = new DialogViewModel("Are you sure you want to reset?", "Yes", "No");

        //    bool? result = dialogService.ShowDialog(vm);
        //    if (result.HasValue && result.GetValueOrDefault() == true)
        //    {
        //        TimeToProcess = false;
        //        ProcessState2 = "Reset";
        //    }
        //    else
        //    {

        //    }
        //}

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
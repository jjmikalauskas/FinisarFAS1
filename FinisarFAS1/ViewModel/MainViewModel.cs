using Common;
using EquipmentCommunications;
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
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ToolService;

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
        private MESService _mesService;

        private Tool currentTool;
     
        const int MAXROWS = 25;

        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        // Test data
        private string thisTool = "6-6-EVAP-002";
        private string thisHostName = "SHM-L10015894";  // "TEX-L10015200"

        private bool showConfirmButtonBox = false;
        private bool allowEmailBody = false;
        private bool showStartMessage = false;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                // CamstarStatusColor = "Red";
                CurrentRecipe = @"Production\6Inch\Contpe\V300";
                CurrentAlarm = DateTime.Now.ToLongTimeString() + " 63-Chamber pressure low";
            }
            else
            {
                CurrentRecipe = @"Production\6Inch\Contpe\V300";
                CurrentAlarm = DateTime.Now.ToLongTimeString() + " 63-Chamber pressure low";
            }

            // Initialize DI objects
            IDialogService2 dialogService1 = new MyDialogService(null);
            dialogService1.Register<DialogViewModel, DialogWindow>();

            this.dialogService = dialogService1;
          
            _mesService = new MESService(); 

            InitializeSystem();               
        }

        private void InitializeSystem()
        {
            GetConfigurationValues(); 

            RegisterForMessages();

            // Default settings
            GetCurrentStatuses();

            CamstarStatusText = EquipmentCommunications.Properties.Settings.Default.CamstarString;
            _startTimerSeconds = EquipmentCommunications.Properties.Settings.Default.StartTimerSeconds;

            Title = "Factory Automation System -" + currentTool.ToolId;
            // Messenger.Default.Send<Tool>(CurrentTool);

            // Set UI bindings
            Started = false;
            TimeToStart = false;
            StartTimerLeft = ""; 
            // RaisePropertyChanged(nameof(StartTimerLeft));
            IsRecipeOverridable = false;
            RaisePropertyChanged(nameof(AreThereWafers));
            Messenger.Default.Send(new WafersInGridMessage(0));

            Port1Wafers = CreateEmptyPortRows();
        }
        
        private void GetConfigurationValues()
        {
            var tc = Common.XMLHelper.ReadToolConfigXml("ToolConfigSample.xml");

            SystemConfig sc = Common.XMLHelper.ReadSysConfigXml("SystemConfigExample.xml");
            showConfirmButtonBox = sc.Dialogs.ShowConfirmationBox;
            allowEmailBody = sc.Dialogs.ShowEmailBox;
            showStartMessage = sc.Dialogs.ShowStartMessageBox;
        }       

        private void GetCurrentStatuses()
        {
            var q = _mesService.Initialize(Globals.MESConfigDir + Globals.MESConfigFile,  thisHostName);

            // Update CamStar first       
            DataTable dtCamstar = _mesService.GetResourceStatus(thisTool);
            UpdateCamstarStatusHandler(new CamstarStatusMessage(dtCamstar));

            // Get Tool status 
            var equip = new EvaTech();
            var equipStatus = "Offline";
            if (equip.AreYouThere(null))
                equipStatus = "Online:Remote";
            currentTool = equip.SetupToolEnvironment();
            Messenger.Default.Send(currentTool);

#if RELEASE
            // Get Tool status #2 with the ToolService project
            string eqsvr = "_eqSvr";
            int timeout = 15; 
            var equip2 = new Evatec(thisTool);
            equip2.Initialize(eqsvr, timeout);
#endif 
            UpdateEquipmentStatusHandler(new EquipmentStatusMessage(equipStatus));

            ProcessState = "Idle";
        }

        private void UpdateCamstarStatusHandler(CamstarStatusMessage msg)
        {
            string tempStatus = "Offline";
            string tempColor = "Red";

            if (msg != null)
            {
                tempColor = msg.IsAvailable ? "Lime" : "Yellow" ;
                tempStatus = msg.ResourceStateName;
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
            Messenger.Default.Register<CloseEmailWindowMessage>(this, CloseEmailResponseMsgHandler);            
        }
     

        private void MoveWafersHandler(MoveWafersMessage msg)
        {
            int idxToMove = MAXROWS - msg.SlotToMove;
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
            LoadPortNames = new ObservableCollection<string> {  ports.LoadPort1Name };

            // This probably needs to be a dictionary! 
            // I should not rely on sequence...
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

#region UI BINDINGS

        private ObservableCollection<Wafer> port1Wafers;
        public ObservableCollection<Wafer> Port1Wafers {
            get { return port1Wafers; }
            set {
                port1Wafers = value;
                RaisePropertyChanged(nameof(Port1Wafers));
                if (AreThereWafers)
                    Messenger.Default.Send(new WafersInGridMessage(port1Wafers?.Count));
                else
                    Messenger.Default.Send(new WafersInGridMessage(0));
            }
        }       

        // PORT 1
        private bool _timeToProcess;
        public bool TimeToStart {
            get { return _timeToProcess; }
            set {
                _timeToProcess = value;
                RaisePropertyChanged(nameof(TimeToStart));
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
                    for (int i = 0; i < MAXROWS; ++i)
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

                return !TimeToStart && AreThereWafers;
            }
        }

        private string _port1Lot1;
        public string Port1Lot1 {
            get { return _port1Lot1; }
            set {
                if (!string.IsNullOrEmpty(value) && _port1Lot1 != value)
                {
                    var dtWafers = _mesService.GetContainerStatus(value);
                    if (dtWafers != null)
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
                    var dtWafers = _mesService.GetContainerStatus(value);
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

        private string _processState;
        public string ProcessState {
            get { return "Process State: " + _processState; }
            set {
                _processState = value;
                RaisePropertyChanged(nameof(ProcessState));
            }
        }

        private string _title;
        public string Title {
            get { return _title; }
            set { _title = value;
                RaisePropertyChanged(nameof(Title));
            }
        }

        private string _camstarStatusText;
        public string CamstarStatusText {
            get { return _camstarStatusText; }
            set {
                _camstarStatusText = value;
                RaisePropertyChanged(nameof(CamstarStatusText));
            }
        }

        private string _startTimerSeconds;
        public int StartTimerSeconds {
            get {
                int seconds = 0;
                if (Int32.TryParse(_startTimerSeconds, out seconds))
                    return seconds;
                return 0; 
            }
        }
         
        private string _camstarStatus;
        public string CamstarStatus {
            get { return _camstarStatus; }
            set {
                _camstarStatus = value;
                RaisePropertyChanged(nameof(CamstarStatus));
            }
        }

        private string _equipmentStatus;
        public string EquipmentStatus {
            get { return _equipmentStatus; }
            set {
                _equipmentStatus = value;
                RaisePropertyChanged(nameof(EquipmentStatus));
            }
        }
     
        // private Operator Operator = null;

        private string _operatorID;
        public string OperatorID {
            get { return _operatorID; }
            set {
                _operatorID = value;
                AuthorizationLevel authLevel = _mesService.ValidateEmployee(value);
                // Set check box 
                OperatorStatus = authLevel != AuthorizationLevel.InvalidUser ? "../Images/CheckBoxRed.png" : "../Images/CheckBoxGreen.png";
                //if (Operator.AuthLevel != AuthorizationLevel.Operator)
                    IsRecipeOverridable = true;
                // else IsRecipeOverridable = false;
                OperatorLevel = authLevel.ToString(); 
                RaisePropertyChanged(nameof(OperatorID));
                RaisePropertyChanged(nameof(OperatorLevel));
                RaisePropertyChanged(nameof(OperatorColor));
            }
        }

        private string _opLevel;
        public string OperatorLevel { 
            get {
                //if (Operator?.AuthLevel == AuthorizationLevel.Engineer)
                //    return "Engineer";
                //else
                //    if (Operator?.AuthLevel == AuthorizationLevel.Admin)
                //    return "Administator";
                return _opLevel;
            }
            set {
                _opLevel = value;
                RaisePropertyChanged(nameof(OperatorLevel));
            }
        }


        //private string _opColor;
        public string OperatorColor {
            get {
                if (OperatorStatus == "../Images/CheckBoxRed.png")
                    return "red";
                else
                    return "lime";
            }            
        }


        private string _tool;
        public string Tool {
            get { return _tool; }
            set {
                _tool = value;
                bool goodTool;
                if (value == thisTool) goodTool = true;
                else
                    goodTool = false; 
                ToolStatus = !goodTool ? "../Images/CheckBoxRed.png" : "../Images/CheckBoxGreen.png";
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

        public AlarmViewModel AlarmVM => new AlarmViewModel(); 
        public LogViewModel LogVM => new LogViewModel(); 


#region GRID MANIPULATION
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
            for (int i = MAXROWS - 1, idx = 0; i >= 0; --i, ++idx)
            {
                string newSlot = (i + 1).ToString();
                currentWafers[idx].Slot = newSlot;
            }
            Port1Wafers = new ObservableCollection<Wafer>(currentWafers);
        }

        private void SetAllWafersToMovedIn()
        {
            ObservableCollection<Wafer> currentWafers = new ObservableCollection<Wafer>(Port1Wafers);
            for (int i = 0; i < currentWafers.Count; ++i)
            {
                if (!string.IsNullOrEmpty(currentWafers[i].WaferID))
                    currentWafers[i].Status = "Moved In";
            }
            Port1Wafers = new ObservableCollection<Wafer>(currentWafers);
        }

        public List<Wafer> SelectedWafers { get; set; }

        private List<Wafer> GetAscendingListOfWafersBySlot(List<Wafer> selectedWafers)
        {
            List<Wafer> orderedList;
            if (selectedWafers?.Count > 1)
                orderedList = selectedWafers.OrderBy(w => Int32.Parse(w.Slot)).Reverse().ToList();
            else
                orderedList = new List<Wafer>(selectedWafers);
            return orderedList;
        }

        private void moveUpCmdHandler()
        {
            int? cnt = SelectedWafers?.Count;
            if (cnt == null || cnt <= 0) return;

            // Remove from list 
            List<Wafer> moveWafers = GetAscendingListOfWafersBySlot(SelectedWafers);

            moveWafers.ForEach(wafer =>
              {
                  Port1Wafers.Remove(wafer);
                  Port1Wafers.Insert(MAXROWS - Int32.Parse(wafer.Slot) - 1, wafer);
              });
            //for (int i = 0; i < cnt.GetValueOrDefault(); ++i)
            //{
            //    //int idx = Int32.Parse(moveWafers[i].Slot)-1;
            //    //Port1Wafers.RemoveAt(idx);                
            //    Port1Wafers.Remove(moveWafers[i]);
            //}

            // Reinsert at slot # which is 1 up
            //for (int i = 0; i < cnt.GetValueOrDefault(); ++i)
            //{
            //    int idx = Int32.Parse(moveWafers[i].Slot);
            //    moveWafers[i].Status = "Moved to " + idx.ToString();
            //    Port1Wafers.Insert(idx, moveWafers[i]);
            //}

            RenumberWafersHandler(null);
            Messenger.Default.Send(new SelectedWafersInGridMessage(moveWafers));
        }

        private void moveDownCmdHandler()
        {
            int? cnt = SelectedWafers?.Count;
            if (cnt == null || cnt <= 0) return;

            // Remove from list 
            List<Wafer> moveWafers = GetAscendingListOfWafersBySlot(SelectedWafers);
            if (moveWafers.Last().Slot == "1")
                return;

            moveWafers.Reverse();
            moveWafers.ForEach(wafer =>
            {
                Port1Wafers.Remove(wafer);
                Port1Wafers.Insert(MAXROWS - Int32.Parse(wafer.Slot) + 1, wafer);
            });


            RenumberWafersHandler(null);
            Messenger.Default.Send(new SelectedWafersInGridMessage(moveWafers));
        }

        private void addEmptyRowCmdHandler()
        {
            int? cnt = SelectedWafers?.Count;
            if (cnt == null || cnt <= 0) return;

            // Remove from list 
            List<Wafer> moveWafers = GetAscendingListOfWafersBySlot(SelectedWafers);

            moveWafers.ForEach(wafer =>
            {
                Messenger.Default.Send(new MoveWafersMessage(wafer));
            });            
        }

#endregion

        // PORT 1 CMDS
        public ICommand ConfirmPort1Cmd => new RelayCommand(confirmPort1CmdHandler);
        public ICommand CancelPort1Cmd => new RelayCommand(cancelPort1CmdHandler);

        public ICommand MoveUpCmd => new RelayCommand(moveUpCmdHandler);
        public ICommand MoveDownCmd => new RelayCommand(moveDownCmdHandler);
        public ICommand AddEmptyRowCmd => new RelayCommand(addEmptyRowCmdHandler);

        public ICommand StartCmd => new RelayCommand(startCmdHandler);
        public ICommand StopCmd => new RelayCommand(stopCmdHandler);
        public ICommand PauseCmd => new RelayCommand(pauseCmdHandler);
        public ICommand AbortCmd => new RelayCommand(abortCmdHandler);

        public ICommand GoLocalCmd => new RelayCommand(goLocalCmdHandler);
        public ICommand GoRemoteCmd => new RelayCommand(goRemoteCmdHandler);

        public ICommand CloseAlarmCmd => new RelayCommand(closeAlarmCmdHandler);
        public ICommand AlarmListingCmd => new RelayCommand(alarmListingCmdHandler);
        public ICommand LogListingCmd => new RelayCommand(logListingCmdHandler);

        public ICommand CamstarCmd => new RelayCommand(camstarCmdHandler);
        public ICommand ResetHostCmd => new RelayCommand(resetHostCmdHandler);
        public ICommand ExitHostCmd => new RelayCommand(exitHostCmdHandler);
     
        private int startTimerLeft;

        private string _startTimerLeft = "";
        public string StartTimerLeft {
            get { return "(" + _startTimerLeft + ")"; }
            set { _startTimerLeft = value; RaisePropertyChanged(nameof(StartTimerLeft)); }
        }

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
            bool? result;
            if (showConfirmButtonBox)
            {
                var vm = new DialogViewModel("Are you sure you want to Confirm these lots?", "Yes", "No");
                result = dialogService.ShowDialog(vm);
            }
            else
                result = true; 
            
            if (result.HasValue && result.GetValueOrDefault() == true)
            {
                SetAllWafersToMovedIn();
                TimeToStart = true;
                ProcessState = "Idle";
                StartTimer(StartTimerSeconds);
            }
            else
            {

            }
        }

        private void StartTimer(int seconds)
        {
            dispatcherTimer.Tick += new EventHandler(startTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            startTimerLeft = seconds;
            StartTimerLeft = seconds.ToString(); 
            dispatcherTimer.Start();
        }

        private void StopTimer()
        {
            dispatcherTimer.Stop();
            dispatcherTimer.Tick -= new EventHandler(startTimer_Tick);
        }

        private void cancelPort1CmdHandler()
        {
            var vm = new DialogViewModel("Are you sure you want to Cancel?", "Yes", "No");
           
            bool? result = dialogService.ShowDialog(vm); 
            if (result.HasValue && result==true)
            {
                ReInitializeSystem(1);
            }
        }

        private void startCmdHandler()
        {
            StopTimer();
            var vm = new DialogViewModel("Please make sure the door is closed and the ports are ready", "Yes", "No");

            bool? result = dialogService.ShowDialog(vm);
            if (result.HasValue && result.GetValueOrDefault() == true)
            {
                
            }
            else
            {

            }
            Started = true; 
            ProcessState = "In Process";
        }

        private void startTimerExpiredHandler()
        {
            StopTimer();
            var vm = new DialogViewModel("The timer has expired to press Start. This lot will be placed onHold in Camstar now.", "", "Ok");
            bool? result = dialogService.ShowDialog(vm);
            if (result.HasValue && result.GetValueOrDefault() == true) {
            }
            else {
            }
            emailViewHandler("Start Timer expired");
            ReInitializeSystem();
        }

        private void startTimer_Tick(object sender, EventArgs e)
        {
            --startTimerLeft;
            if (startTimerLeft <= 0)
            {
                StopTimer();
                startTimerExpiredHandler();
            }
            else
            {
                StartTimerLeft = startTimerLeft.ToString();
            }
        }

        private void stopCmdHandler()
        {
            StopTimer();
            Started = false; 
            ProcessState = "Stopped";
            // Not sure why I ever called this? Messenger.Default.Send(new ToggleAlarmViewMessage());
        }

        private void pauseCmdHandler()
        {
            Started = false; 
            ProcessState = "Paused";
        }

        private void abortCmdHandler()
        {
            StopTimer();
            var vm = new DialogViewModel("Are you sure you want to Abort?", "Yes", "No");
            bool? result = dialogService.ShowDialog(vm);
            if (result.HasValue && result.GetValueOrDefault() == true)
            {
                Started = false;
                ProcessState = "Aborted";
                ReInitializeSystem();
            }
            else
            {

            }
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

        private void alarmListingCmdHandler()
        {
            Messenger.Default.Send(new ToggleAlarmViewMessage());
        }

        private void logListingCmdHandler()
        {
            Messenger.Default.Send(new ToggleLogViewMessage());
        }

        private void resetHostCmdHandler()
        {
            var vm = new DialogViewModel("Are you sure you want to Reset Host?", "Yes", "No");
            bool? result = dialogService.ShowDialog(vm);
            if (result.HasValue && result.GetValueOrDefault() == true)
            {
                emailViewHandler("Resetting Host"); 
                ReInitializeSystem();
            }
            else
            {

            }
        }

        private void emailViewHandler(string eventText)
        {
            string bodyText = $"{DateTime.Now.ToString()} EVENT VALUES: Operator:{OperatorID} Tool:{Tool}" + Environment.NewLine;
            bodyText += $"Lot1:{Port1Lot1}";
            if (!string.IsNullOrEmpty(Port1Lot2)) bodyText += $" Lot2:{Port1Lot2}";

            var vm = new EmailViewModel("ZahirHague@FinisarCorp.com", eventText, bodyText);
            var view = new EmailView() { DataContext = vm, WindowStartupLocation = WindowStartupLocation.CenterScreen };
            view.ShowDialog();
        }


        private void CloseEmailResponseMsgHandler(CloseEmailWindowMessage msg)
        {
            if (!string.IsNullOrEmpty(msg.SendTo))
            {
                string s = $"FAS would be sending email to {msg.SendTo} about {msg.Subject} with body {msg.EmailBody}";
                var vm = new DialogViewModel(s, "Yes", "No");
                bool? result = dialogService.ShowDialog(vm);
            }
            else
            {
                var vm = new DialogViewModel("Operator cancelled the sending of email", "Yes", "No");
                bool? result = dialogService.ShowDialog(vm);
            }
        }
       
        private void ReInitializeSystem(int level=0)
        {            
            if (level == 0)
            {
                OperatorID = "";
                Tool = "";
                CurrentRecipe = "";
            }
            Port1Wafers = CreateEmptyPortRows();
            Port1Lot1 = Port1Lot2 = "";
            TimeToStart = false;
            StartTimerLeft = "";
            ProcessState = "Reset";
            RaisePropertyChanged(nameof(AreThereWafers));
            RaisePropertyChanged(nameof(CanRightClick));
        }

        private void exitHostCmdHandler()
        {
            var vm = new DialogViewModel("Are you sure you want to Exit Host?", "Yes", "No");
            bool? result = dialogService.ShowDialog(vm);
            if (result.HasValue && result.GetValueOrDefault() == true)
            {
                emailViewHandler("Resetting Host");
                // Exit application 
            }
            else
            {

            }
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
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ToolService;
using static Common.Globals;
using Serilog;
using System.Net.Mail;
using System.Net;

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

        // private Evatec currentEquipment;
        SECSHandler<Evatec> currentTool;

        const int MAXROWS = 25;

        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        DispatcherTimer processingTimer = new DispatcherTimer();

        // Test data
        private string thisHostName = "SHM-L10015894";  // "TEX-L10015200" 
        string factoryName = "Wafer";

#if DEBUG
        TestData testWords = new TestData(); 
#endif

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            MyLog = new FASLog();
            // Initialize DI objects
            IDialogService2 dialogService1 = new MyDialogService(null);
            dialogService1.Register<DialogViewModel, DialogWindow>();
            dialogService = dialogService1;

            _mesService = new MESService();

            InitializeSystem();
        }

        private void InitializeSystem()
        {
            MyLog.Timing("Starting InitializeSystem()...");
            GetConfigurationValues();

            RegisterForMessages();

            MyLog.Timing("Starting SetupMESCommunication()...");
            SetupMESCommunication();

            MyLog.Timing("Starting SetupEquipment()...");
            SetupEquipment();
            MyLog.Timing("Done SetupEquipment()...");

            // Initialize some UI vlaues
            CamstarStatusText = CurrentToolConfig.CamstarString;
            _startTimerSeconds = CurrentToolConfig.StartTimerSeconds;

            Title = "Factory Automation System: " + CurrentToolConfig.Toolid;

            // Set UI bindings
            Started = false;
            IsProcessing = false;
            TimeToStart = false;
            StartTimerLeft = "";
            IsRecipeOverridable = false;
            RaisePropertyChanged(nameof(AreThereWafers));
            Messenger.Default.Send(new WafersInGridMessage(0));

            Port1Wafers = CreateEmptyPortRows();
#if DEBUG
            OperatorID = "Mike";
            ToolID = CurrentToolConfig.Toolid;
            //Enumerable.Range(0, 20)
            //          .ToList()
            //          .ForEach(x => Messenger.Default.Send(new EventMessage(testWords.GetNewLogEntry())));
            //UpdateControlStateHandler(new ControlStateChangeMessage(ControlStates.REMOTE, "Online Remote"));
#endif
        }

        private void GetConfigurationValues()
        {
            ReadXmlConfigs();
            LoadPortNames = new ObservableCollection<string>();
            foreach (string portName in CurrentToolConfig.Loadports)
            {
                LoadPortNames.Add(portName);
            }
        }

        private void SetupMESCommunication()
        {
            var q = _mesService.Initialize(MESDefaultConfigDir + MESDefaultConfigFile, thisHostName);

            // Update CamStar first       
            DataTable dtCamstar = _mesService.GetResourceStatus(CurrentToolConfig.Toolid);
            UpdateCamstarStatusHandler(new CamstarStatusMessage(dtCamstar));
        }

        private void SetupEquipment()
        { 
            // Get ToolID status #2 with the ToolService project
            string _eq = CurrentToolConfig.Toolid;
            // TODO remove, get from configuration int timeout = 15;

            currentTool = new SECSHandler<Evatec>(new Evatec());
            //currentTool.InitializeTool(); 
        }

        private void UpdateCamstarStatusHandler(CamstarStatusMessage msg)
        {
            string tempStatus = "Offline";
            string tempColor = "Red";

            if (msg != null)
            {
                tempColor = msg.IsAvailable ? "Lime" : "Yellow";
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

        private void UpdateControlStateHandler(ControlStateChangeMessage msg)
        {
            string tempStatus = msg.Description;
            string tempColor;

            if (msg.ControlState == ControlStates.REMOTE)
            {
                tempStatus = "Online: Remote";
                tempColor = "Lime";
            }
            else
            if (msg.ControlState == ControlStates.LOCAL)
            {
                tempStatus = "Online: Local";
                tempColor = "Yellow";
            }
            else
            if (msg.ControlState == ControlStates.OFFLINE)
            {
                tempStatus = "Offline";
                tempColor = "Red";
            }
            else
            {
                tempStatus = "Unknown";
                tempColor = "DodgerBlue";
            }

            EquipmentStatus = tempStatus;
            EquipmentStatusColor = tempColor;
        }

        private void UpdateProcessStateHandler(ProcessStateChangeMessage msg)
        {
            string tempStatus = msg.Description;
            string tempColor = "White";

            //if (msg.ProcessState == ProcessStates.Compete)
            //    tempColor = "Lime";
            //else
            if (msg.ProcessState == ProcessStates.EXECUTING)
                tempColor = "Yellow";
            else
                if (msg.ProcessState == ProcessStates.NOTREADY)
                tempColor = "Red";
            else
                if (msg.ProcessState == ProcessStates.READY)
                tempColor = "Azure"; 

            ProcessState = msg.Description;
            ProcessStateColor = tempColor;
        }

        private void RegisterForMessages()
        {
            Messenger.Default.Register<CamstarStatusMessage>(this, UpdateCamstarStatusHandler);
            Messenger.Default.Register<ControlStateChangeMessage>(this, UpdateControlStateHandler);
            Messenger.Default.Register<ProcessStateChangeMessage>(this, UpdateProcessStateHandler);
            Messenger.Default.Register<RenumberWafersMessage>(this, RenumberWafersHandler);
            Messenger.Default.Register<MoveWafersMessage>(this, MoveWafersHandler);
            Messenger.Default.Register<CloseAndSendEmailMessage>(this, CloseEmailResponseMsgHandler);
            Messenger.Default.Register<EventMessage>(this, EventMessageHandler);
            Messenger.Default.Register<SecMsgOperationMessage>(this, SecsMessageHandler);
            Messenger.Default.Register<OperatorResponseMessage>(this, UpdateOperatorMsgHandler);
        }

        private void UpdateOperatorMsgHandler(OperatorResponseMessage msg)
        {
            AuthorizationLevel authLevel = msg.authLevel;
            BusyOp = false;
            OperatorStatus = authLevel == AuthorizationLevel.InvalidUser ? "../Images/CheckBoxRed.png" : "../Images/CheckBoxGreen.png";
            if (authLevel == AuthorizationLevel.InvalidUser)
            {
                if (!string.IsNullOrEmpty(OperatorID))
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        var vm = new DialogViewModel("Invalid Operator ID entered! Please re-enter", "", "Ok");
                        dialogService.ShowDialog(vm);
                    });
                }
            }
            else
            {
                IsRecipeOverridable = true;
                // else IsRecipeOverridable = false;
            }
            OperatorLevel = authLevel.ToString();
            RaisePropertyChanged(nameof(OperatorID));
            RaisePropertyChanged(nameof(OperatorLevel));
            RaisePropertyChanged(nameof(OperatorColor));
        }


        private void SecsMessageHandler(SecMsgOperationMessage msg)
        {
            SecMsgOperation = msg.newText;
        }

        private string _secsMsg;
        public string SecMsgOperation
        {
            get { return _secsMsg; }
            set { _secsMsg = value; RaisePropertyChanged(nameof(SecMsgOperation)); }
        }

        private void EventMessageHandler(EventMessage msg)
        {
            // TODO : Quick hack to get running
            if (msg.Message.Contains("Complet")) msg.MsgType = "C"; 

            if (msg.MsgType == "A")
            {
                int len = msg.Message.Length > 80 ? 80 : msg.Message.Length;
                string s = msg.Message.Substring(0, len);
                CurrentAlarm = msg.MsgDateTime + "-" + s;
            }
            else if (msg.MsgType == "C")
            {
                // Update wafer status as Complete
                // Then moveOut wafer
                UpdateWaferStatus("Completed");
                LotStatusColor = "Complete";
                Completed = true;
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    MoveOutWafers();
                });
                
            }
            else if (msg.MsgType == "PM")
            {
                if (msg.Message.Contains("Processing"))
                {
                    UpdateWaferStatus("In Processing");
                    LotStatusColor = "Processing";
                }
            }
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

        // PORT 1
        private bool _timeToProcess;
        public bool TimeToStart
        {
            get { return _timeToProcess; }
            set
            {
                _timeToProcess = value;
                RaisePropertyChanged(nameof(TimeToStart));
                RaisePropertyChanged(nameof(CanRightClick));
                Messenger.Default.Send(new WafersConfirmedMessage(value && AreThereWafers));
            }
        }

        private bool _confirmed;
        public bool Confirmed
        {
            get { return _confirmed; }
            set
            {
                _confirmed = value;
                RaisePropertyChanged(nameof(Confirmed));
                // RaisePropertyChanged(nameof(IsStoppable));
            }
        }

        private bool _started;
        public bool Started
        {
            get { return _started; }
            set
            {
                _started = value;
                RaisePropertyChanged(nameof(Started));
                RaisePropertyChanged(nameof(IsStoppable));
            }
        }

        private bool _isProcessing;
        public bool IsProcessing
        {
            get { return _isProcessing; }
            set
            {
                _isProcessing = value;
                RaisePropertyChanged(nameof(IsProcessing));
                // RaisePropertyChanged(nameof(IsStoppable));
            }
        }


        private bool _localMode;
        public bool LocalMode
        {
            get { return _localMode; }
            set
            {
                _localMode = value;
                RaisePropertyChanged(nameof(LocalMode));
                RaisePropertyChanged(nameof(IsLocal));
            }
        }

        public bool IsLocal => LocalMode;

        public bool IsStoppable => Started;

        public bool AreThereWafers
        {
            get
            {
                if (Port1Wafers == null || Port1Wafers.Count == 0)
                {                    
                    return false;
                }
                else
                {
                    for (int i = 0; i < MAXROWS; ++i)
                    {
                        if (!string.IsNullOrEmpty(port1Wafers[i].WaferNo))
                        {                            
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private bool _isRecipeOverridable;
        public bool IsRecipeOverridable
        {
            get { return _isRecipeOverridable; }
            set
            {
                _isRecipeOverridable = value;
                RaisePropertyChanged(nameof(IsRecipeOverridable));
            }
        }

        public bool CanRightClick
        {
            get
            {

                return !TimeToStart && AreThereWafers;
            }
        }

        private string _port1Lot1;
        public string Port1Lot1
        {
            get { return _port1Lot1; }
            set
            {
                if (!string.IsNullOrEmpty(value) && _port1Lot1 != value)
                {
                    LoadingWafers = true;
                    var dtWafers = _mesService.GetContainerStatus(value);
                    if (dtWafers != null)
                    {
                        var wafers = DataHelpers.MakeDataTableIntoWaferList(dtWafers);
                        AddWafersToGrid(wafers);
                        CurrentRecipe = wafers[0].Recipe;
                    }
                }
                _port1Lot1 = value;
                RaisePropertyChanged(nameof(CanConfirm));
                RaisePropertyChanged(nameof(Port1Lot1));
                Task.Delay(1000).ContinueWith(_ =>
                {
                    LoadingWafers = false; 
                });
            }
        }

        private string _port1Lot2;
        public string Port1Lot2
        {
            get { return _port1Lot2; }
            set
            {
                if (!string.IsNullOrEmpty(value) && _port1Lot2 != value)
                {
                    LoadingWafers = true;
                    var dtWafers = _mesService.GetContainerStatus(value);
                    if (dtWafers != null)
                    {
                        var wafers = DataHelpers.MakeDataTableIntoWaferList(dtWafers);
                        if (wafers[0].Recipe != CurrentRecipe)
                        {
                            var vm = new DialogViewModel("ERROR: Recipe mismatch! Cannot use different recipes between lots!", "", "Ok");
                            dialogService.ShowDialog(vm);
                            _port1Lot2 = "";
                        }
                        else
                        {
                            _port1Lot2 = value;
                            AddWafersToTopGrid(wafers);
                            RaisePropertyChanged("Port1Wafers");
                        }
                    }
                }
                else
                    _port1Lot2 = value;
                RaisePropertyChanged(nameof(Port1Lot2));
                Task.Delay(1000).ContinueWith(_ =>
                {
                    LoadingWafers = false;
                });
            }
        }

        private string _processState;
        public string ProcessState
        {
            get { return "Process State: " + _processState; }
            set
            {
                _processState = value;
                RaisePropertyChanged(nameof(ProcessState));
            }
        }

        private string _processStateColor;
        public string ProcessStateColor
        {
            get { return _processStateColor; }
            set
            {
                _processStateColor = value;
                RaisePropertyChanged(nameof(ProcessStateColor));
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(nameof(Title));
            }
        }

        private string _camstarStatusText;
        public string CamstarStatusText
        {
            get { return _camstarStatusText; }
            set
            {
                _camstarStatusText = value;
                RaisePropertyChanged(nameof(CamstarStatusText));
            }
        }

        private uint _startTimerSeconds;
        public uint StartTimerSeconds
        {
            get
            {
               return _startTimerSeconds;
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

        private string _operatorID;
        public string OperatorID
        {
            get { return _operatorID; }
            set
            {
                _operatorID = value;
                if (!string.IsNullOrEmpty(value))
                {
                    BusyOp = true;
                    AuthorizationLevel authLevel; // = _mesService.ValidateEmployee(value);
#if DEBUG
                    Task.Delay(1100).ContinueWith(_ =>
                    {
                        authLevel = _mesService.ValidateEmployee(value);
                        Messenger.Default.Send(new OperatorResponseMessage(authLevel));
                    });
#else
                        authLevel = new MESDLL().ValidateEmployee(value);
                        Messenger.Default.Send(new OperatorResponseMessage(authLevel));
#endif
                }                
                RaisePropertyChanged(nameof(OperatorID));
                RaisePropertyChanged(nameof(OperatorLevel));
                RaisePropertyChanged(nameof(OperatorColor));
            }
        }

        private bool _busyOp;
        public bool BusyOp
        {
            get { return _busyOp; }
            set { _busyOp = value; RaisePropertyChanged(nameof(BusyOp)); }
        }

        private bool _loadingWafers;
        public bool LoadingWafers
        {
            get { return _loadingWafers; }
            set { _loadingWafers = value; RaisePropertyChanged(nameof(LoadingWafers)); }
        }


        private string _opLevel;
        public string OperatorLevel
        {
            get
            {                
                return _opLevel;
            }
            set
            {
                _opLevel = value;
                RaisePropertyChanged(nameof(OperatorLevel));
            }
        }

        private string _lotStatusColor; 
        public string LotStatusColor
        {
            get { return _lotStatusColor; }
            set
            {
                string processingStep = value; 
                _lotStatusColor = Globals.GetColor(processingStep);
                RaisePropertyChanged(nameof(LotStatusColor));
            }
        }
        //private string _opColor;
        public string OperatorColor
        {
            get
            {
                if (OperatorStatus == "../Images/CheckBoxRed.png")
                    return "red";
                else
                    return "lime";
            }
        }

        private string _tool;
        public string ToolID
        {
            get { return _tool; }
            set
            {
                _tool = value;
                if (string.IsNullOrEmpty(value))
                {
                    RaisePropertyChanged(nameof(ToolID));
                    return;
                }
                if (value == CurrentToolConfig.Toolid)
                {
                    ToolStatus = "../Images/CheckBoxGreen.png";
                }
                else
                {
                    var vm = new DialogViewModel("Invalid Tool ID entered! Please re-enter", "", "Ok");
                    dialogService.ShowDialog(vm);
                    ToolStatus = "../Images/CheckBoxRed.png" ; 
                }                
                RaisePropertyChanged(nameof(ToolID));
            }
        }

        private string _operatorStatus;
        public string OperatorStatus
        {
            get { return _operatorStatus; }
            set {
                _operatorStatus = value;               
                RaisePropertyChanged(nameof(OperatorStatus));
            }
        }

        private string _toolStatus;
        public string ToolStatus
        {
            get { return _toolStatus; }
            set {
                _toolStatus = value;
                RaisePropertyChanged(nameof(CanConfirm));
                RaisePropertyChanged(nameof(ToolStatus));
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

        private string _currentAlarm;
        public string CurrentAlarm
        {
            get { return _currentAlarm; }
            set
            {
                _currentAlarm = value;
                RaisePropertyChanged("CurrentAlarm");
            }
        }

        private string _currentRecipe;
        public string CurrentRecipe
        {
            get { return _currentRecipe; }
            set
            {
                _currentRecipe = value;
                RaisePropertyChanged("CurrentRecipe");
            }
        }

        private string _camstarStatusColor;
        public string CamstarStatusColor
        {
            get { return _camstarStatusColor; }
            set
            {
                _camstarStatusColor = value;
                RaisePropertyChanged("CamstarStatusColor");
            }
        }

        private string _equipmentStatusColor;
        public string EquipmentStatusColor
        {
            get { return _equipmentStatusColor; }
            set
            {
                _equipmentStatusColor = value;
                RaisePropertyChanged("EquipmentStatusColor");
            }
        }

        // SCREEN 2
        private List<string> _currentWaferSetup;
        public List<string> CurrentWaferSetup
        {
            get { return _currentWaferSetup; }
            set
            {
                _currentWaferSetup = value;
                RaisePropertyChanged("CurrentWaferSetup");
            }
        }

        private List<string> _actualWaferSetup;
        public List<string> ActualWaferSetup
        {
            get { return _actualWaferSetup; }
            set
            {
                _actualWaferSetup = value;
                RaisePropertyChanged("ActualWaferSetup");
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

        public bool CanConfirm
        {
            get
            {
                bool okToConfirm = true;
                // If there is no Operator or OperatorLevel is invalid 
                // OR ToolStatus = null or ToolStatus = "red"
                // TODO: Need to check for wafers prior to confirming...
                if (OperatorLevel==null || OperatorLevel.Equals("InvalidUser") ||
                    ToolStatus==null || ToolStatus == "../Images/CheckBoxRed.png" 
                //  ||  !AreThereWafers
                )
                    okToConfirm = false;
                return okToConfirm;
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
                if (!string.IsNullOrEmpty(currentWafers[i].WaferNo))
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

            var goodList = currentWafers.ToList().Where(w => !string.IsNullOrEmpty(w.WaferNo));
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
            RaisePropertyChanged(nameof(CanConfirm));
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

#region MES CALLS FOR MOVEIN, MOVEOUT and HOLD
        private bool MoveInWafers()
        {
            string errMessage = "";
            string comment = "";
            bool movedIn1 = false;
            bool movedIn2 = false;

            // Try to movein lot1
            // if there is an error, display and return 
            // if ok, then display moved in for lot1, and try to move in lot2
            // if there is an error, display error 
            // return from method
            //
            //SHMMoveIn(string sContainer, ref string strErrMsg, 
            //                  bool RequiredCertification, string sEmployee = "", 
            //                  string sComments = "", string sResource = "", string sFactory = "");
            movedIn1 = _mesService.MoveIn(Port1Lot1, ref errMessage, false, OperatorID, comment, ToolID, factoryName);
            if (movedIn1 == false)
            {
                string errString = $"Error moving in lot #{Port1Lot1}: {errMessage}";
                var vm = new DialogViewModel(errString, "", "Ok");
                dialogService.ShowDialog(vm);
                SetAllWafersStatus(Port1Lot1, "ERROR on MoveIn");
                MyLog.Error(errString);
                return false;
            }
            else
            {
                SetAllWafersStatus(Port1Lot1, WaferStatus.MovedIn.ToString());
            }

            if (!string.IsNullOrEmpty(Port1Lot2))
            {
                movedIn2 = _mesService.MoveIn(Port1Lot2, ref errMessage, false, OperatorID, comment, ToolID, factoryName);
                if (movedIn2 == false)
                {
                    string errString = $"Error moving in lot #{Port1Lot2}: {errMessage}";
                    var vm = new DialogViewModel(errString, "", "Ok");
                    dialogService.ShowDialog(vm);
                    SetAllWafersStatus(Port1Lot2, "ERROR on MoveIn");
                    MyLog.Error(errString);
                    return false;
                }
                else
                {
                    SetAllWafersStatus(Port1Lot2, WaferStatus.MovedIn.ToString());
                }
            }
            return true; 
        }

        private bool MoveOutWafers()
        {
            string errMessage = "";
            string comment = "MoveOut at Complete for Lot #" + Port1Lot1 ;
            bool movedOut1 = false;
            bool movedOut2 = false;

            // Try to moveout lot1
            // if there is an error, display and return 
            // if ok, then display moved out for lot1, and try to move out lot2
            // if there is an error, display error 
            // return from method
            //
            //SHMMoveOut(string sContainer, ref string strErrMsg, 
            //                  bool RequiredCertification, string sEmployee = "", 
            //                  string sComments = "");
            movedOut1 = _mesService.MoveOut(Port1Lot1, ref errMessage, false, OperatorID, comment);
            if (movedOut1 == false)
            {
                string errString = $"Error moving out lot #{Port1Lot1}: {errMessage}";
                var vm = new DialogViewModel(errString, "", "Ok");
                dialogService.ShowDialog(vm);
                SetAllWafersStatus(Port1Lot1, "ERROR on MoveOut");
                MyLog.Error(errString);
                return false;
            }
            else
            {
                SetAllWafersStatus(Port1Lot1, WaferStatus.MovedOut.ToString());
            }

            if (!string.IsNullOrEmpty(Port1Lot2))
            {
                comment = "MoveOut at complete for Lot #" + Port1Lot2;
                movedOut2 = _mesService.MoveOut(Port1Lot2, ref errMessage, false, OperatorID, comment);
                if (movedOut2 == false)
                {
                    string errString = $"Error moving out lot #{Port1Lot2}: {errMessage}";
                    var vm = new DialogViewModel(errString, "", "Ok");
                    dialogService.ShowDialog(vm);
                    SetAllWafersStatus(Port1Lot2, "ERROR on MoveOut");
                    MyLog.Error(errString);
                    return false;
                }
                else
                {
                    SetAllWafersStatus(Port1Lot2, WaferStatus.MovedOut.ToString());
                }
            }
            return true;
        }

        private bool HoldWafers(string holdReason)
        {
            string errMessage = "";
            string comment = "";
            bool holdLot1 = false;
            bool holdLot2 = false;

            // Try to hold lots
            // if there is an error, display and return 
            // if ok, then display Hold for lot1, and try to Hold lot2
            // if there is an error, display error 
            // return from method
            //
            // bool SHMHold(string container, string holdReason, ref string errorMsg,
            //              string comment, string factory, string employee, string resourceName);

            holdLot1 = _mesService.Hold(Port1Lot1,  holdReason, ref errMessage, comment, factoryName, OperatorID, ToolID);
            if (holdLot1 == false)
            {
                string errString = $"Error putting lot #{Port1Lot1} on Hold: {errMessage}";
                var vm = new DialogViewModel(errString, "", "Ok");
                dialogService.ShowDialog(vm);
                SetAllWafersStatus(Port1Lot1, "ERROR on Hold");
                MyLog.Error(errString);
                return false;
            }
            else
            {
                SetAllWafersStatus(Port1Lot1, WaferStatus.Hold.ToString());
            }

            if (!string.IsNullOrEmpty(Port1Lot2))
            {
                holdLot2 = _mesService.Hold(Port1Lot1, holdReason, ref errMessage, comment, factoryName, OperatorID, ToolID);

                if (holdLot2 == false)
                {
                    string errString = $"Error putting lot #{Port1Lot2} on Hold: {errMessage}";
                    var vm = new DialogViewModel(errString, "", "Ok");
                    dialogService.ShowDialog(vm);
                    SetAllWafersStatus(Port1Lot2, "ERROR on Hold");
                    MyLog.Error(errString);
                    return false;
                }
                else
                {
                    SetAllWafersStatus(Port1Lot2, WaferStatus.Hold.ToString());
                }
            }
            return true;
        }
#endregion

        private void SetAllWafersStatus(string lotId, string waferStatus)
        {
            ObservableCollection<Wafer> currentWafers = new ObservableCollection<Wafer>(Port1Wafers);
            for (int i = 0; i < currentWafers.Count; ++i)
            {
                var cw = currentWafers[i];
                if (!string.IsNullOrEmpty(cw.ContainerName) && cw.ContainerName.Equals(lotId))
                    cw.Status = waferStatus ;
            }
            Port1Wafers = new ObservableCollection<Wafer>(currentWafers);
        }

        private void UpdateWaferStatus(string newStatus)
        {
            string testStatus = newStatus.ToUpper();
            ObservableCollection<Wafer> currentWafers = new ObservableCollection<Wafer>(Port1Wafers);
            for (int i = 0; i < currentWafers.Count; ++i)
            {
                if (!string.IsNullOrEmpty(currentWafers[i].WaferNo) && !currentWafers[i].Status.Contains("Completed"))
                {
                    if (testStatus.Contains("ABORT"))
                        currentWafers[i].Status += "-" + newStatus;
                    else
                        currentWafers[i].Status = newStatus;
                }
            }
            Port1Wafers = new ObservableCollection<Wafer>(currentWafers);
            LotStatusColor = newStatus;            
        }

        private bool _completed;
        public bool Completed
        {
            get { return _completed; }
            set { _completed = value; RaisePropertyChanged(nameof(Completed)); }
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
        public ICommand CompleteCmd => new RelayCommand(completeCmdHandler);

        public ICommand GoLocalCmd => new RelayCommand(goLocalCmdHandler);
        public ICommand GoRemoteCmd => new RelayCommand(goRemoteCmdHandler);

        public ICommand CloseAlarmCmd => new RelayCommand(closeAlarmCmdHandler);
        public ICommand AlarmListingCmd => new RelayCommand(alarmListingCmdHandler);
        public ICommand LogListingCmd => new RelayCommand(logListingCmdHandler);

        public ICommand CamstarCmd => new RelayCommand(camstarCmdHandler);
        public ICommand ResetHostCmd => new RelayCommand(resetHostCmdHandler);
        public ICommand ExitHostCmd => new RelayCommand(exitHostCmdHandler);

        private uint startTimerLeft;

        private string _startTimerLeft = "";
        public string StartTimerLeft
        {
            get {
                if (!string.IsNullOrWhiteSpace(_startTimerLeft))
                   return "(" + _startTimerLeft + ")";
                else
                   return "";
            }
            set { _startTimerLeft = value; RaisePropertyChanged(nameof(StartTimerLeft)); }
        }

        public void  completeCmdHandler()
        {
            ReInitializeSystem();
        }

        // PORT 1 CMD HANDLERS
        public void closeAlarmCmdHandler()
        {
            //var vm = new DialogViewModel("Are you sure you want to close this alarm?", "Yes", "No");

            //bool? result = dialogService.ShowDialog(vm);
            //if (result.HasValue && result.GetValueOrDefault() == true)
            //{
                Messenger.Default.Send(new CloseAlarmMessage());
                CurrentAlarm = "";
            //}
            //else
            //{

            //}
        }
        private void confirmPort1CmdHandler()
        {
            bool? result = true; 
            if (CurrentToolConfig.Dialogs.ShowConfirmationBox)
            {
                var vm = new DialogViewModel("Are you sure you want to Confirm these lots?", "Yes", "No");
                result = dialogService.ShowDialog(vm);
            }

            if (result.HasValue && result.GetValueOrDefault() == true)
            {
                if (MoveInWafers())
                {
                    TimeToStart = true;
                    Confirmed = true;
                    StartTimers(StartTimerSeconds);
                }
            }
            else
            {

            }
        }

        private void StartTimers(uint seconds)
        {
            dispatcherTimer.Tick += new EventHandler(startTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            startTimerLeft = seconds;
            StartTimerLeft = seconds.ToString();
            dispatcherTimer.Start();

        }

#if DEBUG
        private int nextStep = 0; 
        private void newStageEvent(object sender, EventArgs e)
        {
            ++nextStep;
            if (nextStep==5)
            {
                UpdateWaferStatus("In Processing");
                LotStatusColor = "Processing";
            }
            else
            if (nextStep == 30)
            {
                UpdateWaferStatus("Completed");
                LotStatusColor = "Complete";
            }
            //Messenger.Default.Send(new EventMessage(testWords.GetNewLogEntry()));
            //if (nextStep%23==0)
            //    Messenger.Default.Send(new EventMessage(testWords.GetNewLogEntry("A")));
        }
#endif

        private void StopTimer()
        {
            dispatcherTimer.Stop();
            dispatcherTimer.Tick -= new EventHandler(startTimer_Tick);
            StartTimerLeft = "";
        }

        private void cancelPort1CmdHandler()
        {
            var vm = new DialogViewModel("Are you sure you want to Cancel?", "Yes", "No");

            bool? result = dialogService.ShowDialog(vm);
            if (result.HasValue && result == true)
            {
                ReInitializeSystem(1);
            }
        }

        private void startCmdHandler()
        {
            StopTimer();
            bool? result = true; 

            if (!string.IsNullOrEmpty(CurrentToolConfig.Dialogs.PostStartmessage))
            {
                var vm = new DialogViewModel(CurrentToolConfig.Dialogs.PostStartmessage, "", "Ok");
                result = dialogService.ShowDialog(vm);
            }

            if (result.HasValue && result.GetValueOrDefault() == true)
            {
                SecMessageHelpers.SendSECSStart(); 
            }
            else
            {

            }
            Started = true;
            IsProcessing = true; ;
#if DEBUG
            processingTimer.Tick += new EventHandler(newStageEvent);
            processingTimer.Interval = new TimeSpan(0, 0, 0, 10, 200);
            nextStep = 0;
            processingTimer.Start();
#endif
        }

        private void startTimerExpiredHandler()
        {
            StopTimer();
            var vm = new DialogViewModel("The timer has expired to press Start. This lot will be placed onHold in Camstar now.", "", "Ok");
            bool? result = dialogService.ShowDialog(vm);
            //if (result.HasValue && result.GetValueOrDefault() == true)
            //{
            //}
            //else
            //{
            //}
            HoldWafers("Start Timer ran out by: " + OperatorID);
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
            SecMessageHelpers.SendSECSStop();
            Started = false;
            IsProcessing = false;
        }

        private void pauseCmdHandler()
        {
            SecMessageHelpers.SendSECSPause();
            Started = false;
            IsProcessing = false;
        }

        private void abortCmdHandler()
        {
            StopTimer();
            var vm = new DialogViewModel("Are you sure you want to Abort?", "Yes", "No");
            bool? result = dialogService.ShowDialog(vm);
            if (result.HasValue && result.GetValueOrDefault() == true)
            {
                SecMessageHelpers.SendSECSAbort();
                Started = false;
                IsProcessing = false;
                HoldWafers("Aborted by: " + OperatorID);
                UpdateWaferStatus("Aborted");

                //ReInitializeSystem();
                // Messenger.Default.Send(new SecMsgOperationMessage("SECS Abort messages=> Equipment"));
            }
            else
            {

            }
        }
        private void goLocalCmdHandler()
        {
            SecMessageHelpers.SendSECSGoLocal();
            Started = false;
            IsProcessing = false;
            LocalMode = true;
            // Not setting awaiting equip status update ProcessState = "Local ONLY";
        }

        private void goRemoteCmdHandler()
        {
            SecMessageHelpers.SendSECSGoRemote();
            Started = false;
            IsProcessing = false; 
            LocalMode = false;
            // Not setting awaiting equip status update from tool ProcessState = "Remote Online";
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
                if (AreThereWafers)
                    HoldWafers("ResetHost by: " + OperatorID);
                emailViewHandler("Resetting Host");
                ReInitializeSystem();
            }
            else
            {

            }
        }

        private string GetEmailAddresses()
        {
            string emails = "";
            foreach (string emailAddress in CurrentToolConfig.PrimaryEmailAddressees)
                emails += emailAddress + ";";
            return emails; 
        }

#region EMAIL HANDLERS
        private void emailViewHandler(string subject)
        {
            string sendTo = GetEmailAddresses();
            string bodyText = $"{DateTime.Now.ToString()} EVENT VALUES: Operator:{OperatorID} ToolID:{ToolID}" + Environment.NewLine;
            bodyText += $"Lot1:{Port1Lot1}";
            if (!string.IsNullOrEmpty(Port1Lot2)) bodyText += $" Lot2:{Port1Lot2}";

            if (CurrentToolConfig.Dialogs.ShowEmailBox)
            {
                var vm = new EmailViewModel(sendTo, subject, bodyText);
                var view = new EmailView() { DataContext = vm, WindowStartupLocation = WindowStartupLocation.CenterScreen };
                view.ShowDialog();
            }
            else
                Messenger.Default.Send(new CloseAndSendEmailMessage(sendTo, subject, bodyText));
        }

        private void CloseEmailResponseMsgHandler(CloseAndSendEmailMessage msg)
        {
            if (!string.IsNullOrEmpty(msg.SendTo))
            {
                string fromAddress = CurrentSystemConfig.FromEmailAddress;
                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromAddress);
                message.Subject = msg.Subject;
                message.Body = msg.EmailBody;

                foreach (var address in msg.SendTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    message.To.Add(address);
                }

                SmtpClient client = new SmtpClient(CurrentSystemConfig.EmailServer, CurrentSystemConfig.EmailPort);
                client.Timeout = 100;
                // Credentials are necessary if the server requires the client 
                // to authenticate before it will send e-mail on the client's behalf.
                client.Credentials = CredentialCache.DefaultNetworkCredentials;

                try
                {
                    // client.Send(message);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, ex.Message);
                }
            }
            else
            {
                var vm = new DialogViewModel("Operator cancelled the sending of email", "", "Ok");
                bool? result = dialogService.ShowDialog(vm);
            }
        }
#endregion 

        private void exitHostCmdHandler()
        {
            var vm = new DialogViewModel("Are you sure you want to Exit Host?", "Yes", "No");
            bool? result = dialogService.ShowDialog(vm);
            if (result.HasValue && result.GetValueOrDefault() == true)
            {
                emailViewHandler("Exiting Host");
                if (Globals.AshlServer != null)
                    AshlServer.requestStopAndJoin();
                if (TheTraceDataCollector != null)
                {
                    TheTraceDataCollector.CloseDataFile();
                    TheTraceDataCollector = null;
                }

                Application.Current.Shutdown();
            }
            else
            {

            }
        }

        // Testing as complete
        private void camstarCmdHandler()
        {
            string htmLink = CurrentSystemConfig.CamstarURL;
            
            try
            {
                 System.Diagnostics.Process.Start(htmLink);
            }
            catch (Exception ex)
            {
                MyLog.Error(ex, ex.Message);
            }

        }

        #endregion

        private void ReInitializeSystem(int level = 0)
        {
            if (level == 0)
            {
                OperatorID = "";
                ToolID = "";
                CurrentRecipe = "";
            }
            Port1Wafers = CreateEmptyPortRows();
            Port1Lot1 = Port1Lot2 = "";
            Confirmed = false;
            TimeToStart = false;
            StartTimerLeft = "";
            Completed = false;
            Started = false;
            LotStatusColor = "Ready";
            RaisePropertyChanged(nameof(AreThereWafers));
            RaisePropertyChanged(nameof(CanRightClick));
            RaisePropertyChanged(nameof(CanConfirm));
        }
    }    
}
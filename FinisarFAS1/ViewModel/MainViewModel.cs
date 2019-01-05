using FinisarFAS1.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System.Collections.Generic;
using System.Windows.Input;

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
            }
            else
            {
                CurrentRecipe = @"Recipe: Production\6Inch\Contpe\V300";
                CurrentAlarm = "Alarm Id: 63-Chamber pressure low";
            }
            // Regist for messages 
            Messenger.Default.Register<EntryValuesMessage>(this, UpdateEntryValuesMsg);

        }

        private void UpdateEntryValuesMsg(EntryValuesMessage msg)
        {
            this.Operator = msg.op?.OperatorName;
            this.Tool = msg.tool?.ToolName;
            this.Lot = msg.lot?.LotInfo;
        }


        #region UI BINDINGS
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


        public ICommand StartCmd => new RelayCommand(startCmdHandler);
        public ICommand StopCmd => new RelayCommand(stopCmdHandler);
        public ICommand PauseCmd => new RelayCommand(pauseCmdHandler);
        public ICommand ExitHostCmd => new RelayCommand(exitHostCmdHandler);
        public ICommand CamstarCmd => new RelayCommand(camstarCmdHandler);

        private void startCmdHandler()
        {
            //await dialogService.ShowMessage("Test message to Start", "Start Process Title");
            //DialogMessage dialogMsg = new DialogMessage(ex.Message, null);
            //dialogMsg.Icon = System.Windows.MessageBoxImage.Error;
            //Messenger.Default.Send(dialogMsg);
        }

        private void stopCmdHandler()
        {
            Messenger.Default.Send(new ShowEntryWindowMessage(true));
        }

        private void backToWaferView()
        {
            Messenger.Default.Send(new ShowWaferWindowMessage(null, null, null, true));
        }

        private void pauseCmdHandler()
        {
        }

        private void exitHostCmdHandler()
        {
        }

        private void camstarCmdHandler()
        {

        }

        #endregion
        ////}
    }
}
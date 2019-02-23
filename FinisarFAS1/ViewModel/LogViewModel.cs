using Common;
using FinisarFAS1.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;

namespace FinisarFAS1.ViewModel
{
    // This is not really used anymore - will delete some other time still not sure whether to use it or not
    public class LogViewModel : ViewModelBase
    {
        public LogViewModel()
        {
            LogText = "OperationsLog: 12/05/18: 11:40:01: operator started process. Operator ID: Mike\n" +
                "OperationsLog: 12/05/18: 11:40:01: Process start confirmed\n" +
                "SECSlog: 12/05/18: 11:45:02: Received S6F11 from EVAP-01\n" +
"SECSlog: 12 / 05 / 18: 11:45:02: S6F12,   sxid 0, H->E, 0x10\n" +
"SECSlog: 12 / 05 / 18: 11:45:02:     (BIN)'00'\n" +
"SECSlog: 12 / 05 / 18: 11:45:02: Sent S6F12 to EVAP-01gw\n" +
"SECSlog: 12 / 05 / 18: 11:46:20: S7F19 W, sxid 0, H->E, 0x1e\n" +
"SECSlog: 12 / 05 / 18: 11:46:20: Sent S7F19 to EVAP-01\n" +
"SECSlog: 12 / 05 / 18: 11:46:20: S7F20,   sxid 0, H < -E, 0x1e\n" +
"SECSlog: 12 / 05 / 18: 11:46:20:     L,3\n" +
"                                       (ASC) 'F01'\n" +
"                                       (ASC) 'Particle_qual'\n" +
"                                       (ASC) 'Shield_Coat'\n" +
"SECSlog: 12 / 05 / 18: 11:46:20: Received S7F20 from EVAP-01\n" +
"SECSlog: 12 / 05 / 18: 11:51:43: Sent S2F37 to EVAP-01 - SXSEND ERROR 20\n" +
"SECSlog: 12 / 05 / 18: 11:51:48: S6F11 W, sxid 0, H < -E, 0x11\n" ;
                                       
        }

        private string _logText;
        public string LogText {
            get { return _logText; }
            set { _logText = value;
                RaisePropertyChanged(nameof(LogText));
            }
        }

        public ICommand CloseLogCmd => new RelayCommand(closeLogHandler);

        private void closeLogHandler()
        {
            Messenger.Default.Send(new ToggleLogViewMessage(false));
        }
        
    }
}

using FinisarFAS1.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinisarFAS1.ViewModel
{
    public class LogViewModel : ViewModelBase
    {
        public LogViewModel()
        {
            LogText = "Green juice meditation hoodie, helvetica fanny pack wolf YOLO retro. Crucifix health goth distillery, four dollar toast lomo iPhone narwhal intelligentsia pour-over try-hard poke VHS retro. Fixie helvetica activated charcoal";
        }

        private string _logText;
        public string LogText {
            get { return _logText; }
            set { _logText = value;
                RaisePropertyChanged(nameof(LogText));
            }
        }

        public ICommand CloseCmd => new RelayCommand(closeLogHandler);

        private void closeLogHandler()
        {
            Messenger.Default.Send(new ToggleLogViewMessage(false));
        }
        
    }
}

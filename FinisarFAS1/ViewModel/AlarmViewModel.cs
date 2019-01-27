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
    public class AlarmViewModel : ViewModelBase
    {
        public AlarmViewModel()
        {
            AlarmLogText = "Lorem ipsum dolor amet typewriter deep v plaid lo-fi. Bicycle rights af mlkshk church-key prism PBR&B vinyl. Wolf heirloom four dollar toast, poke ennui brunch ramps mixtape vice humblebrag artisan. Retro ramps snackwave shaman church-key beard vape wayfarers shoreditch. "; 
        }

        private string _alarmLogText;
        public string AlarmLogText {
            get { return _alarmLogText; }
            set { _alarmLogText = value;
                RaisePropertyChanged(nameof(AlarmLogText));
            }
        }

        public ICommand CloseCmd => new RelayCommand(closeAlarmHandler);

        private void closeAlarmHandler()
        {
            Messenger.Default.Send(new ToggleAlarmViewMessage(false));
        }
        
    }
}

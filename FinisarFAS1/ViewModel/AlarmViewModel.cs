using Common;
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
            AlarmLogText = "12/05/2018: 11:38:40 AM: 63-Chamber pressure low\n" +
                "12/05/2018: 11:42:22 AM: 1-Shutter Moving Error\n" +
                "12/05/2018: 11:55:32 AM: 258-Forevacuum Pump Error\n";
            Messenger.Default.Register<EventMessage>(this, EventMessageHandler);
        }

        private void EventMessageHandler(EventMessage msg)
        {
            if (msg.MsgType == "A")
            {
                int len = msg.Message.Length > 80 ? 80 : msg.Message.Length;
                string s = msg.Message.Substring(0, len);
                AlarmLogText += msg.MsgDateTime + "-" + s + "\n";
            }
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

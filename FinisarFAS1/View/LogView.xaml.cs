using Common;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Controls;

namespace FinisarFAS1.View
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : UserControl
    {      
        public ObservableCollection<LogEntry> LogEntries { get; set; }

        public LogView()
        {
            InitializeComponent();          

            DataContext = LogEntries = new ObservableCollection<LogEntry>();          

            Messenger.Default.Register<EventMessage>(this, AddLogEntry);
        }

        private void AddLogEntry(EventMessage msg)
        {
            LogEntry logEntry = new LogEntry() { EventDateTime = msg.MsgDateTime, Message = msg.Message };
            if (msg.MsgType == "A")
                logEntry.Message = "ALARM:" + logEntry.Message;
            Dispatcher.BeginInvoke((Action)(() => LogEntries.Add(logEntry)));
        }

        private void CloseLog_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Messenger.Default.Send(new ToggleLogViewMessage(false));
        }
    }
}

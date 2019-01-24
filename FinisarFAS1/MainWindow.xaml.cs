using Common;
using FinisarFAS1.View;
using FinisarFAS1.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using MESCommunications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinisarFAS1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   

        public MainWindow()
        {
            //this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight * 0.75);
            //this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth * 0.75);

            InitializeComponent();

            Messenger.Default.Register<ShowAlarmWindowMessage>(this, ShowAlarmViewMsg);
            Messenger.Default.Register<ShowLogWindowMessage>(this, ShowLogViewMsg);
        } 
        
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            var uie = e.OriginalSource as UIElement;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                uie.MoveFocus( new TraversalRequest( FocusNavigationDirection.Next));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.BeginStoryboard((Storyboard)this.Resources["collapseEntry"]);
            //Messenger.Default.Send(new ShowSearchWindowMessage(true));
            this.BeginStoryboard((Storyboard)this.Resources["expandAlarm"]);
            this.BeginStoryboard((Storyboard)this.Resources["collapseAlarm"]);

            this.BeginStoryboard((Storyboard)this.Resources["expandLog"]);
            this.BeginStoryboard((Storyboard)this.Resources["collapseLog"]);
            //this.BeginStoryboard((Storyboard)this.Resources["expandWafer"]);

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Messenger.Default.Send(new CancelTransactionMessage());
        }

        private void ShowAlarmViewMsg(ShowAlarmWindowMessage msg)
        {
            if (msg.bVisible)
            {
                this.BeginStoryboard((Storyboard)this.Resources["expandAlarm"]);
            }
            else
            {
                this.BeginStoryboard((Storyboard)this.Resources["collapseAlarm"]);
            }
        }

        private void ShowLogViewMsg(ShowLogWindowMessage msg)
        {
            if (msg.bVisible)
            {
                this.BeginStoryboard((Storyboard)this.Resources["expandLog"]);
            }
            else
            {
                this.BeginStoryboard((Storyboard)this.Resources["collapseLog"]);
            }
        }


    }
}

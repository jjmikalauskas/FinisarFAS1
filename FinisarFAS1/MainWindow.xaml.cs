using FinisarFAS1.View;
using GalaSoft.MvvmLight.Messaging;
using MESCommunications;
using System;
using System.Collections.Generic;
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

            Messenger.Default.Register<ShowEntryWindowMessage>(this, ShowEntryDialogMsg);
            Messenger.Default.Register<GoToMainWindowMessage>(this, ShowMainWindowMsg);

            LoadTable1();
            
        }

        private void LoadTable1()
        {
            // var wafers = MESDAL.GetCurrentWaferSetup(1);
            var wafers = MESDAL.GetCurrentWaferConfigurationSetup(1);
            
            _maindgPort1.ItemsSource = wafers;
            _maindgPort2.ItemsSource = wafers;
        }       

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.BeginStoryboard((Storyboard)this.Resources["collapseEntry"]);
            //Messenger.Default.Send(new ShowSearchWindowMessage(true));
            //this.BeginStoryboard((Storyboard)this.Resources["expandEntry"]);
            //this.BeginStoryboard((Storyboard)this.Resources["expandWafer"]);

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Messenger.Default.Send(new CancelTransactionMessage());
        }

        private void ShowEntryDialogMsg(ShowEntryWindowMessage msg)
        {
            //if (msg.bVisible)
            //{
            //    this.BeginStoryboard((Storyboard)this.Resources["expandEntry"]);
            //}
            //else
            //{
                this.BeginStoryboard((Storyboard)this.Resources["collapseEntry"]);
            //}
        }


        private void ShowMainWindowMsg(GoToMainWindowMessage msg)
        {
            //if (msg.bVisible)
            //{
            //    this.BeginStoryboard((Storyboard)this.Resources["collapseEntry"]);
            //}
            //else
            //{
            //    this.BeginStoryboard((Storyboard)this.Resources["collapseWafer"]);
            //}
        }


    }
}

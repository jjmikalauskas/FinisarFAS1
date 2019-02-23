﻿using Common;
using FinisarFAS1.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace FinisarFAS1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Found this code on internet to remove the Windows title bar close button.
        //Uses System.Windows.Interop and System.Runtime.Interopservices
        //Another call is made in Window_Loaded()
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public MainWindow()
        {
            //this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight * 0.75);
            //this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth * 0.75);

            InitializeComponent();

            Messenger.Default.Register<ToggleAlarmViewMessage>(this, ShowAlarmViewMsg);
            Messenger.Default.Register<ToggleLogViewMessage>(this, ShowLogViewMsg);
            Messenger.Default.Register<SecMsgOperationMessage>(this, UpdateSecMsg);
            //SecMsgOperation = "Testing startup";
        }

        private void UpdateSecMsg(SecMsgOperationMessage msg)
        {
           //SecMsgOperation = msg.newText; 
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            var uie = e.OriginalSource as UIElement;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var box = sender as TextBox;
            if (box != null)
                box.SelectAll();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.BeginStoryboard((Storyboard)this.Resources["collapseEntry"]);
            //Messenger.Default.Send(new ShowSearchWindowMessage(true));
            this.BeginStoryboard((Storyboard)this.Resources["expandAlarm"]);
            this.BeginStoryboard((Storyboard)this.Resources["collapseAlarm"]);

            //this.BeginStoryboard((Storyboard)this.Resources["expandLog"]);
            //this.BeginStoryboard((Storyboard)this.Resources["collapseLog"]);
            //this.BeginStoryboard((Storyboard)this.Resources["expandWafer"]);


            //Found this code on internet to remove the Windows title bar close button.
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Messenger.Default.Send(new CancelTransactionMessage());
        }

        static bool alarmOut = false;
        static bool logOut = false;

        private void ShowAlarmViewMsg(ToggleAlarmViewMessage msg)
        {
            alarmOut = !alarmOut;
            //if (msg.bVisible)
            if (alarmOut)
            {
                if (logPanel.Visibility == Visibility.Visible)
                    ShowLogViewMsg(new ToggleLogViewMessage(false));
                this.BeginStoryboard((Storyboard)this.Resources["expandAlarm"]);
            }
            else
            {
                this.BeginStoryboard((Storyboard)this.Resources["collapseAlarm"]);
            }
            // alarmOut = msg.bVisible;
        }

        private void ShowLogViewMsg(ToggleLogViewMessage msg)
        {
            logOut = !logOut;
            //if (msg.bVisible)
            if (logOut)
            {
                if (alarmOut)
                    ShowAlarmViewMsg(new ToggleAlarmViewMessage(false));
                logPanel.Visibility = Visibility.Visible;
                //this.BeginStoryboard((Storyboard)this.Resources["expandLog"]);
            }
            else
            {
                logPanel.Visibility = Visibility.Collapsed;
                //this.BeginStoryboard((Storyboard)this.Resources["collapseLog"]);
            }
        }

        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            ((MainViewModel)fe.DataContext).closeAlarmCmdHandler();
        }

        private void CircularProgressBar_Loaded(object sender, RoutedEventArgs e)
        {

        }
             

        //public string SecMsgOperation
        //{
        //    get { return (string)GetValue(SecMsgOperationProperty); }
        //    set { SetValue(SecMsgOperationProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for StatusBarText.  
        //// This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty SecMsgOperationProperty =
        //    DependencyProperty.Register("SecMsgOperation", typeof(string), 
        //        typeof(MainWindow), new UIPropertyMetadata(null));

    }
}

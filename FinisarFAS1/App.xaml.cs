using FinisarFAS1.Utility;
using FinisarFAS1.View;
using FinisarFAS1.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinisarFAS1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.KeyUpEvent, new KeyEventHandler(TextBox_HandleEnterKey));
            base.OnStartup(e);

            //IDialogService2 dialogService = new MyDialogService(MainWindow);
            //dialogService.Register<DialogViewModel, DialogWindow>();
            //var vm = new MainViewModel(dialogService);          
            var vm = new MainViewModel();
            var view = new MainWindow { DataContext = vm };
            view.ShowDialog();
        }

        // On KeyDown
        private void TextBox_HandleEnterKey(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var uie = e.OriginalSource as UIElement;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                uie.MoveFocus(
                new TraversalRequest(
                FocusNavigationDirection.Next));
            }
        }

        //public App()
        //{
        //    DisplayMainWindow();
        //}

        //public void DisplayMainWindow()
        //{
        //    var mainVM = new ViewModel.MainViewModel();
        //    string v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        //    var mainWindow = new MainWindow()
        //    {
        //        DataContext = mainVM,
        //        WindowStartupLocation = WindowStartupLocation.CenterScreen,
        //        Title = string.Format("Finisar Factory Automation System - {0}", v)
        //    };
        //    mainWindow.ShowDialog();
        //}
    }
}

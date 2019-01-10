/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:FinisarFAS1"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using FinisarFAS1.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using System;
using System.Threading.Tasks;

namespace FinisarFAS1.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        //public ViewModelLocator()
        //{
        //    ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

        //    ////if (ViewModelBase.IsInDesignModeStatic)
        //    ////{
        //    ////    // Create design time view services and models
        //    ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
        //    ////}
        //    ////else
        //    ////{
        //    ////    // Create run time view services and models
        //    ////    SimpleIoc.Default.Register<IDataService, DataService>();
        //    ////}
        //    SimpleIoc.Default.Register<MyDialogService>(() => { return new MyDialogService(null); });
        //    SimpleIoc.Default.Register<IDialogService2>(() => { return SimpleIoc.Default.GetInstance<MyDialogService>(); });
        //    SimpleIoc.Default.Register<MainViewModel>();
        //    SimpleIoc.Default.Register<WaferConfirmationViewModel>();
        //    SimpleIoc.Default.Register<EntryViewModel>();
        //}

        //public MainViewModel Main {
        //    get {
        //        return ServiceLocator.Current.GetInstance<MainViewModel>();
        //    }
        //}

        //public WaferConfirmationViewModel WaferVM
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<WaferConfirmationViewModel>();
        //    }
        //}
       
        //public EntryViewModel EntryVM
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<EntryViewModel>();
        //    }
        //}

        //public static void Cleanup()
        //{
        //    // TODO Clear the ViewModels
        //}
    }

    //public class DialogService : IDialogService
    //    {
    //        /// <summary>
    //        /// Displays information about an error.
    //        /// </summary>
    //        /// <param name="message">The message to be shown to the user.</param>
    //        /// <param name="title">The title of the dialog box. This may be null.</param>
    //        /// <param name="buttonText">The text shown in the only button
    //        /// in the dialog box. If left null, the text "OK" will be used.</param>
    //        /// <param name="afterHideCallback">A callback that should be executed after
    //        /// the dialog box is closed by the user.</param>
    //        /// <returns>A Task allowing this async method to be awaited.</returns>
    //        public async Task ShowError(string message, string title, string buttonText, Action afterHideCallback)
    //        {
    //            var dialog = CreateDialog(message, title, buttonText, null, afterHideCallback);
    //            await dialog.ShowAsync();
    //        }

    //        /// <summary>
    //        /// Displays information about an error.
    //        /// </summary>
    //        /// <param name="error">The exception of which the message must be shown to the user.</param>
    //        /// <param name="title">The title of the dialog box. This may be null.</param>
    //        /// <param name="buttonText">The text shown in the only button
    //        /// in the dialog box. If left null, the text "OK" will be used.</param>
    //        /// <param name="afterHideCallback">A callback that should be executed after
    //        /// the dialog box is closed by the user.</param>
    //        /// <returns>A Task allowing this async method to be awaited.</returns>
    //        public async Task ShowError(Exception error, string title, string buttonText, Action afterHideCallback)
    //        {
    //            var dialog = CreateDialog(error.Message, title, buttonText, null, afterHideCallback);
    //            await dialog.ShowAsync();
    //        }

    //        /// <summary>
    //        /// Displays information to the user. The dialog box will have only
    //        /// one button with the text "OK".
    //        /// </summary>
    //        /// <param name="message">The message to be shown to the user.</param>
    //        /// <param name="title">The title of the dialog box. This may be null.</param>
    //        /// <returns>A Task allowing this async method to be awaited.</returns>
    //        public async Task ShowMessage(string message, string title)
    //        {
    //            var dialog = CreateDialog(message, title);
    //            await dialog.ShowAsync();
    //        }

    //        /// <summary>
    //        /// Displays information to the user. The dialog box will have only
    //        /// one button.
    //        /// </summary>
    //        /// <param name="message">The message to be shown to the user.</param>
    //        /// <param name="title">The title of the dialog box. This may be null.</param>
    //        /// <param name="buttonText">The text shown in the only button
    //        /// in the dialog box. If left null, the text "OK" will be used.</param>
    //        /// <param name="afterHideCallback">A callback that should be executed after
    //        /// the dialog box is closed by the user.</param>
    //        /// <returns>A Task allowing this async method to be awaited.</returns>
    //        public async Task ShowMessage(string message, string title, string buttonText, Action afterHideCallback)
    //        {
    //            var dialog = CreateDialog(message, title, buttonText, null, afterHideCallback);
    //            await dialog.ShowAsync();
    //        }

    //        /// <summary>
    //        /// Displays information to the user. The dialog box will have only
    //        /// one button.
    //        /// </summary>
    //        /// <param name="message">The message to be shown to the user.</param>
    //        /// <param name="title">The title of the dialog box. This may be null.</param>
    //        /// <param name="buttonConfirmText">The text shown in the "confirm" button
    //        /// in the dialog box. If left null, the text "OK" will be used.</param>
    //        /// <param name="buttonCancelText">The text shown in the "cancel" button
    //        /// in the dialog box. If left null, the text "Cancel" will be used.</param>
    //        /// <param name="afterHideCallback">A callback that should be executed after
    //        /// the dialog box is closed by the user. The callback method will get a boolean
    //        /// parameter indicating if the "confirm" button (true) or the "cancel" button
    //        /// (false) was pressed by the user.</param>
    //        /// <returns>A Task allowing this async method to be awaited.</returns>
    //        public async Task<bool> ShowMessage(
    //            string message,
    //            string title,
    //            string buttonConfirmText,
    //            string buttonCancelText,
    //            Action<bool> afterHideCallback)
    //        {
    //            var result = false;

    //            var dialog = CreateDialog(
    //                message,
    //                title,
    //                buttonConfirmText,
    //                buttonCancelText,
    //                null,
    //                afterHideCallback,
    //                r => result = r);

    //            await dialog.ShowAsync();
    //            return result;
    //        }

    //        /// <summary>
    //        /// Displays information to the user in a simple dialog box. The dialog box will have only
    //        /// one button with the text "OK". This method should be used for debugging purposes.
    //        /// </summary>
    //        /// <param name="message">The message to be shown to the user.</param>
    //        /// <param name="title">The title of the dialog box. This may be null.</param>
    //        /// <returns>A Task allowing this async method to be awaited.</returns>
    //        public async Task ShowMessageBox(string message, string title)
    //        {
    //            var dialog = CreateDialog(message, title);
    //            await dialog.ShowAsync();
    //        }

    //        private MessageDialog CreateDialog(
    //            string message,
    //            string title,
    //            string buttonConfirmText = "OK",
    //            string buttonCancelText = null,
    //            Action afterHideCallback = null,
    //            Action<bool> afterHideCallbackWithResponse = null,
    //            Action<bool> afterHideInternal = null)
    //        {
    //            var dialog = new MessageDialog(message, title);

    //            dialog.Commands.Add(
    //                new UICommand(
    //                    buttonConfirmText,
    //                    o =>
    //                    {
    //                        if (afterHideCallback != null)
    //                        {
    //                            afterHideCallback();
    //                        }

    //                        if (afterHideCallbackWithResponse != null)
    //                        {
    //                            afterHideCallbackWithResponse(true);
    //                        }

    //                        if (afterHideInternal != null)
    //                        {
    //                            afterHideInternal(true);
    //                        }
    //                    }));

    //            dialog.DefaultCommandIndex = 0;

    //            if (!string.IsNullOrEmpty(buttonCancelText))
    //            {
    //                dialog.Commands.Add(
    //                    new UICommand(
    //                        buttonCancelText,
    //                        o =>
    //                        {
    //                            if (afterHideCallback != null)
    //                            {
    //                                afterHideCallback();
    //                            }

    //                            if (afterHideCallbackWithResponse != null)
    //                            {
    //                                afterHideCallbackWithResponse(false);
    //                            }

    //                            if (afterHideInternal != null)
    //                            {
    //                                afterHideInternal(false);
    //                            }
    //                        }));

    //                dialog.CancelCommandIndex = 1;
    //            }

    //            return dialog;
    //        }
    //    }


    
}
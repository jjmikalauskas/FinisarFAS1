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
        //#region DraggedItem

        ///// <summary>
        ///// DraggedItem Dependency Property
        ///// </summary>
        //public static readonly DependencyProperty DraggedItemProperty =
        //    DependencyProperty.Register("DraggedItem", typeof(Wafer), typeof(MainWindow));

        ///// <summary>
        ///// Gets or sets the DraggedItem property.  This dependency property 
        ///// indicates ....
        ///// </summary>
        //public Wafer DraggedItem {
        //    get { return (Wafer)GetValue(DraggedItemProperty); }
        //    set { SetValue(DraggedItemProperty, value); }
        //}

        //#endregion


        public MainWindow()
        {
            //this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight * 0.75);
            //this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth * 0.75);

            InitializeComponent();

            Messenger.Default.Register<ShowAlarmWindowMessage>(this, ShowAlarmDialogMsg);
            Messenger.Default.Register<GoToMainWindowMessage>(this, ShowMainWindowMsg);
          
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
            //this.BeginStoryboard((Storyboard)this.Resources["expandEntry"]);
            //this.BeginStoryboard((Storyboard)this.Resources["expandWafer"]);

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Messenger.Default.Send(new CancelTransactionMessage());
        }

        private void ShowAlarmDialogMsg(ShowAlarmWindowMessage msg)
        {
            //if (msg.bVisible)
            //{
            //    this.BeginStoryboard((Storyboard)this.Resources["expandEntry"]);
            //}
            //else
            //{
            //    this.BeginStoryboard((Storyboard)this.Resources["collapseEntry"]);
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

        //    #region edit mode monitoring

        //    /// <summary>
        //    /// State flag which indicates whether the grid is in edit
        //    /// mode or not.
        //    /// </summary>
        //    public bool IsEditing { get; set; }

        //    private void OnBeginEdit(object sender, DataGridBeginningEditEventArgs e)
        //    {
        //        IsEditing = true;
        //        //in case we are in the middle of a drag/drop operation, cancel it...
        //        if (IsDragging) ResetDragDrop();
        //    }

        //    private void OnEndEdit(object sender, DataGridCellEditEndingEventArgs e)
        //    {
        //        IsEditing = false;
        //    }

        //    #endregion
        //    #region Drag and Drop Rows

        //    /// <summary>
        //    /// Keeps in mind whether
        //    /// </summary>
        //    public bool IsDragging { get; set; }

        //    /// <summary>
        //    /// Initiates a drag action if the grid is not in edit mode.
        //    /// </summary>
        //    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //    {
        //        if (IsEditing) return;

        //        var row = UIHelpers.TryFindFromPoint<DataGridRow>((UIElement)sender, e.GetPosition(_maindgPort1));
        //        if (row == null || row.IsEditing) return;

        //        //set flag that indicates we're capturing mouse movements
        //        IsDragging = true;
        //        DraggedItem = (Wafer)row.Item;
        //    }


        //    /// <summary>
        //    /// Completes a drag/drop operation.
        //    /// </summary>
        //    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //    {
        //        if (!IsDragging || IsEditing)
        //        {
        //            return;
        //        }

        //        var vm = (MainViewModel)DataContext;
        //        //get the target item
        //        Wafer targetItem = (Wafer)_maindgPort1.SelectedItem;

        //        if (targetItem == null || !ReferenceEquals(DraggedItem, targetItem))
        //        {
        //            //remove the source from the list
        //            vm.Port1Wafers.Remove(DraggedItem);

        //            //get target index
        //            var targetIndex = vm.Port1Wafers.IndexOf(targetItem);

        //            //move source at the target's location
        //            vm.Port1Wafers.Insert(targetIndex, DraggedItem);

        //            //select the dropped item
        //            _maindgPort1.SelectedItem = DraggedItem;
        //            //RenumberSlots(vm.Port1Wafers); 
        //            Messenger.Default.Send(new RenumberWafersMessage()); 
        //        }

        //        //reset
        //        ResetDragDrop();
        //    }

        ///// <summary>
        ///// Closes the popup and resets the
        ///// grid to read-enabled mode.
        ///// </summary>
        //private void ResetDragDrop()
        //    {
        //        IsDragging = false;
        //        popup1.IsOpen = false;
        //        _maindgPort1.IsReadOnly = false;
        //    }


        //    /// <summary>
        //    /// Updates the popup's position in case of a drag/drop operation.
        //    /// </summary>
        //    private void OnMouseMove(object sender, MouseEventArgs e)
        //    {
        //        if (!IsDragging || e.LeftButton != MouseButtonState.Pressed) return;

        //        //display the popup if it hasn't been opened yet
        //        if (!popup1.IsOpen)
        //        {
        //            //switch to read-only mode
        //            _maindgPort1.IsReadOnly = true;

        //            //make sure the popup is visible
        //            popup1.IsOpen = true;
        //        }


        //        Size popupSize = new Size(popup1.ActualWidth, popup1.ActualHeight);
        //        popup1.PlacementRectangle = new Rect(e.GetPosition(this), popupSize);

        //        //make sure the row under the grid is being selected
        //        Point position = e.GetPosition(_maindgPort1);
        //        var row = UIHelpers.TryFindFromPoint<DataGridRow>(_maindgPort1, position);
        //        if (row != null) _maindgPort1.SelectedItem = row.Item;
        //    }

        //    #endregion       

    }
}

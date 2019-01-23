using Common;
using FinisarFAS1.Utility;
using FinisarFAS1.ViewModel;
using GalaSoft.MvvmLight.Messaging;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinisarFAS1.View
{
    /// <summary>
    /// Interaction logic for WaferGrid.xaml
    /// </summary>
    public partial class WaferGrid : UserControl
    {
        public WaferGrid()
        {
            InitializeComponent();

            Messenger.Default.Register<WafersConfirmedMessage>(this, WafersConfirmedHandler);
            Messenger.Default.Register<WafersInGridMessage>(this, WafersInGridHandler);
            Messenger.Default.Register<SelectedWafersInGridMessage>(this, SelectedWafersInGridHandler);
        }

        private void SelectedWafersInGridHandler(SelectedWafersInGridMessage msg)
        {
            if (msg.wafers?.Count>0)
               msg.wafers.ForEach(wafer => _maindgPort1.SelectedItems.Add(wafer)); 
        }

        private void WafersConfirmedHandler(WafersConfirmedMessage msg)
        {
            this.IsConfirmed = msg.Confirmed;
        }

        private void WafersInGridHandler(WafersInGridMessage msg)
        {
            this.NumberOfWafers = msg.NumberOfWafers.GetValueOrDefault();
        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var box = sender as TextBox;
            if (box != null)
                box.SelectAll();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {

        }

        #region DraggedItem

        /// <summary>
        /// DraggedItem Dependency Property
        /// </summary>
        public static readonly DependencyProperty DraggedItemProperty =
        DependencyProperty.Register("DraggedItem", typeof(Wafer), typeof(MainWindow));

        /// <summary>
        /// Gets or sets the DraggedItem property.  This dependency property 
        /// indicates ....
        /// </summary>
        public Wafer DraggedItem {
            get {
                var w = (Wafer)GetValue(DraggedItemProperty);
                return (Wafer)GetValue(DraggedItemProperty);
            }
            set { SetValue(DraggedItemProperty, value); }
        }

        #endregion

    public bool IsConfirmed { get; set; }
    public int NumberOfWafers { get; set; }

    #region edit mode monitoring

    /// <summary>
    /// State flag which indicates whether the grid is in edit
    /// mode or not.
    /// </summary>
    public bool IsEditing { get; set; }

    private void OnBeginEdit(object sender, DataGridBeginningEditEventArgs e)
    {
        IsEditing = true;
        //in case we are in the middle of a drag/drop operation, cancel it...
        if (IsDragging) ResetDragDrop();
    }

    private void OnEndEdit(object sender, DataGridCellEditEndingEventArgs e)
    {
        IsEditing = false;
    }

    #endregion

    #region Drag and Drop Rows

    /// <summary>
    /// Keeps in mind whether
    /// </summary>
    public bool IsDragging { get; set; }

    /// <summary>
    /// Initiates a drag action if the grid is not in edit mode.
    /// </summary>
    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (IsEditing || IsConfirmed || NumberOfWafers<=0) return;

        var row = UIHelpers.TryFindFromPoint<DataGridRow>((UIElement)sender, e.GetPosition(_maindgPort1));
        if (row == null || row.IsEditing) return;

        //set flag that indicates we're capturing mouse movements
        IsDragging = true;
        DraggedItem = (Wafer)row.Item;
    }

    /// <summary>
    /// Completes a drag/drop operation.
    /// </summary>
    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!IsDragging || IsEditing)
        {
            return;
        }

        var vm = (MainViewModel)DataContext;
        //get the target item
        Wafer targetItem = (Wafer)_maindgPort1.SelectedItem;

        if (targetItem == null || !ReferenceEquals(DraggedItem, targetItem))
        {
            //remove the source from the list
            vm.Port1Wafers.Remove(DraggedItem);

            //get target index
            var targetIndex = vm.Port1Wafers.IndexOf(targetItem);

            //move source at the target's location
            vm.Port1Wafers.Insert(targetIndex, DraggedItem);

            //select the dropped item
            _maindgPort1.SelectedItem = DraggedItem;
            //RenumberSlots(vm.Port1Wafers); 
            Messenger.Default.Send(new RenumberWafersMessage());
        }

        //reset
        ResetDragDrop();
    }

    /// <summary>
    /// Closes the popup and resets the
    /// grid to read-enabled mode.
    /// </summary>
    private void ResetDragDrop()
    {
        IsDragging = false;
        popup1.IsOpen = false;
        _maindgPort1.IsReadOnly = false;
    }


    /// <summary>
    /// Updates the popup's position in case of a drag/drop operation.
    /// </summary>
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (!IsDragging || e.LeftButton != MouseButtonState.Pressed) return;

        //display the popup if it hasn't been opened yet
        if (!popup1.IsOpen)
        {
            //switch to read-only mode
            _maindgPort1.IsReadOnly = true;

            //make sure the popup is visible
            popup1.IsOpen = true;
        }


        Size popupSize = new Size(popup1.ActualWidth, popup1.ActualHeight);
        popup1.PlacementRectangle = new Rect(e.GetPosition(this), popupSize);

        //make sure the row under the grid is being selected
        Point position = e.GetPosition(_maindgPort1);
        var row = UIHelpers.TryFindFromPoint<DataGridRow>(_maindgPort1, position);
        if (row != null) _maindgPort1.SelectedItem = row.Item;
    }

        #endregion

        #region MENU HANDLERS

        private void Grid_RightClick(object sender, RoutedEventArgs e)
        {
            //Get the clicked MenuItem
            var menuItem = (MenuItem)sender;

            //Get the ContextMenu to which the menuItem belongs
            var contextMenu = (ContextMenu)menuItem.Parent;

            //Find the placementTarget
            var item = (DataGrid)contextMenu.PlacementTarget;

            //Get the underlying item, that you cast to your object that is bound
            //to the DataGrid (and has subject and state as property)
            var toDeleteFromBindedList = (Wafer)item.SelectedCells[0].Item;

            Messenger.Default.Send(new MoveWafersMessage(toDeleteFromBindedList)); 
            //Remove the toDeleteFromBindedList object from your ObservableCollection
            // yourObservableCollection.Remove(toDeleteFromBindedList);
        }


        #endregion

        public MainViewModel MyViewModel {
            get {
                return this.DataContext as MainViewModel;
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get SelectedItems from DataGrid.
            var grid = sender as DataGrid;
            var selected = grid.SelectedItems;

            List<Wafer> selectedObjects = selected.OfType<Wafer>().ToList();

            MyViewModel.SelectedWafers = selectedObjects;
        }
    }
}

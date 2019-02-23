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
using System.Windows.Threading;

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
            // Messenger.Default.Register<RetrievingWafersMessage>(this, RetrievingWafersHandler);
            // overlay.Visibility = Visibility.Collapsed;
        }

        private void RetrievingWafersHandler(RetrievingWafersMessage msg)
        {
            if (msg.bVisible)
                //overlay.Visibility = Visibility.Visible;
            Dispatcher.CurrentDispatcher.Invoke(new Action(() => { overlay.Visibility = Visibility.Visible; }), DispatcherPriority.Normal);
            else
                //overlay.Visibility = Visibility.Collapsed;
            Dispatcher.CurrentDispatcher.Invoke(new Action(() => { overlay.Visibility = Visibility.Collapsed; }), DispatcherPriority.Normal);

        }

        private void SelectedWafersInGridHandler(SelectedWafersInGridMessage msg)
        {
            if (msg.wafers?.Count > 0)
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

        public bool IsConfirmed { get; set; }
        public int NumberOfWafers { get; set; }


        public MainViewModel MyViewModel
        {
            get
            {
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

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            //if (overlay.Visibility == Visibility.Collapsed)
            //    overlay.Visibility = Visibility.Visible;
            //else
            //    overlay.Visibility = Visibility.Collapsed;
        }

    }
}

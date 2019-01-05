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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinisarFAS1.View
{
    /// <summary>
    /// Interaction logic for WaferConfirmationView.xaml
    /// </summary>
    public partial class WaferConfirmationView : UserControl
    {
        public WaferConfirmationView()
        {
            InitializeComponent();

            LoadLeftSideCurrent(); 
            // Messenger.Default.Register<PledgeAvailableIndexMessage>(this, selectedIndexHandler);
        }

        private void LoadLeftSideCurrent()
        {
            var wafers = MESDAL.GetCurrentWaferSetup(1); 

            lbCurrentWafers.ItemsSource = wafers;
        }

        //private void selectedIndexHandler(PledgeAvailableIndexMessage msg)
        //{
        //    lbPledgesAvailable.SelectedIndex = msg.selIndex;
        //}

        //private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        //{
        //    var box = sender as TextBox;
        //    if (box != null)
        //        box.SelectAll();
        //}
    }
}

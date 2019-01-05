using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;

namespace FusionPledgeMaintenance.View
{
    /// <summary>
    /// Interaction logic for PledgesAvailable.xaml
    /// </summary>
    public partial class PledgesAvailable 
    {
        public PledgesAvailable()
        {
            InitializeComponent();
            Messenger.Default.Register<PledgeAvailableIndexMessage>(this, selectedIndexHandler);
        }

        private void selectedIndexHandler(PledgeAvailableIndexMessage msg)
        {
            lbPledgesAvailable.SelectedIndex = msg.selIndex; 
        }

        //private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        //{
        //    var box = sender as TextBox;
        //    if (box != null)
        //        box.SelectAll();
        //}

        //private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        //{

        //}

        //private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        //{
        //    int i = 0; 
        //}

    }
}

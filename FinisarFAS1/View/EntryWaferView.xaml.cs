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
    /// Interaction logic for EntryWaferView.xaml
    /// </summary>
    public partial class EntryWaferView : UserControl
    {
        public EntryWaferView()
        {
            InitializeComponent();
            LoadWafers();
            // Messenger.Default.Register<PledgeAvailableIndexMessage>(this, selectedIndexHandler);
        }

        private void LoadWafers()
        {
            var wafers = MESDAL.GetCurrentWaferSetup(1);

            ActualWaferSlots.ItemsSource = wafers;
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {

        }
    }
}

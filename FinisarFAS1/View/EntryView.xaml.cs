using Common;
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
    /// Interaction logic for EntryView.xaml
    /// </summary>
    public partial class EntryView : UserControl
    {
        public EntryView()
        {
            InitializeComponent();
            Messenger.Default.Register<Messages>(this, entryConfirmationChecks);
        }

        private void entryConfirmationChecks(Messages msg)
        {
            //if (msg.Op==null)
            //{
            //}


        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox box = (TextBox)sender;
                BindingExpression be = box.GetBindingExpression(TextBox.TextProperty);
                be.UpdateSource();
                e.Handled = true;
                ICommand cmd = btnConfirm.Command;
                if (cmd.CanExecute(null))
                    cmd.Execute(null);
            }
        }
    }

    
}

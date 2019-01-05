using FinisarFAS1.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinisarFAS1.ViewModel
{
    public class WaferConfirmationViewModel : ViewModelBase
    {
        public WaferConfirmationViewModel()
        {
            
        }
        

        private string _camstarStatusColor;
        public string CamstarStatusColor
        {
            get { return _camstarStatusColor; }
            set
            {
                _camstarStatusColor = value;
                RaisePropertyChanged("CamstarStatusColor");
            }
        }

        #region COMMANDS AND HANDLERS

        //public ICommand btnClear { get { return new RelayCommand(clearEntryView); } }
        public ICommand SaveWaferConfigurationCmd { get { return new RelayCommand(saveWaferConfig); } }

        #endregion 

        private void saveWaferConfig()
        {
            // Confirm all entries exist in the MES
            //var mes = new MESCommunications.MES();
            //var op = mes.GetOperator(OperatorID);
            //var tool = mes.GetTool(ToolID);
            //var lot = mes.GetLot(LotID);

            Messenger.Default.Send(new ShowWaferWindowMessage(null, null, null, false));
        }
    }
}

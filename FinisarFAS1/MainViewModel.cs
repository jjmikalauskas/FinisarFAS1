using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinisarFAS1
{
    //class MainViewModel : ViewModelBase
    //{
    //    public MainViewModel()
    //    {
    //        CamstarStatusColor = "Pink";
    //    }

    //    #region UI BINDINGS
    //    private string gridData ;
    //    public string GridData {
    //        get { return gridData; }
    //        set { gridData = value;
    //            }
    //    }

    //    private string _camstarStatusColor;
    //    public string CamstarStatusColor {
    //        get { return _camstarStatusColor; }
    //        set { _camstarStatusColor = value;
    //            RaisePropertyChanged("CamstarStatusColor");
    //        }
    //    }

    //    public ICommand btnStart {
    //        get { return new RelayCommand(StartProcess); }
    //    }

    //    private void StartProcess()
    //    {
           
    //        DialogMessage dialogMsg = new DialogMessage(ex.Message, null);
    //        dialogMsg.Icon = System.Windows.MessageBoxImage.Error;
    //        Messenger.Default.Send(dialogMsg);
    //    }       

    //    #endregion
               
    //}

    public class WaferInfo
    {       

        string waferNo; 
        public string WaferNo { get { return this.waferNo; }
            set { this.waferNo = value; }
        }

        string slot; 
        public string Slot {
            get { return this.slot; }
            set { this.slot = value; }
        }

        string scrap;
        public string Scrap {
            get { return this.scrap; }
            set {
                this.scrap = value; }
        }

        string recipe;
        public string Recipe {
            get { return this.recipe; }
            set {
                this.recipe = value; }
        }

        string status;
        public string Status
        {
            get { return this.status; }
            set
            {
                this.status = value;
            }
        }

        public string StatusColor
        {
            get
            {
                return this.status == "Completed" ? "Lime" : (this.status=="In Progress...")? "Yellow" : "Azure";
            }
        }


    }
}

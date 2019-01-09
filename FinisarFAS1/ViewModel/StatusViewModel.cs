﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinisarFAS1.ViewModel
{
    public class StatusVM : ViewModelBase 
    {
        public StatusVM()
        {
            CamstarStatusColor = "Lime";
            EquipmentStatusColor = "Lime";
            ProcessState = "Started"; 
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

        private string _equipmentStatusColor;
        public string EquipmentStatusColor
        {
            get { return _equipmentStatusColor; }
            set { _equipmentStatusColor = value;
                RaisePropertyChanged("EquipmentStatusColor");
            }
        }

        private string _processState;
        public string ProcessState {
            get { return _processState; }
            set {
                _processState = value;
                RaisePropertyChanged("ProcessState");
            }
        }



    }
}

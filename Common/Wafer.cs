using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Wafer 
    {
        string slot;
        public string Slot
        {
            get { return this.slot; }
            set { this.slot = value; }
        }

        string containerName;
        public string ContainerName
        {
            get { return this.containerName; }
            set { this.containerName = value; }
        }

        string waferNo;
        public string WaferNo
        {
            get { return this.waferNo; }
            set { this.waferNo = value; }
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

        string workFlowStepName;
        public string WorkFlowStepName
        {
            get { return this.workFlowStepName; }
            set
            {
                this.workFlowStepName = value;
            }
        }

        string specName;
        public string SpecName
        {
            get { return this.specName; }
            set
            {
                this.specName = value;
            }
        }

        string operation;
        public string Operation
        {
            get { return this.operation; }
            set
            {
                this.operation = value;
            }
        }

        string workCenterName;
        public string WorkCenterName
        {
            get { return this.workCenterName; }
            set
            {
                this.workCenterName = value;
            }
        }
      
        string product;
        public string Product
        {
            get { return this.product; }
            set
            {
                this.product = value;
            }
        }

        string productFamilyName;
        public string ProductFamilyName
        {
            get { return this.productFamilyName; }
            set { this.productFamilyName = value; }
        }

        string processBlock;
        public string ProcessBlock
        {
            get { return this.processBlock; }
            set { this.processBlock = value; }
        }

        string scribeId;
        public string ScribeID
        {
            get { return this.scribeId; }
            set { this.scribeId = value; }
        }       

        string runPkt;
        public string RunPkt
        {
            get { return this.runPkt; }
            set
            {
                this.runPkt = value;
            }
        }

        string recipe;
        public string Recipe
        {
            get { return this.recipe; }
            set
            {
                this.recipe = value;
            }
        }

        string epiVendor;
        public string EpiVendor
        {
            get { return this.epiVendor; }
            set
            {
                this.epiVendor = value;
            }
        }

        string parentContainerQty;
        public string ParentContainerQty
        {
            get { return this.parentContainerQty; }
            set
            {
                this.parentContainerQty = value;
            }
        }

        string childContainerQty;
        public string ChildContainerQty
        {
            get { return this.childContainerQty; }
            set
            {
                this.childContainerQty = value;
            }
        }

        string containerType;
        public string ContainerType
        {
            get { return this.containerType; }
            set
            {
                this.containerType = value;
            }
        }

        string parentContainerName;
        public string ParentContainerName
        {
            get { return this.parentContainerName; }
            set
            {
                this.parentContainerName = value;
            }
        }

        public string StatusColor
        {
            get
            {
                string retColor = "Azure";
                if (this.status == null) return retColor; 
                string currentStatus = this.status.ToUpper();
                if (currentStatus.Contains("COMPLETE"))
                    retColor = "Lime"; 
                else if (currentStatus.Contains("ERROR") || currentStatus.Contains("ABORT"))
                    retColor = "Red";
                else if (currentStatus.Contains("MOVED IN"))
                    retColor = "Yellow";
                else if (currentStatus.Contains("PROCESS"))
                    retColor = "DodgerBlue";
                return retColor;
            }
        }

        public override string ToString()
        {
            return Slot + "=>" + WaferNo + " - " + ScribeID;
        }

    }

    public enum WaferStatus
    {
        MovedIn,         
        MovedOut,
        Hold         
    }
}

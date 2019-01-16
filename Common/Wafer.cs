using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Wafer: Operator
    {
        string waferNo;
        public string WaferNo {
            get { return this.waferNo; }
            set { this.waferNo = value; }
        }

        string waferId;
        public string WaferID {
            get { return this.waferId; }
            set { this.waferId = value; }
        }

        string scribeId;
        public string ScribeID {
            get { return this.scribeId; }
            set { this.scribeId = value; }
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
                this.scrap = value;
            }
        }

        string recipe;
        public string Recipe {
            get { return this.recipe; }
            set {
                this.recipe = value;
            }
        }

        string containerID;
        public string ContainerID {
            get { return this.containerID; }
            set {
                this.containerID = value;
            }
        }

        string operation;
        public string Operation {
            get { return this.operation; }
            set {
                this.operation = value;
            }
        }

        string product;
        public string Product {
            get { return this.product; }
            set {
                this.product = value;
            }
        }

        string status;
        public string Status {
            get { return this.status; }
            set {
                this.status = value;
            }
        }

        public string StatusColor {
            get {
                return this.status == "Completed" ? "Lime" : (this.status == "Moved In") ? "Yellow" : "Azure";
            }
        }

        public override string ToString()
        {
            return WaferID + " - " + ScribeID;
        }

    }


}

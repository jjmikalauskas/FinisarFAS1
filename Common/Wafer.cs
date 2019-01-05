using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Wafer
    {
        string waferNo;
        public string WaferNo {
            get { return this.waferNo; }
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

        string status;
        public string Status {
            get { return this.status; }
            set {
                this.status = value;
            }
        }

        public string StatusColor {
            get {
                return this.status == "Completed" ? "Lime" : (this.status == "In Progress...") ? "Yellow" : "Azure";
            }
        }


    }
}

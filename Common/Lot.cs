using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Lot
    {
        public Lot()
        {
            Id = 0;
            LotName = "Lot Number: " + Id.ToString();
            LotInfo = "This is a lot of wafers to process "; 
        }

        public int Id { get; set; }
        public string LotName { get; set; }
        public string LotInfo { get; set; }
    }
}

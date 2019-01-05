using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Operator
    {
        public Operator()
        {
            Id = 0;
            OperatorName = "Bruce Wayne ";
            OperatorInfo = "Bruce Wayne " + Id.ToString();
        }

        public int Id { get; set; }
        public string OperatorName { get; set; }
        public string OperatorInfo { get; set; }
    }
}

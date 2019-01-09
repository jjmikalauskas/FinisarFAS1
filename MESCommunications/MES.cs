using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESCommunications
{
    public class MES : IMESContract
    {


        public Operator GetOperator(int id, string operatorName)
        {
            return new Operator(); 
        }

        public Tool GetTool(int id, string toolName)
        {
            return new Tool();
        }

        public Lot GetLot(int id, string lotName)
        {
            return new Lot(); 
        }

    }
}

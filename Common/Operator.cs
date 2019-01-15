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
            AuthLevel = AuthorizationLevel.Operator;
            OperatorName = "";
            OperatorInfo = "";
        }

        public int Id { get; set; }
        public AuthorizationLevel AuthLevel { get; set; }
        public string OperatorName { get; set; }
        public string OperatorInfo { get; set; }
    }

    public enum AuthorizationLevel
    {
        Operator, 
        Engineer, 
        Admin
    }

}

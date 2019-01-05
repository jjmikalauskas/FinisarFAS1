using Common; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentCommunications
{
    public class EvaTech : IEquipmentMessages
    {
        // EvaTech spcific variables

        Random r = new Random(); 

        public EvaTech()
        {
        }

        public bool AreYouThere(object equipmentId)
        {
            return r.Next(10) >= 2; 
        }

        public DateTime GetDateTime()
        {
            return DateTime.Now; 
        }

        public string InitiateTrace()
        {
            throw new NotImplementedException();
        }

        public string ReadEquipmentConstants()
        {
            throw new NotImplementedException();
        }

        public string RequestStatus()
        {
            throw new NotImplementedException();
        }

        public void SendOffLine()
        {
            throw new NotImplementedException();
        }

        public void SendOnLine()
        {
            throw new NotImplementedException();
        }

        public void StopTrace()
        {
            throw new NotImplementedException();
        }
    }
}

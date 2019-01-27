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
        public Tool currentTool; 

        Random r = new Random(); 

        public EvaTech()
        {
        }

        public Tool SetupToolEnvironment()
        {
            // Read from Config 
            Tool CurrentTool = new Tool();
            CurrentTool.ToolId = Properties.Settings.Default.ToolID;
            CurrentTool.ToolBrand = Properties.Settings.Default.ToolBrand;
            CurrentTool.NumberOfLoadPorts = Properties.Settings.Default.LoadPorts;
            CurrentTool.LoadLock = Properties.Settings.Default.LoadLock;

            CurrentTool.Ports.LoadPort1Name = Properties.Settings.Default.LoadPort1Name;
            CurrentTool.Ports.LoadPort2Name = Properties.Settings.Default.LoadPort2Name;
         
            return CurrentTool;
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

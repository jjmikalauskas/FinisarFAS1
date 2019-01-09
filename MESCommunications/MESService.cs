using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESCommunications
{
    public class MESService : IMESService
    {
        private IMESService _mesService; 

        public MESService(IMESService iService)
        {
            _mesService = iService; 
        }

        #region PUBLIC METHODS             

        public bool ValidateUserFromCamstar(string userName)
        {            
            return _mesService.ValidateUserFromCamstar(userName);
        }

        public Tool GetTool(string toolName)
        {
            var tool = new Tool();
            tool.Id = GetNextRandom(toolName);
            tool.ToolId = toolName; 
            return tool;
        }

        public Lot GetLot(string lotName)
        {
            var lot = new Lot();
            lot.Id = GetNextRandom(lotName);
            lot.Lot1Name = lotName; 
            //if (lot.Id < 100)
            //    lot = null;
            return lot;
        }       

        public string GetToolStatusFromCamstar(string toolName)
        {
            throw new NotImplementedException();
        }

        public bool GetLotOrWaferInfoFromCamstar(string lotId, int currentCassette)
        {
            throw new NotImplementedException();
        }

        public string LotMoveInCamstar(string lot)
        {
            throw new NotImplementedException();
        }

        #endregion

        private int GetNextRandom(string s)
        {
            int seed = 0;
            foreach (char c in s)
            {
                seed += (int)c;
            }
            Random r = new Random(seed);
            return r.Next(999);
        }
    }

    public class MESDAL
    {
        public static List<Wafer> GetCurrentWaferSetup(int id)
        {
            var wafers = new List<Wafer>();
            for (int i = 20; i <= 25; ++i)
            {
                wafers.Add(new Wafer() { Slot = i.ToString(), WaferNo = id.ToString() + '-' + i.ToString(), Scrap = "No scrap " + i.ToString(), Recipe = "V300" });
            }

            wafers[0].Status = "Completed";
            wafers[1].Status = "In Progress...";

            if (id==1)
            {
                wafers[0].Status = "Completed";
                wafers[1].Status = "Completed";
                wafers[2].Status = "Completed";
                wafers[3].Status = "Completed";
                wafers[4].Status = "Completed";
                wafers[5].Status = "In Progress...";
            }

            return wafers; 
        }

        public static List<Wafer> GetCurrentWaferConfigurationSetup(int id)
        {
            List<Wafer> wafers = new List<Wafer>();
            int waferId; 

            for (int i = 20; i > 0; --i)
            {
                waferId = i * 10; 
                wafers.Add(new Wafer()
                    { Slot = i.ToString(),
                      WaferNo = "61851-003-0" + waferId.ToString(),
                      WaferID = "61851-003-0" + waferId.ToString(),
                      Scrap = "No scrap " + i.ToString(),
                      Recipe = "V300" });
            }

            wafers[0].Status = "Completed";
            wafers[0].ScribeID = "J4989083FFE9";
            wafers[1].Status = "In Progress...";
            wafers[1].ScribeID = "J4989082FFD8";

            if (id == 1)
            {
                wafers[0].Status = "Completed";
                wafers[1].Status = "Completed";
                wafers[2].Status = "Completed";
                wafers[3].Status = "Completed";
                wafers[4].Status = "Completed";
                wafers[5].Status = "In Progress...";

                wafers[2].ScribeID = "J4989081FFC7";
                wafers[3].ScribeID = "J4989080FFB6";
                wafers[4].ScribeID = "J4989079FFA5";
                wafers[5].ScribeID = "J4989078FEE9";
                wafers[6].ScribeID = "J4989077FED8";
                wafers[7].ScribeID = "J4989076FEC7";
                wafers[8].ScribeID = "J4989075FEB6";
                wafers[9].ScribeID = "J4989075FEA5";
                wafers[10].ScribeID = "J4989075FDG9";
                wafers[11].ScribeID = "J4989075FDF8";
                wafers[12].ScribeID = "J4989075FDE7";
                wafers[13].ScribeID = "J4989075FDD6";
                wafers[14].ScribeID = "J4989075FDC5";
                wafers[15].ScribeID = "J4989075FDB4";
                wafers[16].ScribeID = "J4989075FDA3";
                wafers[17].ScribeID = "J4989075FCF5";
                wafers[18].ScribeID = "J4989075FCE4";
                wafers[19].ScribeID = "J4989075FCD3";
            }

            List<Wafer> returnList = wafers;

            return returnList ;
        }
    }


}

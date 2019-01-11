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

        static readonly string lot1 = "61851-001";
        static readonly string lot2 = "61851-002";
        static readonly string lot3 = "61851-003";

        public static async void fakeDelay2(int delayTime)
        {
            int i = 0;
            Task wait = Task.Delay(delayTime);
            await wait;
        }


        public static List<Wafer> GetCurrentWaferConfigurationSetup(string lotId)
        {
            List<Wafer> wafers = new List<Wafer>();
            List<Wafer> returnList; 
            int waferId;

            // var slowTask = Task.Factory.StartNew(() => Globals.fakeDelay(3000));
            // await slowTask;
            // fakeDelay2(3000);

            for (int i = 30; i > 0; --i)
            {
                waferId = i * 10;
                wafers.Add(new Wafer()
                { Slot = i.ToString(),
                    WaferNo = "61851-003-" + waferId.ToString("D3"),
                    WaferID = "61851-003-" + waferId.ToString("D3"),
                    Scrap = "No scrap " + i.ToString(),
                    Product = "1329413",
                    Operation = "ETCH_MEASURE",
                    ContainerID = lotId,
                    Status = "Ready",
                    Recipe = "V300" });
            }

            //wafers[0].Status = "Completed";
            wafers[0].ScribeID = "J4989083FFE9";
            //wafers[1].Status = "In Process...";
            wafers[1].ScribeID = "J4989082FFD8";

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
            wafers[20].ScribeID = "J4989075FCC2";
            wafers[21].ScribeID = "J4989075FCB1";
            wafers[22].ScribeID = "J4989075FCA0";
            wafers[23].ScribeID = "J4989075FBG9";
            wafers[24].ScribeID = "J4989075FBF8";
            wafers[25].ScribeID = "J4989075FBE7";
            wafers[26].ScribeID = "J4989075FBD6";
            wafers[27].ScribeID = "J4989075FBC5";
            wafers[28].ScribeID = "J4989075FBB4";
            wafers[29].ScribeID = "J4989075FBA2";

            if (lotId.Equals(lot1))
            {
                //wafers[0].Status = "Completed";
                //wafers[1].Status = "Completed";
                //wafers[2].Status = "Completed";
                //wafers[3].Status = "Completed";
                //wafers[4].Status = "Completed";
                //wafers[5].Status = "In Process...";
                returnList = wafers.GetRange(0, 6);
            }
            else
            if (lotId.Equals(lot2))
            {
                //wafers[0].Status = "In Process...";
                returnList = wafers.GetRange(10, 8);
            }
            else
            {
                returnList = wafers.GetRange(19, 10); 
            }

            return returnList ;
        }
    }


}

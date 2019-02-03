using Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using MESCommunications.Utility;
using System.Text.RegularExpressions;
using Moq;

namespace MESCommunications
{
    public class MoqMESService : IMESService
    {
        readonly string lot1 = "61851-001";
        readonly string lot2 = "61851-002";
        // readonly string lot3 = "61851-003";

        public bool Initialize(string configFile, string hostName)
        {
            try
            {
                var match = Regex.Match(hostName, @"^\d+\-\d+\-[A-Za-z]+\-\d{2}$");
                if (match.Success)
                    return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public List<Wafer> GetCurrentWaferConfigurationSetup(string lotId)
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
                {
                    Slot = i.ToString(),
                    WaferNo = "61851-003-" + waferId.ToString("D3"),
                    WaferID = "61851-003-" + waferId.ToString("D3"),
                    Scrap = "No scrap " + i.ToString(),
                    Product = "1329413",
                    Operation = "ETCH_MEASURE",
                    ContainerID = lotId,
                    Status = "Ready",
                    Recipe = "V300"
                });
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
                //wafers[4].Status = "Completed";
                //wafers[5].Status = "In Process...";
                returnList = wafers.GetRange(0, 6);
            }
            else if (lotId.Equals(lot2))
            {
                //wafers[0].Status = "In Process...";
                returnList = wafers.GetRange(10, 8);
            }
            else
            {
                returnList = wafers.GetRange(19, 10);
            }

            return returnList;
        }

        private Mock<IMESService> CreateOperatorRepository()
        {
            var repo = new Mock<IMESService>(MockBehavior.Strict);
            repo.Setup(r => r.ValidateEmployee(It.IsAny<string>())).Returns(AuthorizationLevel.InvalidUser);
            repo.Setup(r => r.ValidateEmployee(It.Is<string>(s => s.Contains("ohn")))).Returns(AuthorizationLevel.Engineer);
            repo.Setup(r => r.ValidateEmployee("cindy")).Returns(AuthorizationLevel.Operator);
            repo.Setup(r => r.ValidateEmployee("Mike")).Returns(AuthorizationLevel.Administrator);
            return repo;
        }

        #region PUBLIC METHODS       

        public DataTable GetResourceStatus(string resourceName)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Availability");
            dt.Columns.Add("ResourceName");
            dt.Columns.Add("ResourceStateName");
            dt.Columns.Add("ResourceSubStateName");
            dt.Rows.Add(new object[] { "1", resourceName, "Standby", "Standby" });
            return dt;
        }

        public DataTable GetContainerStatus(string lotId)
        {
            var wafers = GetCurrentWaferConfigurationSetup(lotId);
            var dt = DataHelpers.MakeWaferListIntoDataTable(wafers);
            return dt;
        }       

        #endregion

        public async void fakeDelay2(int delayTime)
        {
            //int i = 0;
            Task wait = Task.Delay(delayTime);
            await wait;

            await Task.Delay(delayTime);
        }
      

        public AuthorizationLevel ValidateEmployee(string strEmployeeName)
        {
            return AuthorizationLevel.Engineer; 
        }

        public bool MoveIn(string container, string errorMsg, bool somebool,
                            string employee, string comment, string resourceName, string factoryName)
        {
            return true; 
        }


        public bool MoveOut(string container, string errorMsg, bool somebool,
                            string employee, string comment)
        {
            return true;
        }

        public bool Hold(string container, string errorMsg,
             string employee, string comment, string resourceName,
             string factory, string holdReason)
        {
            return true; 
        }



    }

}

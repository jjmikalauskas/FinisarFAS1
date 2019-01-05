using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq; 

namespace Tests.MoqTests
{
    public class MoqMESService : IMESService
    {
        private Mock<IOperatorRepository> CreateOperatorRepository()
        {
            var repo = new Mock<IOperatorRepository>(MockBehavior.Strict);
            //repo.Setup(r => r.ValidateUserFromCamstar("John Smith")).Returns(new Operator() { Id = 101, OperatorName = "John Doe", OperatorInfo = "John Doe Info" });
            //repo.Setup(r => r.ValidateUserFromCamstar("cindy")).Returns(new Operator() { Id = 202, OperatorName = "Cindy Doe", OperatorInfo = "Cindy Info" });
            //repo.Setup(r => r.ValidateUserFromCamstar("bobby")).Returns(new Operator() { Id = 303, OperatorName = "Bobby", OperatorInfo = "Bobby Info" });
            repo.Setup(r => r.ValidateUserFromCamstar("John Smith")).Returns(true);
            repo.Setup(r => r.ValidateUserFromCamstar("cindy")).Returns(true);
            repo.Setup(r => r.ValidateUserFromCamstar("bobby")).Returns(false); 
            return repo; 
        }

        #region PUBLIC METHODS

        //public Operator GetOperator(string operatorName)
        //{
        //    var repo = CreateOperatorRepository();

        //    var op = repo.Object.GetOperator(operatorName);
        //    if (op.Id < 100)
        //        op = null;
        //    return op;
        //}

        public Tool GetTool(string toolName)
        {
            var tool = new Tool();
            tool.Id = GetNextRandom(toolName);
            if (tool.Id < 100)
                tool = null;
            return tool;
        }

        public Lot GetLot(string lotName)
        {
            var lot = new Lot();
            lot.Id = GetNextRandom(lotName);
            if (lot.Id < 100)
                lot = null;
            return lot;
        }       

        public bool ValidateUserFromCamstar(string userName)
        {
            var repo = CreateOperatorRepository();
            return repo.Object.ValidateUserFromCamstar(userName);
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
}

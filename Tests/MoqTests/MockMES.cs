using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq; 

namespace Tests.MoqTests
{
    public class MockMESService : IMESService
    {
        private Mock<IOperatorRepository> CreateOperatorRepository()
        {
            var repo = new Mock<IOperatorRepository>(MockBehavior.Strict);
            repo.Setup(r => r.GetOperator("John Smith")).Returns(new Operator() { Id = 101, OperatorName = "John Doe", OperatorInfo = "John Doe Info" });
            repo.Setup(r => r.GetOperator("cindy")).Returns(new Operator() { Id = 202, OperatorName = "Cindy Doe", OperatorInfo = "Cindy Info" });
            repo.Setup(r => r.GetOperator("bobby")).Returns(new Operator() { Id = 303, OperatorName = "Bobby", OperatorInfo = "Bobby Info" });
            return repo; 
        }

        #region PUBLIC METHODS

        public Operator GetOperator(string operatorName, int id)
        {
            var repo = CreateOperatorRepository();

            var op = repo.Object.GetOperator(operatorName);
            if (op.Id < 100)
                op = null;
            return op;
        }

        public Tool GetTool(string toolName, int id)
        {
            var tool = new Tool();
            tool.Id = GetNextRandom(toolName);
            if (tool.Id < 100)
                tool = null;
            return tool;
        }

        public Lot GetLot(string lotName, int id)
        {
            var lot = new Lot();
            lot.Id = GetNextRandom(lotName);
            if (lot.Id < 100)
                lot = null;
            return lot;
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

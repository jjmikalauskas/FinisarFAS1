using Common;
using MESCommunications;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace Camstar.UnitTests
{
    [TestFixture]
    public class MESServiceTests
    {
        private MESService _mesService;
        private readonly string testToolName = "6-6-EVAP-01";
        private readonly string thisHostName = "SHM-L10015894";  // "TEX-L10015200"
        private readonly string strDBServerName = "tex-cs613db-uat.texas.ads.finisar.com";


        [SetUp]
        public void SetUp()
        {
            _mesService = new MESService(new MESDLL());
        }

        [TearDown]
        public void CleanUp()
        {
            _mesService = null;
        }

        [Test]
        public void Initialize_False()
        {
            Assert.That(() => _mesService.Initialize(Globals.MESConfigDir + Globals.MESConfigFile, thisHostName+"X") == false);
        }

        [Test]
        public void Initialize_True()
        {
            var ret = _mesService.Initialize(Globals.MESConfigDir + Globals.MESConfigFile, thisHostName);
            Assert.IsTrue(ret); 
        }

        [Test]
        public void ValidateEmployee_Bad()
        {
            string operatorName = "ZahirHagueXYZ";
            string badResult = "Bad";
            Initialize_True(); 
            var ret = _mesService.ValidateEmployee(operatorName);
            Assert.Equals(ret, badResult);
        }

        [Test]
        public void ValidateEmployee_Success()
        {
            string operatorName = "ZahirHague";
            string goodResult = "Success";
            Initialize_True();
            var ret = _mesService.ValidateEmployee(operatorName);
            Assert.Equals(ret, goodResult);
        }

        [Test]
        public void GetContainerStatus_None()
        {

        }

        [Test]
        public void GetContainerStatus_Ready()
        {

        }

    }
}

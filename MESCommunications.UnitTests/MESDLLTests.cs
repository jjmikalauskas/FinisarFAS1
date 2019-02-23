using Common;
using MESCommunications;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace MESCommunications.UnitTests
{
    [TestFixture]
    public class MESServiceTests
    {
        private MESService _mesService;
        // private readonly string testToolName = "6-6-EVAP-01";
        private readonly string thisHostName = "SHM-L10015894";  // "TEX-L10015200"
        //private readonly string strDBServerName = "tex-cs613db-uat.texas.ads.finisar.com";
        string inifile = @"C:\\FinTest\Config\MESConfig.ini";


        [SetUp]
        public void SetUp()
        {
            _mesService = new MESService(new MESDLL(inifile, thisHostName));
            // _mesService = new MESService(new MoqMESService()); // MESDLL());
        }

        [TearDown]
        public void CleanUp()
        {
            _mesService = null;
        }

        [Test]
        public void Initialize_False()
        {
            Assert.That(() => _mesService.Initialize(inifile, thisHostName) == false);
        }

        [Test]
        public void Initialize_True()
        {
            var ret = _mesService.Initialize(inifile, thisHostName);
            Assert.IsTrue(ret);
        }

        [Test]
        public void ValidateEmployee_ExpectInvalid()
        {
            string operatorName = "ZahirHaqueXYZ";
            AuthorizationLevel expectedAuthLvl = AuthorizationLevel.InvalidUser;
            //Initialize_True();
            var returnAuthLvl = _mesService.ValidateEmployee(operatorName);
            Assert.AreEqual(returnAuthLvl, expectedAuthLvl);
        }

        [Test]
        public void ValidateEmployee_ExpectAdministrator()
        {
            string operatorName = "zahir.haque";
            AuthorizationLevel expectedAuthLvl = AuthorizationLevel.Operator;
            //Initialize_True();
            var returnAuthLvl = _mesService.ValidateEmployee(operatorName);
            Assert.IsTrue(returnAuthLvl == expectedAuthLvl);
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

using Common;
using MESCommunications;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace Camstar.UnitTests
{
    [TestFixture]
    public class MESServiceTests
    {
        private IMESService _mesService;
        private readonly string testToolName = "6-6-EVAP-01";

        [SetUp]
        public void SetUp()
        {
            _mesService = new MESService(new Tests.MoqTests.MoqMESService());
        }

        [TearDown]
        public void CleanUp()
        {
            _mesService = null;
        }

        [Test]
        public void Initialize_False()
        {
            Assert.That(() => _mesService.Initialize(testToolName + "123") == false);
        }

        [Test]
        public void Initialize_True()
        {
            var ret = _mesService.Initialize(testToolName);
            Assert.IsTrue(ret); 
        }


    }
}

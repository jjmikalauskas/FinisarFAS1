using Common;
using NUnit.Framework;

namespace XML.UnitTests
{
    [TestFixture]
    public class UnitTest1
    {
        [SetUp]
        public void SetUp()
        {
            // Set up initializer functions here. This runs prior to every single test 
        }

        [TearDown]
        public void CleanUp()
        {
            // Any cleanup is done here. 
        }

        [Test]
        public void TestXMLReaderToolConfig1()
        {
            var tc = Common.XMLHelper.ReadToolConfigXml("ToolConfigSample.xml");
            Assert.IsTrue(tc.DefineEventsAtAppStart == true);
        }

        [Test]
        public void TestXMLReaderSystemConfig1()
        {

            SystemConfig sc = Common.XMLHelper.ReadSysConfigXml("SystemConfigExample.xml");
            Assert.IsTrue(sc.EmailPort.ToString().Equals("25"));
            Assert.IsFalse(sc.CamstarConfig.DBServerName.Equals("insite"));
        }

        [Test]
        public void TestXMLReaderDialogConfig()
        {

            SystemConfig sc = Common.XMLHelper.ReadSysConfigXml("SystemConfigExample.xml");
            Assert.IsTrue(sc.Dialogs.ShowConfirmationBox);
            Assert.IsFalse(sc.Dialogs.ShowStartMessageBox);
        }
    }
}

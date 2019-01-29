using NUnit.Framework;
using AutoShellMessaging;

namespace ToolService.UnitTests
{
    [TestFixture]
    public class EvatecTests
    {
        //private Evatec _evatec;
        private ISECSHandler<Evatec> _evatec;

        // Values needed to connect to local simulator
        private readonly string _eqSrv = "Evatec01srv";
        private readonly string _eq = "Evatec01";
        private readonly int _timeout = 20;

        private AshlServerLite _myServer;
        private MessagingSettings _messagingSettings;

        [SetUp]
        public void SetUp()
        {
            // Instanciate an Evatec instance
            _evatec = new SECSHandler<Evatec>(new Evatec(_eq));

            // Instanciate an AshlServerLite instance
            _messagingSettings = new MessagingSettings()
            {
                AciConf = "SHM-L10015891:1500", // localhost:port
                CheckDuplicateRegistration = true,
                UseInterfaceSelectionMethod = InterfaceSelectionMethod.DISCOVER,
                Name = "exampleSECS"
            };
            _myServer = AshlServerLite.getInstanceUsingParameters(_messagingSettings);                    
                        
            // In order to Serilog in our tests, a new Serilog logger must be created and used from whithin every unittest method.            
        }

        [TearDown]
        public void Cleanup()
        {
            // Add cleanup code if needed            
        }

        [Test]
        public void Initialize_WhenCalled_DoesNotThrowException()
        {
            Assert.That(() => _evatec.InitializeTool(_eqSrv, _timeout), !Throws.Exception.TypeOf<BoundMessageSendException>());
        }

        [Test]
        [Ignore("Not needed for normal testing")]
        public void Initialize_WhenCalled_ThrowsException()
        {
            Assert.That(() => _evatec.InitializeTool(_eqSrv, _timeout), Throws.Exception.TypeOf<BoundMessageSendException>());
        }

    }
}

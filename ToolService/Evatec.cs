using System;
using System.Collections.Generic;
using AutoShellMessaging;
using SECSInterface;
using Serilog;

namespace ToolService
{
    public class Evatec : Tool
    {
        //public override string Name { get; set; } = "Evatec";

        public Evatec(string name = "Evatec")
        {
            Name = name;            
        }

        public List<SVID> SVIDs { get; set; }
        public List<CEID> CEIDs { get; set; }

        public override void Initialize(string eqSvr, int timeout)
        {            
            // Establish communications
            // Are you there
            // More to come...

            try
            {
                // Uncomment to test thorwing a send exp
                //throw new BoundMessageSendException("Send err!");
                                
                S1F13 s1f13 = new S1F13(Name);
                s1f13.send(eqSvr, timeout);
                Log.Debug("Evatec: S1F13 returned dataSet {@s1f13}", s1f13);
                
                S1F1 s1f1 = new S1F1(Name);
                s1f1.send(eqSvr, timeout);
                Log.Debug("Evatec: S1F1 returned dataSet {@s1f1}", s1f1);

            }
            catch(BoundMessageSendException sendEx)
            {
                throw sendEx;
            }
            catch(Exception e)
            {
                throw e;
            }            
            
        }
    }
}

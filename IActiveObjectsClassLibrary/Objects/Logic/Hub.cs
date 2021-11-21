using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using CommonFunctions;

namespace FerryActiveObjectsClassLibrary
{
    public class Hub : ActiveObject
    {
        //рассылает копию пришедщего сообщения всем подписчикам

        public override string objectType { get { return "Hub"; } }

        public Hub(Scenario _scenario) : base(_scenario)
        {

        }

        public override void processIncomingMessage(IInterObjectMessage msg)
        {

            //просто переслать сообщение
            msg.senderId = guid;
            msg.receiverId = "";
            sendMyMessage(msg);
            
        }

    }
}

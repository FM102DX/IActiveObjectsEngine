using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using CommonFunctions;

namespace FerryActiveObjectsClassLibrary
{
    public class Messenger
    {
        // позволяет объектам обмениваться между собой сообщеняими

        // список нужен для того, чтобы объект мог забрать сообщения не сразу, а когда будет запущен, например, или когда закончит какой-то свой процесс

        public Messenger(Scenario _scenario)
        {
            scenario = _scenario;
        }

        public Dictionary<string, IInterObjectMessage> items = new Dictionary<string, IInterObjectMessage>();

        Scenario scenario;


        public void doMessagesDump()
        {
            Console.ForegroundColor = System.ConsoleColor.Green;
           // Console.WriteLine($"MessagesTotal={items.Count}");
            Console.ForegroundColor = System.ConsoleColor.White;
        }

        public void sendMessage(IInterObjectMessage msg)
        {

            //если сценарий не запущен, то объекты почту не отправляют
            if (scenario.scenarioState != ScenarioStateEnum.Running) return;

            msg.sentDateTime = DateTime.Now;

            IInterObjectMessage msg2;

            //Console.WriteLine($"Object {msg.senderId} is sending message id={msg.guid}, varcontext has {msg.variableContext.outgoingVariableList.Count} vars");

           // msg.variableContext.doVariableContextDump();
            //берем список подписчиков, и каждому шлем это сообщение, используя для этого msg.makeMyReplica();
            //Console.WriteLine($"Getting subscriptions for publisher {msg.senderId} with receiverId={msg.receiverId}");
            var subscriptions = scenario.subscriptionManager.getMySubscriptions(msg.senderId, msg.receiverId);

            //Console.WriteLine($"Object {msg.senderId} has {subscriptions.Count} subscriptions");

            object syncer = new object();

            foreach (var x in subscriptions)
            {
                msg2 = msg.makeMyReplica();
                msg2.sentDateTime = DateTime.Now;
                msg2.receiverId = x.SubscriberGuid;
                bool success = false;
                string s;

                do
                {
                    s = msg.senderId + "_" + fn.generate3block3digitGUID();
                    if (!items.ContainsKey(s))
                    {
                        items.Add(s, msg2);
                        success = true;
                        //TODOтут может зависнуть, но пока так оставляю
                    }
                }
                while (!success);
            }

            doMessagesDump();
        }

        int howMenyLettersForMe(string objectId)
        {
            var messages = items.Where(f => f.Value.receiverId == objectId).ToList();
            if (messages == null) return 0;
            return messages.Count();
        }

        public IInterObjectMessage returnObjectsNextMessage(string activeObjectGuid)
        {
            //читает последнее сообщение с конца, т.е. можно читать по одному
            int x1 = howMenyLettersForMe(activeObjectGuid);
            if (x1 > 0)
            {
                // Console.WriteLine(string.Format("Object '{0}' is checking mail and have {1} messages ", activeObjectGuid, x1));
            }

            if (items.Count == 0) return null;

            //TODO а если тут null, если тут нет сообщений для этого объекта
            var z = items.Where(x => x.Value.receiverId == activeObjectGuid).ToList();

            if (z.Count > 0)
            {
                //есть сообщения
                var rez = items.Where(x => x.Value.receiverId == activeObjectGuid).LastOrDefault();
                IInterObjectMessage msg = rez.Value;
                items.Remove(rez.Key);
                return rez.Value;
            }
            else
            {
                return null;
            }


        }

    }
}

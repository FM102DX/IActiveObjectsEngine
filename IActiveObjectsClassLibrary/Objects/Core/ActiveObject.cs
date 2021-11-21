using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using CommonFunctions;

namespace FerryActiveObjectsClassLibrary
{
    public abstract class ActiveObject : IActiveObject
    {
        public ActiveObject(Scenario _scenario, bool _imReadingMail = true)
        {
            scenario = _scenario;
            variableContext = new ActiveObjectVariableContext(this);
            imReadingMail = _imReadingMail;
        }

        public Queue<IInterObjectMessage> localMessageQueue = new Queue<IInterObjectMessage>();

        public bool imReadingMail { get; set; } //читает ли этот объект вообще входящую почту, просто есть такие, которые ее не читают, напр. таймеры

        public bool imBusy { get; set; } = false;

        Scenario scenario;

        public ActiveObjectVariableContext variableContext { get; set; }

        public string guid { get; set; } = "";

        public void readMyMail()
        {
            if (!imReadingMail) return;
            //if (imBusy) return;

            //даже если занят, сходить за почтой все равно надо

            IInterObjectMessage msg;
            msg = scenario.messenger.returnObjectsNextMessage(guid);
            if (msg == null) return;
            if (imBusy)
            {
                if (msg.isObligatory)
                {
                    //поместить в локальную очередь
                    localMessageQueue.Enqueue(msg);
                }
                else
                {
                    //если оно не обязательное, просто игнорировать
                    msg = null;
                }
            }
            else
            {
                if (localMessageQueue.Count > 0)
                {
                    msg = localMessageQueue.Dequeue();
                    // Console.WriteLine($"Object {guid} is taking message {msg.guid} from local queue");
                }
                else
                {
                    //просто отсавить это msg
                }
            }

            //Итак, вот тут у нас есть msg

            if (msg == null) return;

            //Console.ForegroundColor = System.ConsoleColor.Cyan;
            string s;
            if (msg.variableContext == null)
            {
                s = "null";
            }
            else
            {
                s = msg.variableContext.outgoingVariableList.Count.ToString();
            }
            
            //Console.WriteLine($"Object {guid} got message {msg.guid} with {s} vars");

            //Console.ForegroundColor = System.ConsoleColor.White;

            imBusy = true;
            processIncomingMessage(msg);
            imBusy = false;
        }

        //это должно быть перегружено в каждом активном объекте
        public abstract void processIncomingMessage(IInterObjectMessage msg);

        public abstract string objectType { get; }

        public void sendMyMessage(IInterObjectMessage msg)
        {

            //если сценарий не запущен, ничего не работает
            if (scenario.scenarioState != ScenarioStateEnum.Running) return;

            //пока гуида нет, ничего не работает
            if (guid == "") return;

            //скопировать контекст переменных в сообщение
            msg.variableContext = variableContext.getInterObjectMessageVariableContext(msg);

            //Console.WriteLine($"Creating message variable context {msg.variableContext.outgoingVariableList.Count}");

            scenario.messenger.sendMessage(msg);
        }

    }
}

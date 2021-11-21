using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FerryActiveObjectsClassLibrary
{
    //Интерфейсы

    public interface IActiveObject
    {
        string guid { get; set; }

        void readMyMail();

        string objectType { get;}

        bool imReadingMail { get; }//читает ли этот объект вообще входящую почту, просто есть такие, которые ее не читают, напр. таймеры

        bool imBusy { get; set; }
        ActiveObjectVariableContext variableContext { get; set; }

        void processIncomingMessage(IInterObjectMessage msg);

        void sendMyMessage(IInterObjectMessage msg);
    }

    public interface IInterObjectMessage
    {
        string senderId { get; set; }
        string receiverId { get; set; }
        InterObjectMessageVariableContext variableContext { get; set; }
        DateTime sentDateTime { get; set; }
        string guid { get; set; }
        bool isObligatory { get; set; }

        ObjectMessageTypeEnum msgType { get; }
        string paramString { get; set; }
        string textContent { get; set; }
        string jsonContent { get; set; }
        string xmlContent { get; set; }

        object fileContent { get; set; }

        IInterObjectMessage makeMyReplica();

    }



}

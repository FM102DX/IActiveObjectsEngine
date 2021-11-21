using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using CommonFunctions;

namespace FerryActiveObjectsClassLibrary
{
    public class InterObjectMessage : IInterObjectMessage
    {
        //  это само сообщение
        //  формат сообщения универсален, поскольку для отправки и получения используется одна и та же функция

        public InterObjectMessage(
            ObjectMessageTypeEnum _msgType,
            string _senderId,
            string _textContent,
            bool _isObligatory = true
            )
        {
            msgType = _msgType;
            senderId = _senderId;
            textContent = _textContent;
            guid = "Message_" + fn.generate3block3digitGUID();
            isObligatory = _isObligatory;
            variableContext = new InterObjectMessageVariableContext(this);
        }

        public string guid { get; set; }

        public bool isObligatory { get; set; } // если false, то игнорируется объектом если объект busy. Напр, тики таймера.

        public InterObjectMessageVariableContext variableContext { get; set; }

        public string senderId { get; set; }

        public string receiverId { get; set; } = "";

        public ObjectMessageTypeEnum msgType { get; set; }

        public string paramString { get; set; } = "";
        public string textContent { get; set; } = "";
        public string jsonContent { get; set; } = "";
        public string xmlContent { get; set; } = "";
        public object fileContent { get; set; } = null;

        public DateTime sentDateTime { get; set; }

        public bool deletionMark = false;

        public IInterObjectMessage makeMyReplica()
        {
            InterObjectMessage msg2 = new InterObjectMessage(msgType, senderId, textContent);
            msg2.guid = guid;
            msg2.variableContext = variableContext;
            return msg2;
        }
    }
}

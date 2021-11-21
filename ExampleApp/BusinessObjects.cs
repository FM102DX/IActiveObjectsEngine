using CommonFunctions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using FerryActiveObjectsClassLibrary;
using System.Threading;

namespace Ferry02_Example
{
    //бизнес-объекты

    public class TextWriter : ActiveObject
    {
        //прсото посылает рандомный текст при получении сообщения
        string targetUrl;
        Random random = new Random();
        public override string objectType { get { return "TextWriter"; } }

        public TextWriter(Scenario _scenario) : base(_scenario)
        {
            variableContext.createVariable("a01_int", VariableTypeEnum.Int);
            variableContext.createVariable("b01_string", VariableTypeEnum.String);
        }

        public override void processIncomingMessage(IInterObjectMessage msg)
        {
            string txt="";
            txt = $"Текст1={random.Next(10)}";
            Logger.log("", "Writer writes: "+txt);
            variableContext.setVariableValue("a01_int", random.Next(10));
            variableContext.setVariableValue("b01_string", $"Строка: {random.Next(10)}");

            //Присвоить значение переменной
            sendMyMessage(new InterObjectMessage(ObjectMessageTypeEnum.ActivationMsg, guid, txt));
        }
    }

    public class HttpDataConnector : ActiveObject
    {
        //коннектор
        //ну, коннектор это штуковина, которая ходит не ресурс и делает там запрос
        string targetUrl;

        public override string objectType { get { return "HttpDataConnector"; } }

        public HttpDataConnector(string _objectId, Scenario _scenario, string _targetUrl) : base(_scenario)
        {
            targetUrl = _targetUrl;
        }

        public override void processIncomingMessage(IInterObjectMessage msg)
        {
            //здесь ходим на URL 
            string jsonRezult = "";
            string url = string.Format(@""+ targetUrl);
            try
            {
                //Logger.log("en");
                var request = WebRequest.Create(url);
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    jsonRezult = reader.ReadToEnd();
                }
                
                sendMyMessage( new InterObjectMessage( ObjectMessageTypeEnum.ActivationMsg,guid, jsonRezult) );
            }
            catch
            {

            }
        }


    }

    public class FerryLogger01 : ActiveObject,IActiveObject
    {
        //логгер - штуковина, которая логгирует 

        public FerryLogger01(Scenario _scenario, string _fileName, string _alias, ConsoleColor _color= ConsoleColor.White, DynamicLogger.LogDirectionEnum logDirection= DynamicLogger.LogDirectionEnum.toConsole) : base(_scenario)
        {
            fileName = _fileName;
            dynamicLogger = new DynamicLogger(_fileName, _alias, logDirection);
            color = _color;
        }
        ConsoleColor color;
        public override string objectType { get { return "FerryLogger01"; } }

        DynamicLogger dynamicLogger;

        string fileName;

        public override void processIncomingMessage(IInterObjectMessage msg)
        {
            Console.ForegroundColor = color;
            try
            {
                CommonFunctions.Logger.log($"{dynamicLogger.alias}: ", $"{msg.textContent}");

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ForegroundColor = color;
            }

            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    public class PeriodicTimer : ActiveObject
    {
        public PeriodicTimer(string _objectId, Scenario _scenario, int periodMs) : base(_scenario, false)
        {
            int num = 0;
            tm = new TimerCallback(TimerProcessor);
            timer = new Timer(tm, num, 0, periodMs);
        }
        Timer timer;

        TimerCallback tm;
        public override string objectType { get { return "PeriodicTimer"; } }
        public override void processIncomingMessage(IInterObjectMessage msg)
        {
            //таймер не обрабатывает входящие сообщения
        }

        void TimerProcessor(object obj)
        {
            Console.WriteLine(""); 
            Console.WriteLine("-----------TimerTeak-----------"); 
            InterObjectMessage msg = new InterObjectMessage(ObjectMessageTypeEnum.ActivationMsg,guid, "");
            sendMyMessage(msg);
        }
    }



    public class RndIntGenerator : ActiveObject
    {
        //прсото посылает рандомный текст при получении сообщения
        string targetUrl;
        Random random = new Random();
        public override string objectType { get { return "RndIntGenerator"; } }

        public RndIntGenerator(Scenario _scenario) : base(_scenario)
        {
            variableContext.createVariable("myDigit", VariableTypeEnum.Int);
            variableContext.createVariable("isEven", VariableTypeEnum.Bool);
        }

        public override void processIncomingMessage(IInterObjectMessage msg)
        {
            int myDigit = random.Next(100);
            bool isEven = (myDigit % 2 == 0);
            string evenText = isEven ? "Even" : "Odd";
            string textContent = $"Number is {myDigit} and it's {evenText}";
            Logger.log("", $"RndIntGenerator produced digit={myDigit} and it's {evenText}");
            
            variableContext.setVariableValue("myDigit", myDigit);

            variableContext.setVariableValue("isEven", isEven);

            //Присвоить значение переменной
            sendMyMessage(new InterObjectMessage(ObjectMessageTypeEnum.ActivationMsg, guid, textContent));
        }
    }

}

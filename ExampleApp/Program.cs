using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using CommonFunctions;
using FerryActiveObjectsClassLibrary;

namespace Ferry02_Example
{
    class Program
    {
        static void Main(string[] args)
        {
            App app = new App();
            app.start02();
        }
    }


    public class App
    {

        public void start01()
        {
            Console.WriteLine("Starting app");

            Scenario s = new Scenario();

            s.Stop();

            PeriodicTimer periodicTimer = new PeriodicTimer("", s, 3000);
            s.AddScenarioObject(periodicTimer);

            TextWriter textWriter = new TextWriter(s);
            s.AddScenarioObject(textWriter);

            /*
            HttpDataConnector connector01 = new HttpDataConnector("", s, @"https://develop.ricompany.info/api/aqualionbot");
            s.AddScenarioObject(connector01);

            HttpDataConnector connector02 = new HttpDataConnector("", s, @"https://develop.ricompany.info/api/aqualionbot");
            s.AddScenarioObject(connector02);
            */

            FerryLogger01 logger = new FerryLogger01(s, @"FerryLoggerLogFile.txt", "Logger");
            s.AddScenarioObject(logger);

            s.subscriptionManager.AddSubscription(periodicTimer, textWriter);
            s.subscriptionManager.AddSubscription(textWriter, logger);

            /*
            s.subscriptionManager.AddSubscription(periodicTimer, connector01);
            s.subscriptionManager.AddSubscription(periodicTimer, connector02);
            s.subscriptionManager.AddSubscription(connector01, logger);
            s.subscriptionManager.AddSubscription(connector02, logger);
            */



            s.Run();

            Console.ReadLine();

        }

        public void start02()
        {
            Console.WriteLine("Starting app");

            Scenario s = new Scenario();

            s.Stop();

            PeriodicTimer periodicTimer = new PeriodicTimer("", s, 2500);
            s.AddScenarioObject(periodicTimer);

            RndIntGenerator rndIntGenerator = new RndIntGenerator(s);
            s.AddScenarioObject(rndIntGenerator);

            //логгирует четные
            FerryLogger01 evenLogger = new FerryLogger01(s, @"FerryLoggerLogFile.txt", "EvenLogger", ConsoleColor.Cyan);
            s.AddScenarioObject(evenLogger);

            //логгирует нечетные
            FerryLogger01 oddLogger1 = new FerryLogger01(s, @"FerryLoggerLogFile.txt", "OddLogger01", ConsoleColor.Yellow);
            s.AddScenarioObject(oddLogger1);

            FerryLogger01 oddLogger2 = new FerryLogger01(s, @"FerryLoggerLogFile.txt", "OddLogger02", ConsoleColor.Yellow);
            s.AddScenarioObject(oddLogger2);

            FerryLogger01 oddLogger3 = new FerryLogger01(s, @"FerryLoggerLogFile.txt", "OddLogger03", ConsoleColor.Yellow);
            s.AddScenarioObject(oddLogger3);

            Hub hub = new Hub(s);
            s.AddScenarioObject(hub);

            Router router = new Router(s);
            s.AddScenarioObject(router);
            router.addRoutingRule(1, "isEven", ValueComparisonOperatorEnum.Equals, "true", evenLogger);
            router.addRoutingRule(2, "isEven", ValueComparisonOperatorEnum.Equals, "false", hub);

            s.subscriptionManager.AddSubscription(periodicTimer, rndIntGenerator);
            s.subscriptionManager.AddSubscription(rndIntGenerator, router);
            s.subscriptionManager.AddSubscription(router, evenLogger);
            s.subscriptionManager.AddSubscription(router, hub);
            s.subscriptionManager.AddSubscription(hub, oddLogger1);
            s.subscriptionManager.AddSubscription(hub, oddLogger2);
            s.subscriptionManager.AddSubscription(hub, oddLogger3);

            s.Run();

            Console.ReadLine();

        }
    }

  
    







}

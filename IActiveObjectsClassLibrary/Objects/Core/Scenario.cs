using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using CommonFunctions;

namespace FerryActiveObjectsClassLibrary
{
    public class Scenario
    {
        public Scenario()
        {
            scenarioInternalTimer = new ScenarioInternalTimer(this);
            subscriptionManager = new SubscriptionManager(this);
            messenger = new Messenger(this);

        }

        //сценарий выполнения, состоящий из активных объектов, подписанных друг на друга

        ScenarioStateEnum _scenarioState;

        public ScenarioStateEnum scenarioState
        {
            get { return _scenarioState; }
            set
            {
                _scenarioState = value;

                switch (_scenarioState)
                {
                    case ScenarioStateEnum.Running:
                        //надо запусмтить
                        scenarioInternalTimer.Run();
                        break;

                    case ScenarioStateEnum.Stopped:
                        //надо остановить
                        scenarioInternalTimer.Stop();
                        break;
                }
            }

        }

        ScenarioInternalTimer scenarioInternalTimer;

        public List<IActiveObject> myObjects = new List<IActiveObject>();

        public SubscriptionManager subscriptionManager;

        public Messenger messenger;

        public void AddScenarioObject(IActiveObject targetObject)
        {
            bool _guidOk = false;

            string _guid = "";

            do
            {
                _guid = targetObject.objectType + "_" + fn.generate3block3digitGUID();
                _guidOk = !ScenarioObjectsHaveThisGuid(_guid);
            }
            while (!_guidOk);

            targetObject.guid = _guid;

            myObjects.Add(targetObject);

        }

        public void Save()
        {

        }

        public void Load()
        {

        }

        public void Run()
        {
            scenarioState = ScenarioStateEnum.Running;

        }

        public void Stop()
        {
            scenarioState = ScenarioStateEnum.Stopped;
        }

        public bool ScenarioObjectsHaveThisGuid(string guid)
        {
            int cnt = myObjects.Where(x => x.guid == guid).ToList().Count;
            return cnt > 0;
        }

        public void RemoveScenarioObject(IActiveObject TargetObject)
        {
            myObjects.RemoveAll(x => x.guid == TargetObject.guid);
            subscriptionManager.RemoveObjectFromSubscriptions(TargetObject.guid);
        }

        public IActiveObject getObjectByGuid(string guid)
        {
            var x = myObjects.Where(y => y.guid == guid).ToList();
            if (x.Count == 0)
            {
                return null;
            }
            else
            {
                return x[0];
            }
        }

        public class SubscriptionManager

        {
            Scenario _scenario;

            public SubscriptionManager(Scenario scenario)
            {
                _scenario = scenario;
            }

            //Класс, который управляет подписками

            public List<Subscription> Subscriptions = new List<Subscription>();

            public bool SubscriptionExists(String PublisherGuid, String SubscriberGuid)
            {
                int cnt = Subscriptions.Where(x => x.PublisherGuid == PublisherGuid && x.SubscriberGuid == SubscriberGuid).ToList().Count;
                return cnt > 0;
            }

            public void AddSubscription(IActiveObject publisher, IActiveObject subscriber)
            {
                bool _SubscriptionExists = SubscriptionExists(publisher.guid, subscriber.guid);
                if (!_SubscriptionExists)
                {
                    Subscription subscription = new Subscription(publisher.guid, subscriber.guid);
                    Subscriptions.Add(subscription);
                }
            }

            public void RemoveSubscription(IActiveObject publisher, IActiveObject subscriber)
            {
                Subscriptions.RemoveAll(x => x.PublisherGuid == publisher.guid && x.SubscriberGuid == subscriber.guid);
            }

            public void ClearSubscriptionsOfAnActiveObject(IActiveObject activeObject)
            {
                Subscriptions.RemoveAll(x => x.SubscriberGuid == activeObject.guid);
            }

            public class Subscription
            {
                //Подписка одного IActiveObject на другой
                public String PublisherGuid = "";
                public String SubscriberGuid = "";
                public Subscription(String _PublisherGuid, String _SubscriberGuid)
                {
                    PublisherGuid = _PublisherGuid;
                    SubscriberGuid = _SubscriberGuid;
                }

            }

            public void RemoveObjectFromSubscriptions(String ActiveObjectGuid)
            {
                //отписать всех от этого объекта
                //нужно при удалении объекта
                Subscriptions.RemoveAll(x => x.PublisherGuid == ActiveObjectGuid);
            }

            public List<Subscription> getMySubscriptions(string senderId, string receiverId="")
            {
                if (receiverId=="")
                {
                    return Subscriptions.Where(x => x.PublisherGuid == senderId).ToList();
                }
                else
                {
                    return Subscriptions.Where(x => x.PublisherGuid == senderId && x.SubscriberGuid == receiverId).ToList();
                }
                
            }

        }

        public class ScenarioInternalTimer
        {
            Timer timer;
            TimerCallback tm;
            Scenario scenario;

            //это таймер, который пинает все объекты в сценарии, чтобы они читали сообщения
            public ScenarioInternalTimer(Scenario _scenario)
            {
                scenario = _scenario;
            }

            public void TagMyTargetObjects(object obj)
            {
                
                //если сценарий не запущен, то объекты за почтой не ходят
                if (scenario.scenarioState != ScenarioStateEnum.Running) return; 

                scenario.myObjects.ForEach(x => {
                    x.readMyMail();
                });
            }

            public void Run()
            {
                // устанавливаем метод обратного вызова
                tm = new TimerCallback(TagMyTargetObjects);

                // создаем таймер
                timer = new Timer(tm, null, 0, 500);
            }

            public void Stop()
            {
                timer = null;
            }
        }
    }
}

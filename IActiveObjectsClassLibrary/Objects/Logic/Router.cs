using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using CommonFunctions;

namespace FerryActiveObjectsClassLibrary
{
    public class Router : ActiveObject
    {
        //посылает запрос тому или иному подпсичику в зависимости от значения перменной из контекста

        List<RoutingRule> routingRules = new List<RoutingRule>();

        public override string objectType { get { return "Router"; } }

        public Router(Scenario _scenario) : base(_scenario)
        {

        }

        public override void processIncomingMessage(IInterObjectMessage msg)
        {
            //здесь во входящем сообщении нам прилетает набор переменных
            //надо каждое сообщение прогнать по правилам и выполнить их

            List<RoutingRule> _rrList = routingRules.OrderBy(x => x.order).ToList();

            foreach (RoutingRule y in _rrList)
            {
                //вычислить данное правило для этого сообщения
                //если оно true, то отправить сообщение адресату
               // Console.WriteLine($"Evaluating rule {y.getMyDumpString()}");
                bool z = y.evaluate(msg);
                //Console.WriteLine($"Validation result = {z}");
                if (z)
                {
                    //string textMsg = $"number is {msg.variableContext}";

                    //просто пересылаем сообщение, т.е. senderId берем из msg
                    IInterObjectMessage msg2 = new InterObjectMessage(ObjectMessageTypeEnum.ActivationMsg, guid, "");
                    msg2 = msg.makeMyReplica();

                    //Console.WriteLine($"msg has {msg.variableContext.outgoingVariableList.Count}vars, msg2 has {msg2.variableContext.outgoingVariableList.Count} vars");

                    msg2.senderId = guid;
                    msg2.receiverId = y.routingTargetObject.guid;
                    sendMyMessage(msg2);
                    return;
                }
            }
        }

        public void addRoutingRule(int _order, string _variableName, ValueComparisonOperatorEnum _valueComparisonOperator, object _comparisonValue, IActiveObject _routingTargetObject)
        {
            routingRules.Add(RoutingRule.getMyInstande(this, _order, _variableName, _valueComparisonOperator, _comparisonValue, _routingTargetObject));
        }

        public class RoutingRule
        {
            private RoutingRule(Router _parent)
            {
                parent = _parent;
            }

            Router parent;
            public int order;
            public string variableName;
            public ValueComparisonOperatorEnum valueComparisonOperator;
            public object comparisonValue;
            public IActiveObject routingTargetObject;

            public static RoutingRule getMyInstande(Router _parent, int _order, string _variableName, ValueComparisonOperatorEnum _valueComparisonOperator, object _comparisonValue, IActiveObject _routingTargetObject)
            {

                /*
                bool allowed = VariableContextBase.operatorIsAllowedForVariableType(_variableType, _valueComparisonOperator);
                //todo добавить тут правил проверки
                if (!allowed)
                {
                    //вот тут надо бросать ошибку
                    Console.WriteLine("Error: wrong operator for this typeof variavle");
                    return null;
                }
                */

                RoutingRule rule = new RoutingRule(_parent)
                {
                    order = _order,
                    variableName = _variableName,
                    valueComparisonOperator = _valueComparisonOperator,
                    comparisonValue = _comparisonValue,
                    routingTargetObject = _routingTargetObject
                };

                return rule;

            }

            public bool evaluate(IInterObjectMessage msg)
            {
                //вытащить тип из переменной
                //привести к типу значение из msg и значение 
                //вычисляет данное правило для данного сообщения

                Variable var = msg.variableContext.getVariableByName(variableName);

                string varTxt = var==null ? "Var is Null" : "Var is " + var.getMyDumpString();
//                Console.WriteLine(varTxt);

                if (var == null)
                {
                    Console.WriteLine($"Error: variableName={variableName} not found by Router guid={parent.guid}");
                    return false;
                }
                try
                {
                    switch (var.variableType)
                    {
                        case VariableTypeEnum.Bool:
                            bool v1 = Convert.ToBoolean(var.variableValue);
                            bool v2 = Convert.ToBoolean(comparisonValue);

                            switch (valueComparisonOperator)
                            {
                                case ValueComparisonOperatorEnum.Equals:
                                    return (v1 == v2);
                                case ValueComparisonOperatorEnum.NotEqual:
                                    return (v1 != v2);

                            }
                            break;

                        case VariableTypeEnum.Int:
                            int int1 = Convert.ToInt32(var.variableValue);
                            int int2 = Convert.ToInt32(comparisonValue);

                            switch (valueComparisonOperator)
                            {
                                case ValueComparisonOperatorEnum.Equals:
                                    return (int1 == int2);
                                case ValueComparisonOperatorEnum.NotEqual:
                                    return (int1 != int2);
                                case ValueComparisonOperatorEnum.Greater:
                                    return (int1 > int2);
                                case ValueComparisonOperatorEnum.Lower:
                                    return (int1 < int2);
                                case ValueComparisonOperatorEnum.GreaterOrEquals:
                                    return (int1 >= int2);
                                case ValueComparisonOperatorEnum.LowerOrEquals:
                                    return (int1 <= int2);
                            }
                            break;

                        case VariableTypeEnum.Double:
                            double d1 = Convert.ToDouble(var.variableValue);
                            double d2 = Convert.ToDouble(comparisonValue);
                            switch (valueComparisonOperator)
                            {
                                case ValueComparisonOperatorEnum.Equals:
                                    return (d1 == d2);
                                case ValueComparisonOperatorEnum.NotEqual:
                                    return (d1 != d2);
                                case ValueComparisonOperatorEnum.Greater:
                                    return (d1 > d2);
                                case ValueComparisonOperatorEnum.Lower:
                                    return (d1 < d2);
                                case ValueComparisonOperatorEnum.GreaterOrEquals:
                                    return (d1 >= d2);
                                case ValueComparisonOperatorEnum.LowerOrEquals:
                                    return (d1 <= d2);
                            }
                            break;

                        case VariableTypeEnum.String:
                            string s1 = Convert.ToString(var.variableValue);
                            string s2 = Convert.ToString(comparisonValue);

                            switch (valueComparisonOperator)
                            {
                                case ValueComparisonOperatorEnum.Equals:
                                    return (s1 == s2);
                                case ValueComparisonOperatorEnum.NotEqual:
                                    return (s1 != s2);
                                case ValueComparisonOperatorEnum.Contains:
                                    return (s1.Contains(s2));
                                case ValueComparisonOperatorEnum.NotContains:
                                    return (!s1.Contains(s2));
                            }
                            break;

                        default:
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error converting values: variableName={variableName} valueRR={comparisonValue} valueMSG={var.variableValue} error text={ex.Message}");
                    return false;
                }
                return false;
            }

            public string getMyDumpString()
            {
                return $"RR order={order} variableName={variableName} op={valueComparisonOperator} comparisonValue={comparisonValue} target={routingTargetObject.guid}";
            }
        }



    }
}

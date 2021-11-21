using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonFunctions;

namespace FerryActiveObjectsClassLibrary
{
    public class ActiveObjectVariableContext : VariableContextBase
    {
        //это компонент ActiveObject, который хранит входящие пременные 
        public ActiveObjectVariableContext(IActiveObject _parent)
        {
            parent = _parent;
        }

        public fn.CommonOperationResult setVariableValue(String _variableName, Object _variableValue)
        {
            var x = outgoingVariableList.Where(y => y.variableName == _variableName).ToList();
            if (x.Count == 0)
            {
                var rez = fn.CommonOperationResult.sayFail("No variable with name={_variableName}");
                rez.returningValue = null;
                return rez;
            }
            var z = x[0];
            z.variableValue = _variableValue;
            return fn.CommonOperationResult.sayOk();
        }

        public fn.CommonOperationResult getVariableValue(String _variableName)
        {
            var x = outgoingVariableList.Where(y => y.variableName == _variableName).ToList();
            if (x.Count == 0)
            {
                return fn.CommonOperationResult.sayFail("No variable with name={_variableName}");
            }

            var z = x[0];
            var rez = fn.CommonOperationResult.returnValue(z.variableValue);
            return rez;
        }

        public IActiveObject parent;

        public List<Variable> outgoingVariableList = new List<Variable>();

        public InterObjectMessageVariableContext getInterObjectMessageVariableContext(IInterObjectMessage msg)
        {
            InterObjectMessageVariableContext rez = new InterObjectMessageVariableContext(msg);
            foreach (Variable v in outgoingVariableList)
            {
                rez.createVariable(v.variableName, v.variableType, v.variableValue);
            }

            foreach (Variable v in msg.variableContext.outgoingVariableList)
            {
                if (!rez.doIHaveVariableWithName(v.variableName))
                {
                    rez.createVariable(v.variableName, v.variableType, v.variableValue);
                }
            }

            return rez;
        }
        public List<Variable> incomingVariableList
        {
            get
            {
                //Берем всех издателей, на которые подписан данный ActiveObject, берем их  outgoingVariableList, суммируем 
                return null;
            }
        }

        public void createVariable(String _variableName, VariableTypeEnum _variableType)
        {
            Variable v = new Variable { variableName = _variableName, variableType = _variableType };
            outgoingVariableList.Add(v);
        }

    }

    public class InterObjectMessageVariableContext : VariableContextBase
    {
        public InterObjectMessageVariableContext(IInterObjectMessage _parent)
        {
            parent = _parent;
        }
        IInterObjectMessage parent;
    }

    public abstract class VariableContextBase
    {
        //это компонент ActiveObject, который хранит входящие пременные 
        public VariableContextBase()
        {

        }

        public fn.CommonOperationResult getVariableValue(String _variableName)
        {
            var x = outgoingVariableList.Where(y => y.variableName == _variableName).ToList();
            if (x.Count == 0)
            {
                return fn.CommonOperationResult.sayFail("No variable with name={_variableName}");
            }

            var z = x[0];
            var rez = fn.CommonOperationResult.returnValue(z.variableValue);
            return rez;
        }

        public bool doIHaveVariableWithName(string name)
        {
            var x = outgoingVariableList.Where(y=>y.variableName==name).ToList();
            return x.Count > 0;
        }

        public List<Variable> outgoingVariableList = new List<Variable>();

        public Variable getVariableByName(string name)
        {
            var x = outgoingVariableList.Where(y => y.variableName == name).ToList();
            if (x.Count > 0) return x[0]; else return null;
        }

        public void createVariable(String _variableName, VariableTypeEnum _variableType, object _variableValue = null)
        {
            Variable v = new Variable { variableName = _variableName, variableType = _variableType };
            v.variableValue = _variableValue;
            outgoingVariableList.Add(v);
        }

        public string getVariableContextDumpString()
        {
            return string.Join("\r\n", outgoingVariableList.Select(x=>$"{x.variableName}={x.variableValue} ").ToList());
        }

        public void doVariableContextDump()
        {
            Console.WriteLine(getVariableContextDumpString());
        }

        public static string getValueComparisonOperatorTextRepresentation(ValueComparisonOperatorEnum valueComparisonOperator)
        {
            switch (valueComparisonOperator)
            {
                case ValueComparisonOperatorEnum.Equals:
                    return "=";

                case ValueComparisonOperatorEnum.GreaterOrEquals:
                    return ">=";

                case ValueComparisonOperatorEnum.LowerOrEquals:
                    return "<=";

                case ValueComparisonOperatorEnum.NotEqual:
                    return "!=";

                case ValueComparisonOperatorEnum.Contains:
                    return "Contains";

                default:
                    return "";
            }
        }

        public static List<ValueComparisonOperatorEnum> getAviableOperatorList4ValuType(VariableTypeEnum variableType)
        {
            List<ValueComparisonOperatorEnum> rez = new List<ValueComparisonOperatorEnum>();
            switch (variableType)
            {
                case VariableTypeEnum.Bool:
                    rez.Add(ValueComparisonOperatorEnum.Equals);
                    rez.Add(ValueComparisonOperatorEnum.NotEqual);
                    break;

                case VariableTypeEnum.Int:
                case VariableTypeEnum.Double:
                    rez.Add(ValueComparisonOperatorEnum.Equals);
                    rez.Add(ValueComparisonOperatorEnum.NotEqual);
                    rez.Add(ValueComparisonOperatorEnum.GreaterOrEquals);
                    rez.Add(ValueComparisonOperatorEnum.LowerOrEquals);
                    break;

                case VariableTypeEnum.String:
                    rez.Add(ValueComparisonOperatorEnum.Equals);
                    rez.Add(ValueComparisonOperatorEnum.NotEqual);
                    rez.Add(ValueComparisonOperatorEnum.Contains);
                    rez.Add(ValueComparisonOperatorEnum.NotContains);
                    break;
            }
            return rez;
        }

        public static bool operatorIsAllowedForVariableType(VariableTypeEnum variableType, ValueComparisonOperatorEnum valueComparisonOperator)
        {
            List<ValueComparisonOperatorEnum> rez = getAviableOperatorList4ValuType(variableType);
            return (rez.Contains(valueComparisonOperator));
        }

    }

    public class Variable
    {
        public String variableName;

        public VariableTypeEnum variableType;

        public object variableValue;

        public string getMyDumpString()
        {
            return $"Variable name={variableName} variableType={variableType} variableValue={variableValue}";
        }
    }




}

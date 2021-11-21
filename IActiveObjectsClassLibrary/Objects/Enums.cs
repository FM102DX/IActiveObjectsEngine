using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using CommonFunctions;


namespace FerryActiveObjectsClassLibrary
{

      public enum ObjectMessageTypeEnum
    {
        ActivationMsg=1
    }

    public enum VariableTypeEnum
    {
        String = 1,
        Int = 2,
        Double = 3,
        Bool = 4

    }

    public enum ValueComparisonOperatorEnum
    {
        Equals = 1,
        Greater = 2,
        Lower = 3,
        GreaterOrEquals = 4,
        LowerOrEquals = 5,
        NotEqual = 6,
        Contains = 7,
        NotContains = 8
    }

    public enum ScenarioStateEnum
    {
        Running = 1,
        Stopped = 2,
        Paused = 3 //TODO если будет надо
    }

}

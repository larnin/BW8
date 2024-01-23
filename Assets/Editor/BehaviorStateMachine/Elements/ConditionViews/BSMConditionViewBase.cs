using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BSMConditionViewBase
{
    public abstract VisualElement GetElement();
    public abstract BSMConditionBase GetCondition();

    public static BSMConditionViewBase Create(BSMConditionBase condition)
    {
        if (condition is BSMConditionMultiCondition)
            return new BSMConditionViewMultiCondition(condition as BSMConditionMultiCondition);

        if (condition is BSMConditionAfterTimer)
            return new BSMConditionViewAfterTimer(condition as BSMConditionAfterTimer);

        if (condition is BSMConditionStateEnded)
            return new BSMConditionViewStateEnded(condition as BSMConditionStateEnded);

        Debug.LogError("No condition view for type " + condition.GetType().ToString());
        return null;
    }
}

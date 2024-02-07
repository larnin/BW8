using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BSMConditionViewBase : BSMAttributeHolderView
{
    public BSMConditionViewBase(BSMNode node) : base(node) { }

    public abstract BSMConditionBase GetCondition();

    public static BSMConditionViewBase Create(BSMNode node, BSMConditionBase condition)
    {
        if (condition is BSMConditionMultiCondition)
            return new BSMConditionViewMultiCondition(node, condition as BSMConditionMultiCondition);

        if (condition is BSMConditionAfterTimer)
            return new BSMConditionViewAfterTimer(node, condition as BSMConditionAfterTimer);

        if (condition is BSMConditionStateEnded)
            return new BSMConditionViewStateEnded(node, condition as BSMConditionStateEnded);

        Debug.LogError("No condition view for type " + condition.GetType().ToString());
        return null;
    }
}

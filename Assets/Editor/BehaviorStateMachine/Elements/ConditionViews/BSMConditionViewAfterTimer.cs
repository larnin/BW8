using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class BSMConditionViewAfterTimer : BSMConditionViewBase
{
    BSMConditionAfterTimer m_condition;

    public BSMConditionViewAfterTimer(BSMConditionAfterTimer condition)
    {
        m_condition = condition;
    }

    public override BSMConditionBase GetCondition()
    {
        return m_condition;
    }

    public override VisualElement GetElement()
    {
        return null;
    }
}

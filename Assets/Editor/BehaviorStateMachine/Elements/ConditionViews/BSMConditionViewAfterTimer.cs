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

    public override VisualElement GetElement()
    {
        var prop = new FloatField("Duration");
        prop.value = m_condition.timer;
        prop.RegisterValueChangedCallback(ValueChanged);

        return prop;
    }

    void ValueChanged(ChangeEvent<float> value)
    {
        m_condition.timer = value.newValue;
        if (m_condition.timer < 0)
            m_condition.timer = 0;
    }
}

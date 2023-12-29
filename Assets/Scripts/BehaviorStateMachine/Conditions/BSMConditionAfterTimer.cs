using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

class BSMConditionAfterTimer : BSMConditionBase
{
    [SerializeField] float m_fTimer;
    
    public override VisualElement GetElement()
    {
        var prop = new FloatField("Duration");
        prop.value = m_fTimer;
        prop.RegisterValueChangedCallback(ValueChanged);

        return prop;
    }

    void ValueChanged(ChangeEvent<float> value)
    {
        m_fTimer = value.newValue;
        if (m_fTimer < 0)
            m_fTimer = 0;
    }
}
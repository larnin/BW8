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

    public BSMConditionViewAfterTimer(BSMNode node, BSMConditionAfterTimer condition) : base(node)
    {
        m_condition = condition;
        SetAttributeHolder(m_condition);
    }

    public override BSMConditionBase GetCondition()
    {
        return m_condition;
    }

    protected override VisualElement GetObjectElement()
    {
        return null;
    }
}

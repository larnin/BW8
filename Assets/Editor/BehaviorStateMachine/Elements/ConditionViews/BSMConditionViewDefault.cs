using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

class BSMConditionViewDefault : BSMConditionViewBase
{
    BSMConditionBase m_condition;

    public BSMConditionViewDefault(BSMNode node, BSMConditionBase condition) : base(node)
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

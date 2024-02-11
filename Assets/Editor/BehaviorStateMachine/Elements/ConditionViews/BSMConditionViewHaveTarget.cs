using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class BSMConditionViewHaveTarget : BSMConditionViewBase
{
    BSMConditionHaveTarget m_condition;

    public BSMConditionViewHaveTarget(BSMNode node, BSMConditionHaveTarget condition) : base(node)
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

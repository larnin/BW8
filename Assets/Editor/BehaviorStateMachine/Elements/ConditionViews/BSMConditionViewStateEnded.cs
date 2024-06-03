using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class BSMConditionViewStateEnded : BSMConditionViewBase
{
    BSMConditionStateEnded m_condition;

    public BSMConditionViewStateEnded(BSMNode node, BSMConditionStateEnded condition) : base(node)
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
        return new HelpBox("Transition when the previous state is ended", HelpBoxMessageType.Info);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public  class BSMActionViewDefault : BSMActionViewBase
{
    BSMActionBase m_action;

    public BSMActionViewDefault(BSMNode node, BSMActionBase action) : base(node)
    {
        m_action = action;
        SetAttributeHolder(m_action);
    }

    protected override VisualElement GetObjectElement()
    {
        return null;
    }

    public override BSMActionBase GetAction()
    {
        return m_action;
    }

}

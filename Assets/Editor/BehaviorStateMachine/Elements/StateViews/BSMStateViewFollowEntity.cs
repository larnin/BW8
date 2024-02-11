using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class BSMStateViewFollowEntity : BSMStateViewBase
{
    BSMStateFollowEntity m_state;

    public BSMStateViewFollowEntity(BSMNode node, BSMStateFollowEntity state) : base(node)
    {
        m_state = state;
        SetAttributeHolder(m_state);
    }

    protected override VisualElement GetObjectElement()
    {
        return null;
    }

    public override BSMStateBase GetState()
    {
        return m_state;
    }
}
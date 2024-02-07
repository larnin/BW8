using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class BSMStateViewIdle : BSMStateViewBase
{
    BSMStateIdle m_state;

    public BSMStateViewIdle(BSMNode node, BSMStateIdle state) : base(node)
    {
        m_state = state;
        SetAttributeHolder(m_state);
    }

    protected override VisualElement GetObjectElement()
    {
        VisualElement element = new VisualElement();

        TextField animField = BSMEditorUtility.CreateTextField("Animation", m_state.animation, x=> { OnAnimationChange(x.newValue); });
        element.Add(animField);

        Toggle oriented = BSMEditorUtility.CreateCheckbox("Is Oriented", m_state.isOrientable, x => { OnOrientableChange(x.newValue); });
        element.Add(oriented);

        return element;
    }

    public override BSMStateBase GetState()
    {
        return m_state;
    }

    void OnAnimationChange(string newValue)
    {
        m_state.animation = newValue;
    }

    void OnOrientableChange(bool newValue)
    {
        m_state.isOrientable = newValue;
    }
}

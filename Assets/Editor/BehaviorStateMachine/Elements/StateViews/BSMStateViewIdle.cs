using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class BSMStateViewIdle : BSMStateViewBase
{
    BSMStateIdle m_state;

    public BSMStateViewIdle(BSMStateIdle state)
    {
        m_state = state;
    }

    public override VisualElement GetElement()
    {
        VisualElement element = new VisualElement();

        TextField animField = BSMUtility.CreateTextField("Animation", m_state.animation, OnAnimationChange);
        element.Add(animField);

        Toggle oriented = BSMUtility.CreateCheckbox("Is Oriented", m_state.isOrientable, OnOrientableChange);
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

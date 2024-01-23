using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class BSMConditionViewMultiCondition : BSMConditionViewBase, BSMConditionNodePopupCallback
{
    BSMConditionMultiCondition m_condition; 
    
    VisualElement m_baseContainer;
    VisualElement m_button;

    public BSMConditionViewMultiCondition(BSMConditionMultiCondition condition)
    {
        m_condition = condition;
    }

    public override VisualElement GetElement()
    {
        VisualElement bloc = new VisualElement();

        m_baseContainer = new VisualElement();
        bloc.Add(m_baseContainer);
        UpdateBaseContainer();
        
        bloc.Add(NewConditionElement());

        return bloc;
    }

    VisualElement NewConditionElement()
    {
        m_button = BSMUtility.CreateButton("Add condition", CreatePopup);
        return m_button;
    }

    void UpdateBaseContainer()
    {

    }

    void CreatePopup()
    {
        Vector2 pos = m_button.LocalToWorld(new Vector2(0, 0));
        pos.y -= 100;
        Rect rect = new Rect(pos, new Vector2(200, 100));

        UnityEditor.PopupWindow.Show(rect, new BSMConditionNodePopup(this));
    }

    public void SetCondition(BSMConditionBase condition)
    {
        m_condition.AddCondition(condition);
        UpdateBaseContainer();
    }
}

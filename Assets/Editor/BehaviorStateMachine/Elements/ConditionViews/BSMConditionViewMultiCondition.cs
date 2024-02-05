using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class BSMConditionViewMultiCondition : BSMConditionViewBase, BSMConditionNodePopupCallback, BSMDropdownCallback
{
    BSMConditionMultiCondition m_condition;

    List<BSMConditionViewBase> m_conditionsView = new List<BSMConditionViewBase>();
    
    VisualElement m_baseContainer;
    Button m_addConditionButton;
    Button m_operatorButton;

    public BSMConditionViewMultiCondition(BSMConditionMultiCondition condition)
    {
        m_condition = condition;
    }

    public override VisualElement GetElement()
    {
        VisualElement bloc = new VisualElement();

        bloc.Add(GetOperator());

        m_baseContainer = new VisualElement();
        bloc.Add(m_baseContainer);
        UpdateBaseContainer();
        
        bloc.Add(NewConditionElement());

        return bloc;
    }

    VisualElement GetOperator()
    {
        var header = BSMEditorUtility.CreateHorizontalLayout();
        Label labelName = new Label("Operator");
        header.Add(labelName);

        string op = m_condition.conditionOperator.ToString();
        m_operatorButton = BSMEditorUtility.CreateButton(op, CreateOperatorPopup);
        header.Add(m_operatorButton);

        return header;
    }

    void CreateOperatorPopup()
    {
        Vector2 pos = m_operatorButton.LocalToWorld(new Vector2(0, 0));
        pos.y -= 100;
        Rect rect = new Rect(pos, new Vector2(200, 100));

        var names = Enum.GetNames(typeof(BSMConditionMultiCondition.BSMConditionOperator)).ToList();

        UnityEditor.PopupWindow.Show(rect, new BSMSimpleDropdownPopup(names, this));
    }

    public override BSMConditionBase GetCondition()
    {
        return m_condition;
    }

    VisualElement NewConditionElement()
    {
        m_addConditionButton = BSMEditorUtility.CreateButton("Add condition", CreatePopup);
        return m_addConditionButton;
    }

    void UpdateBaseContainer()
    {
        m_baseContainer.Clear();

        m_conditionsView.Clear();

        for(int i = 0; i < m_condition.GetConditionNb(); i++)
        {
            var condition = m_condition.GetCondition(i);

            m_conditionsView.Add(BSMConditionViewBase.Create(condition));
        }

        foreach(var c in m_conditionsView)
        {
            string conditionName = BSMConditionBase.GetName(c.GetCondition().GetType());

            var header = BSMEditorUtility.CreateHorizontalLayout();
            header.Add(BSMEditorUtility.CreateLabel(conditionName, 4));

            int index = m_baseContainer.childCount;
            var removeButton = BSMEditorUtility.CreateButton("X", () => RemoveCondition(index));
            removeButton.style.maxWidth = 20;
            header.Add(removeButton);

            VisualElement conditionContainer = new VisualElement();
            conditionContainer.Add(header);
            conditionContainer.Add(c.GetElement());

            BSMEditorUtility.SetContainerStyle(conditionContainer, 2, new Color(0.4f, 0.4f, 0.4f), 1, 3, new Color(0.15f, 0.15f, 0.15f));
        }
    }

    void CreatePopup()
    {
        Vector2 pos = m_addConditionButton.LocalToWorld(new Vector2(0, 0));
        pos.y -= 100;
        Rect rect = new Rect(pos, new Vector2(200, 100));

        UnityEditor.PopupWindow.Show(rect, new BSMConditionNodePopup(this));
    }

    public void SetCondition(BSMConditionBase condition)
    {
        m_condition.AddCondition(condition);
        UpdateBaseContainer();
    }

    void RemoveCondition(int index)
    { 
        m_condition.RemoveCondition(index);
        UpdateBaseContainer();
    }

    void BSMDropdownCallback.SendResult(int result)
    {
        var value = (BSMConditionMultiCondition.BSMConditionOperator)result;
        m_condition.conditionOperator = value;
        m_operatorButton.text = value.ToString();
    }
}

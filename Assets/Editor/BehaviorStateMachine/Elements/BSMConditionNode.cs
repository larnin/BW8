﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;

public class BSMConditionNode : BSMNode
{
    BSMConditionBase m_condition = null;

    public override void Draw() 
    {
        base.Draw();

        LocalDraw();
    }

    void LocalDraw()
    {
        extensionContainer.Clear();

        if (m_condition == null)
            DrawSetCondition();
        else DrawCondition();

        RefreshExpandedState();
    }

    void DrawSetCondition()
    {
        Button addChoiceButton = BSMUtility.CreateButton("Set condition", CreatePopup);
        extensionContainer.Add(addChoiceButton);
    }

    void DrawCondition()
    {
        string conditionName = BSMConditionBase.GetName(m_condition.GetType());

        var header = new VisualElement();
        header.AddToClassList("bsm-condition__header");
        Label labelName = new Label(conditionName);
        header.Add(labelName);
        var removeButton = BSMUtility.CreateButton("X", RemoveCondition);
        removeButton.AddToClassList("bsm-node__condition-remove");
        header.Add(removeButton);

        extensionContainer.Add(header);
        var element = m_condition.GetElement();
        if (element != null)
            extensionContainer.Add(element);
    }

    public void SetCondition(BSMConditionBase condition)
    {
        m_condition = condition;
        LocalDraw();
    }

    void CreatePopup()
    {
        var pos = GetPosition();

        UnityEditor.PopupWindow.Show(pos, new BSMConditionNodePopup(this));
    }

    void RemoveCondition()
    {
        SetCondition(null);
    }
}

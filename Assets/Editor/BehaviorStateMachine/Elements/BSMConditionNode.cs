using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

public class BSMConditionNode : BSMNode, BSMConditionNodePopupCallback
{
    BSMConditionBase m_condition = null;
    BSMConditionViewBase m_conditionView = null;

    VisualElement m_button;

    public override void Draw() 
    {
        base.Draw();

        LocalDraw();
    }

    void LocalDraw()
    {
        extensionContainer.Clear();

        m_button = null;

        if (m_condition == null || m_conditionView == null)
            DrawSetCondition();
        else DrawCondition();

        RefreshExpandedState();
    }

    void DrawSetCondition()
    {
        m_button = BSMUtility.CreateButton("Set condition", CreatePopup);
        extensionContainer.Add(m_button);
    }

    void DrawCondition()
    {
        string conditionName = BSMConditionBase.GetName(m_condition.GetType());

        var header = new VisualElement();
        header.style.flexDirection = FlexDirection.Row;
        header.style.justifyContent = Justify.SpaceBetween;
        Label labelName = new Label(conditionName);
        labelName.style.paddingBottom = 4;
        labelName.style.paddingLeft = 4;
        labelName.style.paddingRight = 4;
        labelName.style.paddingTop = 4;
        header.Add(labelName);
        var removeButton = BSMUtility.CreateButton("X", RemoveCondition);
        removeButton.style.maxWidth = 20;
        header.Add(removeButton);

        extensionContainer.Add(header);
        var element = m_conditionView.GetElement();
        if (element != null)
            extensionContainer.Add(element);
    }

    public void SetCondition(BSMConditionBase condition)
    {
        m_condition = condition;
        if (m_condition == null)
            m_conditionView = null;
        else m_conditionView = BSMConditionViewBase.Create(m_condition);
        LocalDraw();
    }

    void CreatePopup()
    {
        if (m_button == null)
            return;

        var pos = m_button.LocalToWorld(new Vector2(0, 0));
        pos.y -= 100;
        var rect = new Rect(pos, new Vector2(200, 100));

        UnityEditor.PopupWindow.Show(rect, new BSMConditionNodePopup(this));
    }

    void RemoveCondition()
    {
        SetCondition(null);
    }
}

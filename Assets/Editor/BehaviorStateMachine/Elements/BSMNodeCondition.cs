using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

public class BSMNodeCondition : BSMNode, BSMConditionNodePopupCallback
{
    BSMConditionBase m_condition = null;
    BSMConditionViewBase m_conditionView = null;

    VisualElement m_button;

    public override void Draw() 
    {
        base.Draw();

        /* INPUT CONTAINER */

        Port inputPort = this.CreatePort("In", Orientation.Horizontal, Direction.Input, Port.Capacity.Single);

        inputContainer.Add(inputPort);

        /* OUTPUT CONTAINER */

        Port outputPort = this.CreatePort("Out", Orientation.Horizontal, Direction.Output, Port.Capacity.Single);

        outputContainer.Add(outputPort);

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
        m_button = BSMEditorUtility.CreateButton("Set condition", CreatePopup);
        extensionContainer.Add(m_button);
    }

    void DrawCondition()
    {
        string conditionName = BSMConditionBase.GetName(m_condition.GetType());

        var header = BSMEditorUtility.CreateHorizontalLayout();
        header.Add(BSMEditorUtility.CreateLabel(conditionName, 4));
        var removeButton = BSMEditorUtility.CreateButton("X", RemoveCondition);
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

    public BSMConditionBase GetCondition()
    {
        return m_condition;
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

    public override void UpdateStyle(bool error)
    {
        base.UpdateStyle(error);

        if(error)
        {
            mainContainer.style.backgroundColor = errorBackgroundColor;
            mainContainer.style.borderBottomColor = errorBorderColor;
            mainContainer.style.borderLeftColor = errorBorderColor;
            mainContainer.style.borderRightColor = errorBorderColor;
            mainContainer.style.borderTopColor = errorBorderColor;
        }
        else
        {
            Color backgroundColor = new Color(0.15f, 0.15f, 0.1f);
            Color borderColor = new Color(0.5f, 0.5f, 0.3f);

            mainContainer.style.backgroundColor = backgroundColor;
            mainContainer.style.borderBottomColor = borderColor;
            mainContainer.style.borderLeftColor = borderColor;
            mainContainer.style.borderRightColor = borderColor;
            mainContainer.style.borderTopColor = borderColor;
        }

        float borderWidth = 2;
        float borderRadius = 20;

        mainContainer.style.borderBottomWidth = borderWidth;
        mainContainer.style.borderLeftWidth = borderWidth;
        mainContainer.style.borderRightWidth = borderWidth;
        mainContainer.style.borderTopWidth = borderWidth;

        mainContainer.style.borderBottomLeftRadius = borderRadius;
        mainContainer.style.borderBottomRightRadius = borderRadius;
        mainContainer.style.borderTopLeftRadius = borderRadius;
        mainContainer.style.borderTopRightRadius = borderRadius;
    }
}

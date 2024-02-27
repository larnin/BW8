using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BSMNodeState : BSMNode, BSMStateNodePopupCallback, BSMAddActionPopupCallback
{
    BSMStateBase m_state = null;
    BSMStateViewBase m_stateView = null;

    VisualElement m_button;
    VisualElement m_actionButton;

    public override void Draw()
    {
        base.Draw();

        /* INPUT CONTAINER */

        Port inputPort = this.CreatePort("In", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
        inputContainer.Add(inputPort);

        /* OUTPUT CONTAINER */

        Port outputPort = this.CreatePort("Out", Orientation.Horizontal, Direction.Output, Port.Capacity.Multi);
        outputContainer.Add(outputPort);

        LocalDraw();
    }

    void LocalDraw()
    {
        extensionContainer.Clear();

        m_button = null;
        m_actionButton = null;

        if (m_state == null || m_stateView == null)
            DrawSetState();
        else DrawState();

        RefreshExpandedState();
    }

    void DrawSetState()
    {
        m_button = BSMEditorUtility.CreateButton("Set state", CreatePopup);
        extensionContainer.Add(m_button);
    }

    void DrawState()
    {
        string stateName = BSMStateBase.GetName(m_state.GetType());

        var header = BSMEditorUtility.CreateHorizontalLayout();
        var label = BSMEditorUtility.CreateLabel(stateName, 4);
        label.style.flexGrow = 2;
        header.Add(label);
        var removeButton = BSMEditorUtility.CreateButton("X", RemoveState);
        removeButton.style.maxWidth = 20;
        header.Add(removeButton);
        m_actionButton = BSMEditorUtility.CreateButton(">", DisplayActionPopup);
        m_actionButton.style.maxWidth = 20;
        header.Add(m_actionButton);

        extensionContainer.Add(header);
        var element = m_stateView.GetElement();
        if (element != null)
            extensionContainer.Add(element);
    }

    public void SetState(BSMStateBase state)
    {
        m_state = state;
        if (m_state == null)
            m_stateView = null;
        else m_stateView = BSMStateViewBase.Create(this, m_state);
        LocalDraw();
    }

    public BSMStateBase GetState()
    {
        return m_state;
    }

    void CreatePopup()
    {
        if (m_button == null)
            return;

        var pos = m_button.LocalToWorld(new Vector2(0, 0));
        pos.y -= 100;
        var rect = new Rect(pos, new Vector2(200, 100));

        UnityEditor.PopupWindow.Show(rect, new BSMStateNodePopup(this));
    }

    void RemoveState()
    {
        SetState(null);
    }

    public override void UpdateStyle(bool error)
    {
        base.UpdateStyle(error);

        if (error)
        {
            mainContainer.style.backgroundColor = errorBackgroundColor;
            mainContainer.style.borderBottomColor = errorBorderColor;
            mainContainer.style.borderLeftColor = errorBorderColor;
            mainContainer.style.borderRightColor = errorBorderColor;
            mainContainer.style.borderTopColor = errorBorderColor;
        }
        else
        {
            Color backgroundColor = new Color(0.1f, 0.1f, 0.15f);
            Color borderColor = new Color(0.3f, 0.3f, 0.5f);

            mainContainer.style.backgroundColor = backgroundColor;
            mainContainer.style.borderBottomColor = borderColor;
            mainContainer.style.borderLeftColor = borderColor;
            mainContainer.style.borderRightColor = borderColor;
            mainContainer.style.borderTopColor = borderColor;
        }

        float borderWidth = 1;
        float borderRadius = 2;

        mainContainer.style.borderBottomWidth = borderWidth;
        mainContainer.style.borderLeftWidth = borderWidth;
        mainContainer.style.borderRightWidth = borderWidth;
        mainContainer.style.borderTopWidth = borderWidth;

        mainContainer.style.borderBottomLeftRadius = borderRadius;
        mainContainer.style.borderBottomRightRadius = borderRadius;
        mainContainer.style.borderTopLeftRadius = borderRadius;
        mainContainer.style.borderTopRightRadius = borderRadius;
    }

    void DisplayActionPopup()
    {
        if (m_actionButton == null)
            return;

        var pos = m_actionButton.LocalToWorld(new Vector2(0, 0));
        pos.y -= 100;
        var rect = new Rect(pos, new Vector2(200, 100));

        var actions = m_state.GetActions();
        if (actions == null || actions.Count == 0)
            return;
        List<string> names = new List<string>();
        foreach (var a in actions)
            names.Add(a.name);

        UnityEditor.PopupWindow.Show(rect, new BSMAddActionPopup(names, this));
    }

    public void AddAction(string name, BSMActionBase action)
    {
        m_state.AddAction(name, action);
        LocalDraw();
    }
}

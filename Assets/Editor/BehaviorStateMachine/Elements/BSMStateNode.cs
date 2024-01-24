using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BSMStateNode : BSMNode, BSMStateNodePopupCallback
{
    BSMStateBase m_state = null;
    BSMStateViewBase m_stateView = null;

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

        if (m_state == null || m_stateView == null)
            DrawSetState();
        else DrawState();

        RefreshExpandedState();
    }

    void DrawSetState()
    {
        m_button = BSMUtility.CreateButton("Set state", CreatePopup);
        extensionContainer.Add(m_button);
    }

    void DrawState()
    {
        string stateName = BSMStateBase.GetName(m_state.GetType());

        var header = new VisualElement();
        header.style.flexDirection = FlexDirection.Row;
        header.style.justifyContent = Justify.SpaceBetween;
        Label labelName = new Label(stateName);
        labelName.style.paddingBottom = 4;
        labelName.style.paddingLeft = 4;
        labelName.style.paddingRight = 4;
        labelName.style.paddingTop = 4;
        header.Add(labelName);
        var removeButton = BSMUtility.CreateButton("X", RemoveState);
        removeButton.style.maxWidth = 20;
        header.Add(removeButton);

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
        else m_stateView = BSMStateViewBase.Create(m_state);
        LocalDraw();
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
}

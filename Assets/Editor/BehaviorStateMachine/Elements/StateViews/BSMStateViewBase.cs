using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BSMStateViewBase : BSMAttributeHolderView
{
    VisualElement m_actionsContainer;
    List<BSMActionViewBase> m_actionsView = new List<BSMActionViewBase>();

    public BSMStateViewBase(BSMNode node) : base(node) { }

    public abstract BSMStateBase GetState();

    public static BSMStateViewBase Create(BSMNode node, BSMStateBase state)
    {
        //if (state is BSMStateIdle)
        //    return new BSMStateViewIdle(node, state as BSMStateIdle);

        return new BSMStateViewDefault(node, state);
    }

    protected override VisualElement GetActionsElement()
    {
        if (GetState() == null)
            return null;

        m_actionsContainer = new VisualElement();

        UpdateActionsContainer();

        return m_actionsContainer;
    }

    void UpdateActionsContainer()
    {
        m_actionsContainer.Clear();
        m_actionsView.Clear();

        m_actionsContainer.Add(BSMEditorUtility.CreateLabel("Actions"));

        bool allEmpty = true;
        var actions = GetState().GetActions();
        foreach (var a in actions)
        {
            if (a.actions == null || a.actions.Count == 0)
                continue;

            allEmpty = false;

            VisualElement actionsContainer = new VisualElement();
            BSMEditorUtility.SetContainerStyle(actionsContainer, 2, new Color(0.4f, 0.4f, 0.4f), 1, 3, new Color(0.15f, 0.15f, 0.15f));

            actionsContainer.Add(BSMEditorUtility.CreateLabel("Group: " + a.name));

            for (int i = 0; i < a.actions.Count; i++)
            {
                var action = a.actions[i];

                VisualElement actionContainer = new VisualElement();
                BSMEditorUtility.SetContainerStyle(actionContainer, 2, new Color(0.4f, 0.4f, 0.4f), 1, 3, new Color(0.15f, 0.15f, 0.15f));

                VisualElement layout = BSMEditorUtility.CreateHorizontalLayout();
                actionContainer.Add(layout);
                layout.Add(BSMEditorUtility.CreateLabel(BSMActionBase.GetName(action.GetType())));

                int index = i;
                var removeButton = BSMEditorUtility.CreateButton("X", ()=> { RemoveAction(a.name, index); });
                removeButton.style.maxWidth = 20;
                layout.Add(removeButton);

                var actionView = BSMActionViewBase.Create(m_node, action);
                m_actionsView.Add(actionView);
                actionContainer.Add(actionView.GetElement());

                actionsContainer.Add(actionContainer);
            }
            m_actionsContainer.Add(actionsContainer);
        }

        if (allEmpty)
            m_actionsContainer.Clear();
    }

    void RemoveAction(string name, int index)
    {
        GetState().RemoveAction(name, index);
        UpdateActionsContainer();
    }
}

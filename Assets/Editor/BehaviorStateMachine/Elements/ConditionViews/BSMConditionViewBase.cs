using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BSMConditionViewBase : BSMAttributeHolderView
{
    VisualElement m_actionsContainer;
    List<BSMActionViewBase> m_actionsView = new List<BSMActionViewBase>();

    public BSMConditionViewBase(BSMNode node) : base(node) { }

    public abstract BSMConditionBase GetCondition();

    public static BSMConditionViewBase Create(BSMNode node, BSMConditionBase condition)
    {
        if (condition is BSMConditionMultiCondition)
            return new BSMConditionViewMultiCondition(node, condition as BSMConditionMultiCondition);

        if (condition is BSMConditionStateEnded)
            return new BSMConditionViewStateEnded(node, condition as BSMConditionStateEnded);

        if (condition is BSMConditionInverseCondition)
            return new BSMConditionViewInverseCondition(node, condition as BSMConditionInverseCondition);

        return new BSMConditionViewDefault(node, condition);
    }
    protected override VisualElement GetActionsElement()
    {
        if (GetCondition() == null)
            return null;

        m_actionsContainer = new VisualElement();

        UpdateActionsContainer();

        return m_actionsContainer;
    }

    void UpdateActionsContainer()
    {
        m_actionsContainer.Clear();
        m_actionsView.Clear();

        var actions = GetCondition().GetActions();
        if (actions.Count == 0)
            return;

        m_actionsContainer.Add(BSMEditorUtility.CreateLabel("Actions"));

        VisualElement actionsContainer = new VisualElement();
        if(actions.Count > 1)
            BSMEditorUtility.SetContainerStyle(actionsContainer, 2, new Color(0.4f, 0.4f, 0.4f), 1, 3, new Color(0.15f, 0.15f, 0.15f));

        for (int i = 0; i < actions.Count; i++)
        {
            var action = actions[i];

            VisualElement actionContainer = new VisualElement();
            BSMEditorUtility.SetContainerStyle(actionContainer, 2, new Color(0.4f, 0.4f, 0.4f), 1, 3, new Color(0.15f, 0.15f, 0.15f));

            VisualElement layout = BSMEditorUtility.CreateHorizontalLayout();
            actionContainer.Add(layout);
            layout.Add(BSMEditorUtility.CreateLabel(BSMActionBase.GetName(action.GetType())));

            int index = i;
            var removeButton = BSMEditorUtility.CreateButton("X", () => { RemoveAction(index); });
            removeButton.style.maxWidth = 20;
            layout.Add(removeButton);

            var actionView = BSMActionViewBase.Create(m_node, action);
            m_actionsView.Add(actionView);
            actionContainer.Add(actionView.GetElement());

            actionsContainer.Add(actionContainer);
        }
        m_actionsContainer.Add(actionsContainer);
    }

    void RemoveAction(int index)
    {
        GetCondition().RemoveAction(index);
        UpdateActionsContainer();
    }
}

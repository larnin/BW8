using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class BSMConditionViewInverseCondition : BSMConditionViewBase, BSMConditionNodePopupCallback
{
    BSMConditionInverseCondition m_condition;
    BSMConditionViewBase m_conditionView;

    VisualElement m_container;
    Button m_setConditionButton;

    public BSMConditionViewInverseCondition(BSMNode node, BSMConditionInverseCondition condition) : base(node)
    {
        m_condition = condition;
        SetAttributeHolder(m_condition);
    }

    public override BSMConditionBase GetCondition()
    {
        return m_condition;
    }

    public void SetCondition(BSMConditionBase condition)
    {
        m_condition.condition = condition;
        UpdateContainer();
    }

    void CreatePopup()
    {
        Vector2 pos = m_setConditionButton.LocalToWorld(new Vector2(0, 0));
        pos.y -= 100;
        Rect rect = new Rect(pos, new Vector2(200, 100));

        UnityEditor.PopupWindow.Show(rect, new BSMConditionNodePopup(this));
    }

    protected override VisualElement GetObjectElement()
    {
        m_container = new VisualElement();

        UpdateContainer();

        return m_container;
    }

    void UpdateContainer()
    {
        m_container.Clear();

        if (m_condition.condition == null)
        {
            m_conditionView = null;
            m_setConditionButton = BSMEditorUtility.CreateButton("Set condition", CreatePopup);
            m_container.Add(m_setConditionButton);
        }
        else
        {
            m_conditionView = BSMConditionViewBase.Create(m_node, m_condition.condition);

            string conditionName = BSMConditionBase.GetName(m_condition.condition.GetType());

            var header = BSMEditorUtility.CreateHorizontalLayout();
            header.Add(BSMEditorUtility.CreateLabel(conditionName, 4));

            var removeButton = BSMEditorUtility.CreateButton("X", RemoveCondition);
            removeButton.style.maxWidth = 20;
            header.Add(removeButton);

            VisualElement conditionContainer = new VisualElement();
            conditionContainer.Add(header);
            conditionContainer.Add(m_conditionView.GetElement());

            BSMEditorUtility.SetContainerStyle(conditionContainer, 2, new Color(0.4f, 0.4f, 0.4f), 1, 3, new Color(0.15f, 0.15f, 0.15f));

            m_container.Add(conditionContainer);
        }
    }

    void RemoveCondition()
    {
        m_condition.condition = null;
        UpdateContainer();
    }
}

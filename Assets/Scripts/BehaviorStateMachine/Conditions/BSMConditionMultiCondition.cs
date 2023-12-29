using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class BSMConditionMultiCondition : BSMConditionBase
{
    enum BSMConditionOperator
    {
        OR,
        AND,
        XOR,
        NOR,
        NAND,
    }

    [SerializeField] List<BSMConditionBase> m_conditions = new List<BSMConditionBase>();
    VisualElement m_baseContainer;

    public override VisualElement GetElement()
    {
        VisualElement bloc = new VisualElement();



        return bloc;
    }

    void UpdateBaseContainer()
    {

    }

    void AddCondition(BSMConditionBase condition)
    {
        m_conditions.Add(condition);
        UpdateBaseContainer();
    }

    void RemoveCondition(int index)
    {
        if (index < 0 || index >= m_conditions.Count)
            return;

        m_conditions.RemoveAt(index);
        UpdateBaseContainer();
    }
}

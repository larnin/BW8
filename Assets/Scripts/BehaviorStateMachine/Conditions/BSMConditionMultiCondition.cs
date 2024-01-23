using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class BSMConditionMultiCondition : BSMConditionBase
{
    public enum BSMConditionOperator
    {
        OR,
        AND,
        XOR,
        NOR,
        NAND,
    }

    [SerializeField] List<BSMConditionBase> m_conditions = new List<BSMConditionBase>();
    [SerializeField] BSMConditionOperator m_operator = BSMConditionOperator.AND;
    public BSMConditionOperator conditionOperator { get { return m_operator; } set { m_operator = value; } }

    public void AddCondition(BSMConditionBase condition)
    {
        m_conditions.Add(condition);
    }

    public void RemoveCondition(int index)
    {
        if (index < 0 || index >= m_conditions.Count)
            return;

        m_conditions.RemoveAt(index);
    }

    public int GetConditionNb()
    {
        return m_conditions.Count;
    }

    public BSMConditionBase GetCondition(int index)
    {
        if (index < 0 || index >= m_conditions.Count)
            return null;

        return m_conditions[index];
    }
}

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
        XNOR,
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

    public override bool IsValid()
    {
        int nbValid = 0;

        foreach(var condition in m_conditions)
        {
            if (condition.IsValid())
                nbValid++;
        }

        switch(m_operator)
        {
            case BSMConditionOperator.OR:
                return nbValid > 0;
            case BSMConditionOperator.AND:
                return nbValid == m_conditions.Count;
            case BSMConditionOperator.XOR:
                return nbValid % 2 == 0;
            case BSMConditionOperator.NAND:
                return nbValid < m_conditions.Count;
            case BSMConditionOperator.NOR:
                return nbValid == 0;
            case BSMConditionOperator.XNOR:
                return nbValid % 2 != 0;
        }

        return false;
    }
}

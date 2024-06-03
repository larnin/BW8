using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class QuestConditionList
{
    enum QuestConditionsOperator
    {
        OR,
        AND,
        XOR,
        NOR,
        NAND,
        XNOR,
    }

    [SerializeField] List<QuestCondition> m_condition = new List<QuestCondition>();
    [SerializeField] QuestConditionsOperator m_operator = QuestConditionsOperator.AND;

    public bool IsValid()
    {
        int nbValid = 0;

        foreach(var condition in m_condition)
        {
            if (condition.IsValid())
                nbValid++;
        }

        switch(m_operator)
        {
            case QuestConditionsOperator.OR:
                return nbValid > 0;
            case QuestConditionsOperator.AND:
                return nbValid == m_condition.Count;
            case QuestConditionsOperator.XOR:
                return nbValid % 2 == 1;
            case QuestConditionsOperator.NOR:
                return nbValid == 0;
            case QuestConditionsOperator.NAND:
                return nbValid != m_condition.Count;
            case QuestConditionsOperator.XNOR:
                return nbValid % 2 == 0;
        }
        return false;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMConditionInverseCondition : BSMConditionBase
{
    [SerializeField] BSMConditionBase m_condition;

    public BSMConditionBase condition { get { return m_condition; } set { m_condition = value; } }

    public override bool IsValid()
    {
        if (m_condition == null)
            return false;

        return !m_condition.IsValid();
    }
}

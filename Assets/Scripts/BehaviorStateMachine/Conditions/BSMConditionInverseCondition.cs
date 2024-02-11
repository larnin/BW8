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

    public override void Load(JsonObject obj)
    {
        m_condition = null;

        var conditionElt = obj.GetElement("Condition");
        if(conditionElt != null && conditionElt.IsJsonObject())
        {
            var conditionObj = conditionElt.JsonObject();
            var condition = BSMConditionBase.LoadCondition(conditionObj);
            m_condition = condition;
        }
    }

    public override void Save(JsonObject obj)
    {
        if (m_condition != null)
        {
            var condObj = BSMConditionBase.SaveCondition(m_condition);
            if (condObj != null)
                obj.AddElement("Condition", condObj);
        }
    }

    public override bool IsValid()
    {
        if (m_condition == null)
            return false;

        return !m_condition.IsValid();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMConditionOnTriggerSelf : BSMConditionBase
{
    static string triggerName = "Trigger";

    public BSMConditionOnTriggerSelf()
    {
        AddAttribute(triggerName, new BSMAttributeObject(""));
    }

    public override bool IsValid()
    {
        string trigger = GetStringAttribute(triggerName);

        if (trigger == null)
            return false;

        GetFirstQuestEntityEvent getEntity = new GetFirstQuestEntityEvent(trigger);
        Event<GetFirstQuestEntityEvent>.Broadcast(getEntity);

        if (getEntity.entity == null)
            return false;

        var collider = getEntity.entity.GetComponent<Collider2D>();
        if (collider == null)
            return false;

        return collider.OverlapPoint(GetControler().transform.position);
    }
}

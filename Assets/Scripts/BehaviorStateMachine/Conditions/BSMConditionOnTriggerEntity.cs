using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMConditionOnTriggerEntity : BSMConditionBase
{
    static string triggerName = "Trigger";
    static string targetName = "Target";

    public BSMConditionOnTriggerEntity()
    {
        AddAttribute(triggerName, new BSMAttributeObject(""));
        AddAttribute(targetName, BSMAttributeObject.Create((GameObject)null));
    }

    public override bool IsValid()
    {
        string trigger = GetStringAttribute(triggerName);
        var entity = GetUnityObjectAttribute<GameObject>(targetName);

        if (trigger == null || entity == null)
            return false;

        GetFirstQuestEntityEvent getEntity = new GetFirstQuestEntityEvent(trigger);
        Event<GetFirstQuestEntityEvent>.Broadcast(getEntity);

        if (getEntity.entity == null)
            return false;

        var collider = getEntity.entity.GetComponent<Collider2D>();
        if (collider == null)
            return false;

        return collider.OverlapPoint(entity.transform.position);
    }
}

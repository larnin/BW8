using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMConditionOnTriggerTag : BSMConditionBase
{
    static string triggerName = "Trigger";
    static string targetName = "Target";
    static string countName = "Count";

    public BSMConditionOnTriggerTag()
    {
        AddAttribute(triggerName, new BSMAttributeObject(""));
        AddAttribute(targetName, new BSMAttributeObject(""));
        AddAttribute(countName, new BSMAttributeObject(1));
    }

    public override bool IsValid()
    {
        string trigger = GetStringAttribute(triggerName);
        string entity = GetStringAttribute(targetName);
        int count = GetIntAttribute(countName);

        if (count <= 0)
            return true;

        if (trigger == null || entity == null)
            return false;

        GetFirstQuestEntityEvent getEntity = new GetFirstQuestEntityEvent(trigger);
        Event<GetFirstQuestEntityEvent>.Broadcast(getEntity);

        if (getEntity.entity == null)
            return false;

        var collider = getEntity.entity.GetComponent<Collider2D>();
        if (collider == null)
            return false;

        GetAllQuestEntityEvent getEntities = new GetAllQuestEntityEvent(entity);
        Event<GetAllQuestEntityEvent>.Broadcast(getEntities);

        int nbOnTrigger = 0;

        foreach(var e in getEntities.entities)
        {
            Vector2 pos = e.transform.position;
            if (collider.OverlapPoint(pos))
                nbOnTrigger++;
        }

        return nbOnTrigger >= count ;
    }
}

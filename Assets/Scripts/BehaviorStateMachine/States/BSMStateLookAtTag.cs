using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMStateLookAtTag : BSMStateBase
{
    static string targetName = "Target";

    public BSMStateLookAtTag()
    {
        AddAttribute(targetName, new BSMAttributeObject(""));
    }

    public override void BeginUpdate()
    {
        var targetTag = GetStringAttribute(targetName, "");
        GetFirstQuestEntityEvent getEntity = new GetFirstQuestEntityEvent(targetTag);
        Event<GetFirstQuestEntityEvent>.Broadcast(getEntity);

        if (getEntity.entity != null)
        {
            var direction = getEntity.entity.transform.position - GetControler().transform.position;
            var dir = AnimationDirectionEx.GetDirection(direction);
            Event<SetLookDirectionEvent>.Broadcast(new SetLookDirectionEvent(dir), m_controler.gameObject);
        }

        Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
    }

    public override void Update()
    {
        Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
    }
}

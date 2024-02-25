using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMStateSendMessageToTag : BSMStateBase
{
    static string messageName = "Message";
    static string targetName = "Target";

    public BSMStateSendMessageToTag()
    {
        AddAttribute(messageName, new BSMAttributeObject(""));
        AddAttribute(targetName, new BSMAttributeObject(""));
    }

    public override void OnBeginUpdate()
    {
        var targetTag = GetStringAttribute(targetName, "");
        var message = GetStringAttribute(messageName, "");

        GetAllQuestEntityEvent getEntities = new GetAllQuestEntityEvent(targetTag);
        Event<GetAllQuestEntityEvent>.Broadcast(getEntities);

        foreach(var target in getEntities.entities)
            Event<BSMSendMessageEvent>.Broadcast(new BSMSendMessageEvent(message), target.gameObject);

        Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
    }

    public override void Update()
    {
        Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
    }
}

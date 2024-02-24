using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMStateSendMessageToEntity : BSMStateBase
{
    static string messageName = "Message";
    static string targetName = "Target";

    public BSMStateSendMessageToEntity()
    {
        AddAttribute(messageName, new BSMAttributeObject(""));
        AddAttribute(targetName, BSMAttributeObject.CreateUnityObject((GameObject)null));
    }

    public override void BeginUpdate()
    {
        var target = GetUnityObjectAttribute(targetName, (GameObject)null);
        var message = GetStringAttribute(messageName, "");

        if (target != null)
            Event<BSMSendMessageEvent>.Broadcast(new BSMSendMessageEvent(message), target);

        Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
    }

    public override void Update()
    {
        Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
    }
}

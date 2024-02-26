using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMActionSendMessageToEntity : BSMActionBase
{
    static string messageName = "Message";
    static string targetName = "Target";

    public BSMActionSendMessageToEntity()
    {
        AddAttribute(messageName, new BSMAttributeObject(""));
        AddAttribute(targetName, BSMAttributeObject.CreateUnityObject((GameObject)null));
    }

    public override void Exec()
    {
        var target = GetUnityObjectAttribute(targetName, (GameObject)null);
        var message = GetStringAttribute(messageName, "");

        if (target != null)
            Event<BSMSendMessageEvent>.Broadcast(new BSMSendMessageEvent(message), target);
    }
}


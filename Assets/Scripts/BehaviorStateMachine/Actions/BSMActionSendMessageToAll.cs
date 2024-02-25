using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BSMActionSendMessageToAll : BSMActionBase
{
    static string messageName = "Message";

    public BSMActionSendMessageToAll()
    {
        AddAttribute(messageName, new BSMAttributeObject(""));
    }

    public override void Exec()
    {
        var message = GetStringAttribute(messageName, "");

        Event<BSMSendMessageEvent>.Broadcast(new BSMSendMessageEvent(message));
    }
}
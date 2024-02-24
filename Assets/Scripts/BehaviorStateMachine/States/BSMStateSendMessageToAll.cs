using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BSMStateSendMessageToAll : BSMStateBase
{
    static string messageName = "Message";

    public BSMStateSendMessageToAll()
    {
        AddAttribute(messageName, new BSMAttributeObject(""));
    }

    public override void BeginUpdate()
    {
        var message = GetStringAttribute(messageName, "");

        Event<BSMSendMessageEvent>.Broadcast(new BSMSendMessageEvent(message));

        Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
    }

    public override void Update()
    {
        Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
    }
}

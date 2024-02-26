using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BSMActionSendMessageToTag  : BSMActionBase
{
    static string messageName = "Message";
    static string targetName = "Target";

    public BSMActionSendMessageToTag()
    {
        AddAttribute(messageName, new BSMAttributeObject(""));
        AddAttribute(targetName, new BSMAttributeObject(""));
    }

    public override void Exec()
    {
        var targetTag = GetStringAttribute(targetName, "");
        var message = GetStringAttribute(messageName, "");

        GetAllQuestEntityEvent getEntities = new GetAllQuestEntityEvent(targetTag);
        Event<GetAllQuestEntityEvent>.Broadcast(getEntities);

        foreach (var target in getEntities.entities)
            Event<BSMSendMessageEvent>.Broadcast(new BSMSendMessageEvent(message), target.gameObject);
    }
}

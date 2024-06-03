using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BSMActionLookAtTag : BSMActionBase
{
    static string targetName = "Target";

    public BSMActionLookAtTag()
    {
        AddAttribute(targetName, new BSMAttributeObject(""));
    }

    public override void Exec()
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
    }

}

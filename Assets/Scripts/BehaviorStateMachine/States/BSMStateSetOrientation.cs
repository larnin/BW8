using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BSMStateSetOrientation : BSMStateBase
{
    static string orientationName = "Orientation";

    public BSMStateSetOrientation()
    {
        AddAttribute(orientationName, BSMAttributeObject.CreateEnum(AnimationDirection.Down));
    }

    public override void OnBeginUpdate()
    {
        var dir = GetEnumAttribute(orientationName, AnimationDirection.Down);

        Event<SetLookDirectionEvent>.Broadcast(new SetLookDirectionEvent(dir));
        Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
    }

    public override void Update()
    {
        Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
    }
}

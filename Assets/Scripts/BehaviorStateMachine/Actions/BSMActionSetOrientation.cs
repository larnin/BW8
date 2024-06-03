using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BSMActionSetOrientation : BSMActionBase
{
    static string orientationName = "Orientation";

    public BSMActionSetOrientation()
    {
        AddAttribute(orientationName, BSMAttributeObject.CreateEnum(AnimationDirection.Down));
    }

    public override void Exec()
    {
        var dir = GetEnumAttribute(orientationName, AnimationDirection.Down);

        Event<SetLookDirectionEvent>.Broadcast(new SetLookDirectionEvent(dir), GetControler().gameObject);
    }
}

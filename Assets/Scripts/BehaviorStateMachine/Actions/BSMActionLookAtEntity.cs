using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMActionLookAtEntity : BSMActionBase
{
    static string targetName = "Target";

    public BSMActionLookAtEntity()
    {
        AddAttribute(targetName, BSMAttributeObject.CreateUnityObject((GameObject)null));
    }

    public override void Exec()
    {
        var target = GetUnityObjectAttribute(targetName, (GameObject)null);
        if (target != null)
        {
            var direction = target.transform.position - GetControler().transform.position;
            var dir = AnimationDirectionEx.GetDirection(direction);
            Event<SetLookDirectionEvent>.Broadcast(new SetLookDirectionEvent(dir), m_controler.gameObject);
        }
    }
}

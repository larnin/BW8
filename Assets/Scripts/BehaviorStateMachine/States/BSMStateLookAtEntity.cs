using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMStateLookAtEntity : BSMStateBase
{
    static string targetName = "Target";

    public BSMStateLookAtEntity()
    {
        AddAttribute(targetName, BSMAttributeObject.CreateUnityObject((GameObject)null));
    }

    public override void OnBeginUpdate()
    {
        var target = GetUnityObjectAttribute(targetName, (GameObject)null);
        if(target != null)
        {
            var direction = target.transform.position - GetControler().transform.position;
            var dir = AnimationDirectionEx.GetDirection(direction);
            Event<SetLookDirectionEvent>.Broadcast(new SetLookDirectionEvent(dir), m_controler.gameObject);
        }

        Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
    }

    public override void Update()
    {
        Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
    }
}

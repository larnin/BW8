using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SetExclusiveBehaviourEvent
{
    public MonoBehaviour behaviour;

    public SetExclusiveBehaviourEvent(MonoBehaviour _behaviour)
    {
        behaviour = _behaviour;
    }
}

public class GetExclusiveBehaviourEvent
{
    public MonoBehaviour behaviour;
}

public class SetDefaultBehaviourEvent { }

public class GetDefaultBehaviourEvent
{
    public MonoBehaviour defaultBehaviour;
}
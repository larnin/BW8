using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SetProjectileDataEvent
{
    public float speed = -1;
    public float maxDistance = -1;

    public int damages = -1;
    public float knockback = -1;
    public LayerMask hitLayer;

    public GameObject caster;
}

public class ThrowEvent { }

public class CanBeAttractedEvent
{
    public bool canBeAttracted = false;
}

public class IsDetroyedWhenCatch
{
    public bool destroyedWhenCatch = false;
}

public class StartAttractEvent
{
    public GameObject target;
    public float speedMultiplier = 1;

    public StartAttractEvent(GameObject _target, float _speedMultiplier = 1)
    {
        target = _target;
        speedMultiplier = _speedMultiplier;
    }
}

public class StopAttractEvent { }

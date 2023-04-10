using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CenterUpdatedEventInstant
{
    public Vector2 pos;

    public CenterUpdatedEventInstant(Vector2 _pos)
    {
        pos = _pos;
    }
}

public class CenterUpdatedEvent
{
    public Vector2 pos;

    public CenterUpdatedEvent(Vector2 _pos)
    {
        pos = _pos;
    }
}

public class TeleportPlayerEvent
{
    public Vector2 pos;

    public TeleportPlayerEvent(Vector2 _pos)
    {
        pos = _pos;
    }
}

public class GetOffsetVelocityEvent
{
    public float velocityMultiplier;
    public Vector2 offsetVelocity;

    public GetOffsetVelocityEvent()
    {
        velocityMultiplier = 1;
        offsetVelocity = Vector2.zero;
    }
}

public class GetStatusEvent
{
    public Vector2 direction;
    public bool rolling;

    public GetStatusEvent()
    {
        direction = Vector2.zero;
        rolling = false;
    }
}

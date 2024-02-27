using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StartMoveEvent
{
    public Vector3 target;

    public StartMoveEvent(Vector3 _target)
    {
        target = _target;
    }
}

public class StartFollowEvent
{
    public GameObject target;

    public StartFollowEvent(GameObject _target)
    {
        target = _target;
    }
}

public class StopMoveEvent { }

public class IsMovingEvent
{
    public bool isMoving = false;
}

public class SetLookDirectionEvent
{
    public AnimationDirection lookDirection;

    public SetLookDirectionEvent(AnimationDirection dir)
    {
        lookDirection = dir;
    }
}

public class MoveEndJumpEvent { }

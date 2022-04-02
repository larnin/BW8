using UnityEngine;

class StartRollEvent { }
class EndRollEvent { }

class StartUseEvent { }
class EndUseEvent { }

class GetInputsEvent
{
    public Vector2 direction;
    public bool roll;
    public bool use;
}
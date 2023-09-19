using UnityEngine;

public class StartRollEvent { }
public class EndRollEvent { }

public class StartUseWeaponEvent { }
public class EndUseWeaponEvent { }

public class StartUseItemEvent { }
public class EndUseItemEvent { }

public class StartInteractEvent { }
public class EndInteractEvent { }

public class GetInputsEvent
{
    public Vector2 direction;
    public bool roll;
    public bool useWeapon;
    public bool useItem;
    public bool interact;
}

// ui controls event

public class StartSubmitEvent { }
public class EndSubmitEvent { }

public class StartCancelEvent { }
public class EndCancelEvent { }

public class StartClickEvent { }
public class EndClickEvent { }

public class StartMiddleClickEvent { }
public class EndMiddleClickEvent { }

public class StartRightClickEvent { }
public class EndRightClickEvent { }

public class GetUIInputsEvent
{
    public Vector2 navigation;
    public bool submit;
    public bool cancel;
    public Vector2 point;
    public Vector2 scroll;
    public bool click;
    public bool middleClick;
    public bool rightClick;
}
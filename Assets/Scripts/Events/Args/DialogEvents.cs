using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StartDialogEvent
{
    public GameObject target;
    public DialogObject dialog;

    public StartDialogEvent(GameObject _target, DialogObject _dialog)
    {
        target = _target;
        dialog = _dialog;
    }
}

public class EndDialogEvent { }

public class IsDialogEndedEvent
{
    public bool ended = false;
}
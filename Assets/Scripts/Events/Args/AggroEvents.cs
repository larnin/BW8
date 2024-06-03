using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UpdateAggroTargetEvent
{
    public GameObject target;

    public UpdateAggroTargetEvent(GameObject _target)
    {
        target = _target;
    }
}

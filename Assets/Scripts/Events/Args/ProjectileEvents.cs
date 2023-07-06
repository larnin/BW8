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

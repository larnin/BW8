using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class SwordOneAttack
{
    public float hitDistance = 1;
    public float hitDelay = 0.1f;
    public float hitRadius = 0.5f;
    public float hitStartAngle = 0;
    public float hitEndAngle = 0;
    public float hitDuration = 0.3f;
    public int damage = 1;
    public float knockback = 1;

    public float moveDistance = 0;
    public float moveDelay = 0.1f;
    public float moveDuration = 0.5f;
}

public class SwordData : ScriptableObject
{
    public SwordOneAttack attack1;
    public SwordOneAttack attack2;
    public SwordOneAttack attack3;
    public SwordOneAttack attackDash;

    public LayerMask hitLayer;
    public float attackCooldown = 0.5f;
}

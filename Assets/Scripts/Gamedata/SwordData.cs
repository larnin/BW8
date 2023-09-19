using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SwordData : ScriptableObject
{
    public float attackDistance = 1;
    public float attackDelay = 0.1f;
    public float attackRadius = 0.5f;
    public float attackDuration = 0.3f;
    public float attackCooldown = 0.5f;
    public int damage = 1;
    public float knockback = 1;
}

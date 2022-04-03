using System.Collections;
using UnityEngine;

public class GetLifeEvent
{
    public int life;
    public int maxLife;

    public GetLifeEvent(int _life, int _maxLife)
    {
        life = _life;
        maxLife = _maxLife;
    }
}

public class RegenLifeEvent
{
    public int value;

    public RegenLifeEvent(int _value)
    {
        value = _value;
    }
}

public class HitEvent
{
    public int damage;
    public GameObject caster;
    public float knockback;

    public HitEvent(int _damage, GameObject _caster, float _knockback)
    {
        damage = _damage;
        caster = _caster;
        knockback = _knockback;
    }
}

public class DeathEvent { }

public class LifeLossEvent
{
    public GameObject caster;
    public float knockback;

    public LifeLossEvent(GameObject _caster, float _knockback)
    {
        caster = _caster;
        knockback = _knockback;
    }
}

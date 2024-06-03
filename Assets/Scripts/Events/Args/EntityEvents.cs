using System.Collections;
using UnityEngine;

public class GetPlayerLifeEvent
{
    public int life;
    public int maxLife;
}

public class GetPlayerMoneyEvent
{
    public int money;
}

public class GetPlayerPositionEvent
{
    public Vector2 pos;
}

public class GetLifeEvent
{
    public int life;
    public int maxLife;

    public GetLifeEvent(int _life, int _maxLife)
    {
        life = _life;
        maxLife = _maxLife;
    }

    public GetLifeEvent()
    {
        life = 0;
        maxLife = 0;
    }
}

public class SetLifeEvent
{
    public int life;

    public SetLifeEvent(int _life)
    {
        life = _life;
    }
}

public class SetLifePercentEvent
{
    public float lifePercent;

    public SetLifePercentEvent(float _lifePercent)
    {
        lifePercent = _lifePercent;
        if (lifePercent < 0)
            lifePercent = 0;
        if (lifePercent > 1)
            lifePercent = 1;
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

public class DeathEvent 
{
    public GameObject caster;

    public DeathEvent(GameObject _caster)
    {
        caster = _caster;
    }
}

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

public class LifeHealEvent { }

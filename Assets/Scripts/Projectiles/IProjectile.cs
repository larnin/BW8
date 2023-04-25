using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum ProjectileType
{
    Stone,
    Bomb
}

public interface IProjectile
{
    public void SetVelocity(float speed);
    public void SetMaxDistance(float distance);

    public void SetDamage(int damages, float knockback);
    public void SetHitLayer(LayerMask hitLayer);

    public void Throw();
}

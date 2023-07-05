using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ProjectileList : ScriptableObject
{
    [Serializable]
    public class OneProjectile
    {
        public ProjectileType type;
        public GameObject projectile;
    }

    [SerializeField] List<OneProjectile> m_projectiles;

    public GameObject GetProjectile(ProjectileType type)
    {
        foreach(var p in m_projectiles)
        {
            if (p.type == type)
            {
                var projectile = p.projectile.GetComponent<IProjectile>();
                if (projectile != null)
                    projectile.SetProjectileType(type);
                return p.projectile;
            }
        }

        return null;
    }
}

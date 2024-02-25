using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HitOnContact : MonoBehaviour
{
    [SerializeField] int m_damages = 1;
    [SerializeField] float m_knockback = 0;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var commonData = World.common;
        if ((commonData.playerLayer & (1 << collision.gameObject.layer)) != 0)
        {
            Event<HitEvent>.Broadcast(new HitEvent(m_damages, gameObject, m_knockback), collision.gameObject);
        }
    }
}

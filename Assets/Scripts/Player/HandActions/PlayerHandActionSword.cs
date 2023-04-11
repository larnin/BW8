using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerHandActionSword : PlayerHandActionBase
{
    float m_duration = 0;
    bool m_haveAttacked = false;
    bool m_waiting = true;
    Vector2 m_direction = Vector2.zero;

    public PlayerHandActionSword(PlayerHandController player) : base(player)
    {
        
    }

    public override void OnPress()
    {
        if (!m_waiting)
            return;

        GetStatusEvent status = new GetStatusEvent();
        Event<GetStatusEvent>.Broadcast(status, m_player.gameObject);
        if (status.lockActions)
            return;

        m_waiting = false;
        m_haveAttacked = false;
        m_duration = 0;

        AnimationDirection dir = AnimationDirectionEx.GetDirection(status.direction);
        m_direction = AnimationDirectionEx.GetDirection(dir);

        Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent("Attack", dir, 2), m_player.gameObject);
    }

    public override void Process(bool inputPressed)
    {
        if (m_waiting)
            return;

        m_duration += Time.deltaTime;

        if (m_duration >= World.sword.attackDelay && !m_haveAttacked)
        {
            m_haveAttacked = true;

            Vector2 pos = m_player.transform.position;
            pos += m_direction * World.sword.attackDistance;

            var cols = Physics2D.OverlapCircleAll(pos, World.sword.attackRadius);

            foreach (var col in cols)
            {
                if (col.gameObject.layer == m_player.gameObject.layer)
                    continue;

                Event<HitEvent>.Broadcast(new HitEvent(World.sword.damage, m_player.gameObject, World.sword.knockback), col.gameObject);
            }
        }

        if (m_duration >= World.sword.attackCooldown)
        {
            m_waiting = true;
            m_duration = 0;
            m_haveAttacked = false;
        }
    }

    public override bool AreActionsLocked()
    {
        return !m_waiting;
    }
}

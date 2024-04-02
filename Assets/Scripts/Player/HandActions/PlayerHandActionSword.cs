using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerHandActionSword : PlayerHandActionBase
{
    enum AttackState
    {
        none,
        cooldown,
        attack1,
        attack2,
        attack3,
        attackDash,
    }

    AttackState m_state = AttackState.none;
    float m_duration = 0;
    float m_maxDuration;

    Vector2 m_direction = Vector2.zero;

    bool m_havePressedAttack = false;

    List<GameObject> m_hitReminder = new List<GameObject>();

    Vector2 m_velocity = Vector2.zero;

    public PlayerHandActionSword(PlayerHandController player) : base(player)
    {
        
    }

    public override void OnPress()
    {
        if (m_state == AttackState.cooldown || m_state == AttackState.attackDash || m_state == AttackState.attack3)
            return;

        GetStatusEvent status = new GetStatusEvent();
        Event<GetStatusEvent>.Broadcast(status, m_player.gameObject);
        if (status.lockActions && !AreActionsLocked() && !status.dashing)
            return;

        if(m_state == AttackState.none)
        {
            m_havePressedAttack = false;

            if (status.dashing)
                m_state = AttackState.attackDash;
            else m_state = AttackState.attack1;

            AnimationDirection dir = AnimationDirectionEx.GetDirection(status.direction);
            m_direction = AnimationDirectionEx.GetDirection(dir);

            StartCurrentAnimation();
        }
        else m_havePressedAttack = true;
    }

    public override void BeginProcess()
    {
        m_state = AttackState.none;
    }

    public override void Process(bool inputPressed)
    {
        if (m_state == AttackState.none)
            return;

        m_duration += Time.deltaTime;

        if(m_state == AttackState.cooldown)
        {
            if (m_duration >= m_maxDuration)
                m_state = AttackState.none;
            return;
        }

        if(m_duration >= m_maxDuration)
        {
            AttackState nextState = AttackState.cooldown;

            if(m_havePressedAttack)
            {
                if (m_state == AttackState.attack1)
                    nextState = AttackState.attack2;
                else if (m_state == AttackState.attack2)
                    nextState = AttackState.attack3;
            }

            m_state = nextState;
            m_duration = 0;

            if(m_state == AttackState.cooldown)
            {
                m_maxDuration = World.sword.attackCooldown;
                return;
            }

            StartCurrentAnimation();
        }

        var data = GetData(m_state);
        if (data == null)
            return;

        if(m_duration >= data.hitDelay && m_duration <= data.hitDelay + data.hitDuration)
        {
            Vector2 pos = m_player.transform.position;
            pos += m_direction * data.hitDistance;

            var cols = Physics2D.OverlapCircleAll(pos, data.hitRadius, World.sword.hitLayer);

            foreach(var col in cols)
            {
                if (m_hitReminder.Contains(col.gameObject))
                    continue;

                Event<HitEvent>.Broadcast(new HitEvent(data.damage, m_player.gameObject, data.knockback), col.gameObject);
            }
        }

        if (m_duration >= data.moveDelay && m_duration <= data.moveDelay + data.moveDuration)
        {
            float speed = data.moveDistance / data.moveDuration;

            m_velocity = m_direction * speed;
        }
        else m_velocity = Vector2.zero;
    }
    public override void GetVelocity(out Vector2 offsetVelocity, out float velocityMultiplier)
    {
        offsetVelocity = Vector2.zero;
        velocityMultiplier = 1;

        if (m_state == AttackState.none || m_state == AttackState.cooldown)
            return;

        offsetVelocity = m_velocity;
        velocityMultiplier = 0;
    }

    public override bool AreActionsLocked()
    {
        return m_state != AttackState.none && m_state != AttackState.cooldown;
    }

    void StartCurrentAnimation()
    {
        var dir = AnimationDirectionEx.GetDirection(m_direction);

        string animName = GetAnimName(m_state);
        Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(animName, dir, 2), m_player.gameObject);

        GetAnimationDurationEvent duration = new GetAnimationDurationEvent(animName, dir);
        Event<GetAnimationDurationEvent>.Broadcast(duration, m_player.gameObject);

        m_duration = 0;
        m_maxDuration = duration.duration;
        m_hitReminder.Clear();

        m_velocity = Vector2.zero;
        m_havePressedAttack = false;
    }

    static SwordOneAttack GetData(AttackState state)
    {
        switch(state)
        {
            case AttackState.attack1:
                return World.sword.attack1;
            case AttackState.attack2:
                return World.sword.attack2;
            case AttackState.attack3:
                return World.sword.attack3;
            case AttackState.attackDash:
                return World.sword.attackDash;
            default:
                return null;
        }
    }

    static string GetAnimName(AttackState state)
    {
        switch(state)
        {
            case AttackState.attack1:
                return "Attack_1";
            case AttackState.attack2:
                return "Attack_2";
            case AttackState.attack3:
                return "Attack_3";
            case AttackState.attackDash:
                return "Dash_Attack";
            default:
                return "NULL";
        }
    }
}

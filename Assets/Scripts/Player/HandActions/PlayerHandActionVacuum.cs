using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerHandActionVacuum : PlayerHandActionBase
{
    const string startAnim = "Vacuum_Start";
    const string loopAnim = "Vacuum_Loop";
    const string moveAnim = "Vacuum_Move";
    const string endAnim = "Vacuum_End";

    enum State
    {
        Disabled,
        Start,
        Loop,
        End,
    }

    State m_state = State.Disabled;
    float m_duration = 0;
    Vector2 m_direction = Vector2.zero;
    bool m_moving = false;


    public PlayerHandActionVacuum(PlayerHandController player) : base(player)
    {

    }

    public override void OnPress()
    {
        if (m_state != State.Disabled)
            return;

        GetStatusEvent status = new GetStatusEvent();
        Event<GetStatusEvent>.Broadcast(status, m_player.gameObject);
        if (status.lockActions)
            return;

        m_state = State.Start;
        m_moving = false;

        AnimationDirection dir = AnimationDirectionEx.GetDirection(status.direction);
        m_direction = AnimationDirectionEx.GetDirection(dir);

        var duration = new GetAnimationDurationEvent(startAnim, dir);
        Event<GetAnimationDurationEvent>.Broadcast(duration, m_player.gameObject);
        m_duration = duration.duration;
        Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(startAnim, dir, 2), m_player.gameObject);
    }

    public override void BeginProcess()
    {
        m_state = State.Disabled;
        m_duration = 0;
    }

    public override void Process(bool inputPressed)
    {
        if (m_state == State.Disabled)
            return;

        m_duration -= Time.deltaTime;

        switch(m_state)
        {
            case State.Start:
                {
                    if(m_duration <= 0)
                    {
                        m_state = State.Loop;
                        AnimationDirection dir = AnimationDirectionEx.GetDirection(m_direction);
                        Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(loopAnim, dir, 2, true), m_player.gameObject);
                    }
                    break;
                }
            case State.Loop:
                {
                    ProcessLoop(inputPressed);
                    break;
                }
            case State.End:
                {
                    if(m_duration <= 0)
                    {
                        m_state = State.Disabled;
                        m_duration = 0;
                    }
                    break;
                }
            default:
                break;
        }
    }

    public override void GetVelocity(out Vector2 offsetVelocity, out float velocityMultiplier)
    {
        offsetVelocity = Vector2.zero;
        velocityMultiplier = 1;

        if (m_state == State.Start || m_state == State.End)
            velocityMultiplier = 0;
        else if (m_state == State.Loop)
            velocityMultiplier = World.vacuum.moveSpeedMultiplier;

    }

    public override bool AreActionsLocked()
    {
        return m_state != State.Disabled;
    }

    void ProcessLoop(bool inputPressed)
    {
        if(!inputPressed)
        {
            m_state = State.End;
            AnimationDirection dir = AnimationDirectionEx.GetDirection(m_direction);
            var duration = new GetAnimationDurationEvent(endAnim, dir);
            Event<GetAnimationDurationEvent>.Broadcast(duration, m_player.gameObject);
            m_duration = duration.duration;
            Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(endAnim, dir, 2), m_player.gameObject);
            return;
        }

        GetStatusEvent status = new GetStatusEvent();
        Event<GetStatusEvent>.Broadcast(status, m_player.gameObject, m_player.gameObject);

        bool moving = status.velocity.magnitude > 0.1f;
        Debug.Log(status.velocity.magnitude);
        if(moving != m_moving)
        {
            m_moving = moving;
            AnimationDirection dir = AnimationDirectionEx.GetDirection(m_direction);

            if (m_moving)
                Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(moveAnim, dir, 2, true), m_player.gameObject);
            else Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(loopAnim, dir, 2, true), m_player.gameObject);
        }
    }
}

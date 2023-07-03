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

    class ParticleData
    {
        public ParticleSystem system;
        public float duration;
        public float initialEmission;
    }

    State m_state = State.Disabled;
    float m_duration = 0;
    Vector2 m_direction = Vector2.zero;
    bool m_moving = false;

    AnimationDirection m_particleDirection = AnimationDirection.none;
    GameObject m_particules;
    List<ParticleData> m_particlesDatas = new List<ParticleData>();


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
        UpdateParticles();

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
        if(moving != m_moving)
        {
            m_moving = moving;
            AnimationDirection dir = AnimationDirectionEx.GetDirection(m_direction);

            if (m_moving)
                Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(moveAnim, dir, 2, true), m_player.gameObject);
            else Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(loopAnim, dir, 2, true), m_player.gameObject);
        }
    }

    void UpdateParticles()
    {
        float maxDuration = World.vacuum.particleAppearDuration;

        if (m_state == State.Start || m_state == State.Loop)
        {
            if(m_particules != null)
            {
                AnimationDirection currentDirection = AnimationDirectionEx.GetDirection(m_direction);
                if (currentDirection != m_particleDirection)
                    DestroyParticleSystem();
            }

            if (m_particules == null)
                MakeParticleSystem();
            foreach (var p in m_particlesDatas)
            {
                p.duration += Time.deltaTime;
                if (p.duration > maxDuration)
                    p.duration = maxDuration;
            }
        }
        else if(m_state == State.End || m_state == State.Disabled)
        {
            if(m_particules != null)
            {
                int nbStopped = 0;
                foreach(var p in m_particlesDatas)
                {
                    p.duration -= Time.deltaTime * 10;
                    if(p.duration <= 0)
                    {
                        p.duration = 0;
                        nbStopped++;
                    }
                }
                if (nbStopped >= m_particlesDatas.Count)
                    DestroyParticleSystem();
            }
        }

        foreach(var p in m_particlesDatas)
        {
            float multiplier = p.duration / maxDuration;
            if (multiplier > 1)
                multiplier = 1;

            var em = p.system.emission;
            em.rateOverTimeMultiplier =  multiplier * p.initialEmission;
        }
    }

    void MakeParticleSystem()
    {
        if (m_particules != null)
            DestroyParticleSystem();

        m_particleDirection = AnimationDirectionEx.GetDirection(m_direction);

        GameObject selectedPrefab = World.vacuum.particlesLeftPrefab;
        if (m_particleDirection == AnimationDirection.Up)
            selectedPrefab = World.vacuum.particlesUpPrefab;
        else if (m_particleDirection == AnimationDirection.Down)
            selectedPrefab = World.vacuum.particleDownPrefab;

        m_particules = GameObject.Instantiate(selectedPrefab);
        m_particules.transform.parent = m_player.transform;
        m_particules.transform.localPosition = Vector3.zero;
        m_particules.transform.localRotation = Quaternion.identity;

        if (m_particleDirection == AnimationDirection.Right)
            m_particules.transform.localRotation = Quaternion.Euler(0, 0, 180);

        var particleSystems = m_particules.GetComponentsInChildren<ParticleSystem>();
        foreach(var p in particleSystems)
        {
            var data = new ParticleData();
            data.duration = 0;
            data.system = p;
            var em = p.emission;
            data.initialEmission = em.rateOverTimeMultiplier;
            m_particlesDatas.Add(data);
        }
    }

    void DestroyParticleSystem()
    {
        if (m_particules == null)
            return;

        foreach (var p in m_particlesDatas)
        {
            var em = p.system.emission;
            em.rateOverTimeMultiplier = 0;
        }

        GameObject.Destroy(m_particules, 5);
        m_particules = null;
        m_particlesDatas.Clear();
    }
}

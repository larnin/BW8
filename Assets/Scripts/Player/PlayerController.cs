using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum DashState
    {
        none,
        start, 
        loop, 
        end
    }

    [SerializeField] float m_maxSpeed = 1;
    [SerializeField] float m_acceleration = 2;

    [SerializeField] float m_dashSpeed = 2;
    [SerializeField] float m_dashDistance = 1;

    Rigidbody2D m_rigidbody = null;

    SubscriberList m_subscriberList = new SubscriberList();

    Vector2 m_inputsDirection;
    bool m_inputsStartDash;

    Vector2 m_oldPosition;
    Vector2 m_direction;
    DashState m_dashState = DashState.none;
    float m_dashDuration = 0;
    float m_dashMaxDuration = 0;


    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();

        m_subscriberList.Add(new Event<StartDashEvent>.LocalSubscriber(OnStartDash, gameObject));
        m_subscriberList.Add(new Event<GetStatusEvent>.LocalSubscriber(GetStatus, gameObject));

        m_subscriberList.Add(new Event<TeleportPlayerEvent>.Subscriber(OnTeleport));
        m_subscriberList.Add(new Event<GetPlayerLifeEvent>.Subscriber(GetPlayerLife));
        m_subscriberList.Add(new Event<GetPlayerPositionEvent>.Subscriber(GetPlayerPos));

        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    private void Start()
    {
        m_oldPosition = transform.position;
        m_direction = new Vector2(0, -1);

        Event<CenterUpdatedEventInstant>.Broadcast(new CenterUpdatedEventInstant(transform.position));

        HUD.Open();
    }

    private void FixedUpdate()
    {
        GetInputsEvent inputs = new GetInputsEvent();
        Event<GetInputsEvent>.Broadcast(inputs, gameObject);
        m_inputsDirection = inputs.direction;

        if (!UpdatePaused())
        {
            GetOffsetVelocityEvent velocityData = new GetOffsetVelocityEvent();
            Event<GetOffsetVelocityEvent>.Broadcast(velocityData, gameObject);

            UpdateDash();
            UpdateVelocity(velocityData.velocityMultiplier, velocityData.offsetVelocity);
        }

        m_oldPosition = transform.position;
        m_inputsStartDash = false;

        Event<CenterUpdatedEvent>.Broadcast(new CenterUpdatedEvent(transform.position));

        UpdateAnimation();
    }

    bool UpdatePaused()
    {
        if (Gamestate.instance.paused)
        {
            m_rigidbody.velocity = Vector3.zero;
            Vector3 pos = m_oldPosition;
            pos.z = transform.position.z;
            transform.position = pos;
            return true;
        }

        return false;
    }

    void UpdateVelocity(float multiplier, Vector2 offset)
    {
        if (m_dashState != DashState.none)
            return;

        float inputMagnitude = m_inputsDirection.magnitude;
        if (inputMagnitude < 0.1f)
            inputMagnitude = 0;
        if (inputMagnitude > 1)
            inputMagnitude = 1;

        Vector2 wantedVelocity = Vector2.zero;
        if(inputMagnitude > 0)
            wantedVelocity = m_inputsDirection.normalized * inputMagnitude * m_maxSpeed * multiplier;

        //do each axis separately
        Vector2 velocity = m_rigidbody.velocity;

        float frameAcceleration = m_acceleration * Time.deltaTime;

        if (multiplier <= 0.01f)
            velocity = Vector2.zero;
        else
        {
            if (velocity.x < wantedVelocity.x)
            {
                velocity.x += frameAcceleration;
                if (velocity.x > wantedVelocity.x)
                    velocity.x = wantedVelocity.x;
            }
            else if (velocity.x > wantedVelocity.x)
            {
                velocity.x -= frameAcceleration;
                if (velocity.x < wantedVelocity.x)
                    velocity.x = wantedVelocity.x;
            }
            if (velocity.y < wantedVelocity.y)
            {
                velocity.y += frameAcceleration;
                if (velocity.y > wantedVelocity.y)
                    velocity.y = wantedVelocity.y;
            }
            else if (velocity.y > wantedVelocity.y)
            {
                velocity.y -= frameAcceleration;
                if (velocity.y < wantedVelocity.y)
                    velocity.y = wantedVelocity.y;
            }
        }

        if(offset.sqrMagnitude > 0.1f)
        {
            if (offset.x > 0 && offset.x > velocity.x)
                velocity.x = offset.x;
            if (offset.x < 0 && offset.x < velocity.x)
                velocity.x = offset.x;

            if (offset.y > 0 && offset.y > velocity.y)
                velocity.y = offset.y;
            if (offset.y < 0 && offset.y < velocity.y)
                velocity.y = offset.y;
        }

        float velocityMagnitude = velocity.magnitude;
        if (velocityMagnitude > 0.1f)
            m_direction = velocity / velocityMagnitude;

        m_rigidbody.velocity = velocity;
    }

    void UpdateDash()
    {
        const string startName = "Dash_Start";
        const string loopName = "Dash_Loop";
        const string endName = "Dash_End";

        if(m_dashState != DashState.none)
        {
            m_dashDuration += Time.deltaTime;
            switch(m_dashState)
            {
                case DashState.start:
                    {
                        m_rigidbody.velocity = Vector2.zero;
                        if (m_dashDuration >= m_dashMaxDuration)
                        {
                            m_dashState = DashState.loop;
                            m_dashDuration = 0;
                            m_dashMaxDuration = m_dashDistance / m_dashSpeed;

                            AnimationDirection dir = AnimationDirectionEx.GetDirection(m_direction);

                            PlayAnimationEvent play = new PlayAnimationEvent(loopName, dir, 1, true);
                            Event<PlayAnimationEvent>.Broadcast(play, gameObject);
                        }
                    }
                    break;
                case DashState.loop:
                    {
                        Vector2 velocity = m_direction * m_dashSpeed;
                        m_rigidbody.velocity = velocity;

                        if (m_dashDuration >= m_dashMaxDuration)
                        {
                            m_dashState = DashState.end;
                            m_dashDuration = 0;

                            AnimationDirection dir = AnimationDirectionEx.GetDirection(m_direction);
                            PlayAnimationEvent play = new PlayAnimationEvent(endName, dir, 1, false);
                            Event<PlayAnimationEvent>.Broadcast(play, gameObject);

                            GetAnimationDurationEvent animDuration = new GetAnimationDurationEvent(endName, dir);
                            Event<GetAnimationDurationEvent>.Broadcast(animDuration, gameObject);

                            m_dashMaxDuration = animDuration.duration;
                        }
                    }
                    break;
                case DashState.end:
                    {
                        m_rigidbody.velocity = Vector2.zero;
                        if (m_dashDuration >= m_dashMaxDuration)
                        {
                            m_dashState = DashState.end;
                            m_dashDuration = 0;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        else if(m_inputsStartDash)
        {
            if (m_inputsDirection.magnitude >= 0.1f)
                m_direction = m_inputsDirection;

            if (MathF.Abs(m_direction.x) > MathF.Abs(m_direction.y))
                m_direction.y = 0;
            else m_direction.x = 0;
            m_direction /= MathF.Abs(m_direction.x + m_direction.y);

            m_dashState = DashState.start;

            AnimationDirection dir = AnimationDirectionEx.GetDirection(m_direction);
            PlayAnimationEvent play = new PlayAnimationEvent(startName, dir, 1, false);
            Event<PlayAnimationEvent>.Broadcast(play, gameObject);

            GetAnimationDurationEvent animDuration = new GetAnimationDurationEvent(startName, dir);
            Event<GetAnimationDurationEvent>.Broadcast(animDuration, gameObject);

            m_dashDuration = 0;
            m_dashMaxDuration = animDuration.duration;

            PlayAnimationEvent play2 = new PlayAnimationEvent(loopName, dir, 1, true, true);
            Event<PlayAnimationEvent>.Broadcast(play2, gameObject);
        }
    }

    void UpdateAnimation()
    {
        const string idleName = "Idle";
        const string moveName = "Move";

        GetAnimationEvent anim = new GetAnimationEvent(0, 0);
        Event<GetAnimationEvent>.Broadcast(anim, gameObject);

        string name = idleName;
        Vector2 velocity = m_rigidbody.velocity;
        if (velocity.magnitude > 0.1f)
            name = moveName;

        AnimationDirection dir = AnimationDirectionEx.GetDirection(m_direction);

        if (name == anim.name && dir == anim.direction)
            return;

        PlayAnimationEvent play = new PlayAnimationEvent(name, dir, 0, true);
        Event<PlayAnimationEvent>.Broadcast(play, gameObject);
    }

    void OnStartDash(StartDashEvent e)
    {
        GetStatusEvent status = new GetStatusEvent();
        Event<GetStatusEvent>.Broadcast(status, gameObject);
        if(!status.lockActions)
            m_inputsStartDash = true;
    }

    void OnTeleport(TeleportPlayerEvent e)
    {
        Vector3 pos = transform.position;
        pos.x = e.pos.x;
        pos.y = e.pos.y;
        transform.position = pos;
        m_rigidbody.velocity = Vector2.zero;

        Event<CenterUpdatedEventInstant>.Broadcast(new CenterUpdatedEventInstant(transform.position));
    }

    void GetStatus(GetStatusEvent e)
    {
        e.direction = m_direction;
        e.lockActions |= m_dashState != DashState.none;
        e.velocity = m_rigidbody.velocity;
        e.dashing = m_dashState != DashState.none;
    }

    void GetPlayerLife(GetPlayerLifeEvent e)
    {
        GetLifeEvent life = new GetLifeEvent();
        Event<GetLifeEvent>.Broadcast(life, gameObject);

        e.life = life.life;
        e.maxLife = life.maxLife;
    }

    void GetPlayerPos(GetPlayerPositionEvent e)
    {
        e.pos = transform.position;
    }
}


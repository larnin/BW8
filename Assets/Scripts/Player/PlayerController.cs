using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float m_maxSpeed = 1;
    [SerializeField] float m_acceleration = 2;

    [SerializeField] float m_rollSpeed = 2;
    [SerializeField] float m_rollDuration = 1;

    Rigidbody2D m_rigidbody = null;

    SubscriberList m_subscriberList = new SubscriberList();

    Vector2 m_inputsDirection;
    bool m_inputsStartRoll;

    Vector2 m_oldPosition;
    Vector2 m_direction;
    bool m_rolling;
    float m_rollingDuration;


    private void Awake()
    {
        m_subscriberList.Add(new Event<StartRollEvent>.LocalSubscriber(OnStartRoll, gameObject));
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
        m_rigidbody = GetComponent<Rigidbody2D>();

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

            if (velocityData.overrideVelocity)
                m_rigidbody.velocity = velocityData.offsetVelocity;
            else
            {
                UpdateRoll();
                UpdateVelocity();
            }
        }

        m_oldPosition = transform.position;
        m_inputsStartRoll = false;

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

    void UpdateVelocity()
    {
        if (m_rolling)
            return;

        float inputMagnitude = m_inputsDirection.magnitude;
        if (inputMagnitude < 0.1f)
            inputMagnitude = 0;
        if (inputMagnitude > 1)
            inputMagnitude = 1;

        Vector2 wantedVelocity = Vector2.zero;
        if(inputMagnitude > 0)
            wantedVelocity = m_inputsDirection.normalized * inputMagnitude * m_maxSpeed;

        //do each axis separately
        Vector2 velocity = m_rigidbody.velocity;

        float frameAcceleration = m_acceleration * Time.deltaTime;

        if(velocity.x < wantedVelocity.x)
        {
            velocity.x += frameAcceleration;
            if (velocity.x > wantedVelocity.x)
                velocity.x = wantedVelocity.x;
        }
        else if(velocity.x > wantedVelocity.x)
        {
            velocity.x -= frameAcceleration;
            if (velocity.x < wantedVelocity.x)
                velocity.x = wantedVelocity.x;
        }    
        if(velocity.y < wantedVelocity.y)
        {
            velocity.y += frameAcceleration;
            if (velocity.y > wantedVelocity.y)
                velocity.y = wantedVelocity.y;
        }
        else if( velocity.y > wantedVelocity.y)
        {
            velocity.y -= frameAcceleration;
            if (velocity.y < wantedVelocity.y)
                velocity.y = wantedVelocity.y;
        }

        float velocityMagnitude = velocity.magnitude;
        if (velocityMagnitude > 0.1f)
            m_direction = velocity / velocityMagnitude;

        m_rigidbody.velocity = velocity;
    }

    void UpdateRoll()
    {
        if(m_rolling)
        {
            Vector2 velocity = m_direction * m_rollSpeed;
            m_rigidbody.velocity = velocity;

            m_rollingDuration += Time.deltaTime;
            if (m_rollingDuration >= m_rollDuration)
                m_rolling = false;
        }
        else if(m_inputsStartRoll)
        {
            if (m_inputsDirection.magnitude >= 0.1f)
                m_direction = m_inputsDirection;

            //make direction axis aligned
            if (MathF.Abs(m_direction.x) > MathF.Abs(m_direction.y))
                m_direction.y = 0;
            else m_direction.x = 0;
            m_direction /= MathF.Abs(m_direction.x + m_direction.y);

            m_rolling = true;
            m_rollingDuration = 0;

            AnimationDirection dir = AnimationDirectionEx.GetDirection(m_direction);
            PlayAnimationEvent play = new PlayAnimationEvent("Roll", dir, 1, false);
            Event<PlayAnimationEvent>.Broadcast(play, gameObject);
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

    void OnStartRoll(StartRollEvent e)
    {
        m_inputsStartRoll = true;
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
        e.rolling = m_rolling;
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


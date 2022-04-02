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
    Animator m_animator = null;

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

        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();

        m_oldPosition = transform.position;
        m_direction = new Vector2(0, -1);

        Event<CenterUpdatedEventInstant>.Broadcast(new CenterUpdatedEventInstant(transform.position));
    }

    private void FixedUpdate()
    {
        GetInputsEvent inputs = new GetInputsEvent();
        Event<GetInputsEvent>.Broadcast(inputs, gameObject);
        m_inputsDirection = inputs.direction;

        if (!UpdatePaused())
        {
            UpdateRoll();
            UpdateVelocity();
        }

        m_oldPosition = transform.position;
        m_inputsStartRoll = false;

        Event<CenterUpdatedEvent>.Broadcast(new CenterUpdatedEvent(transform.position));
    }

    bool UpdatePaused()
    {
        if (Gamestate.instance.paused)
        {
            m_rigidbody.velocity = Vector3.zero;
            transform.position = m_oldPosition;
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
            //make direction axis aligned
            if (MathF.Abs(m_direction.x) > MathF.Abs(m_direction.y))
                m_direction.y = 0;
            else m_direction.x = 0;
            m_direction /= MathF.Abs(m_direction.x + m_direction.y);

            m_rolling = true;
            m_rollingDuration = 0;
        }
    }

    void OnStartRoll(StartRollEvent e)
    {
        m_inputsStartRoll = true;
    }
}


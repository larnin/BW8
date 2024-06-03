using UnityEngine;

public class MoveJumpController : MonoBehaviour
{
    enum TargetType
    {
        none,
        position,
        entity,
    }

    enum State
    {
        idle,
        start,
        jump,
        end,
    }

    [SerializeField] string m_idleAnim = "Idle";
    [SerializeField] string m_startJumpAnim = "StartJump";
    [SerializeField] string m_jumpingAnim = "Jumping";
    [SerializeField] string m_endJumpAnim = "EndJump";
    [SerializeField] float m_jumpDistance = 2;
    [SerializeField] float m_jumpSpeed = 2;
    [SerializeField] float m_jumpHeight = 1;
    [SerializeField] float m_delayBetweenJump = 1;
    [SerializeField] bool m_endMoveAfterEachJump = false;
    [SerializeField] GameObject m_localRender;
    [SerializeField] GameObject m_instantiateEndJump;
    Rigidbody2D m_rigidbody;

    State m_state = State.idle;
    float m_duration = 0;
    Vector2 m_startJump = Vector2.zero;
    Vector2 m_endJump = Vector2.zero;
    Vector2 m_oldPos = Vector2.zero;

    TargetType m_targetType = TargetType.none;
    Vector3 m_targetPos;
    GameObject m_target;

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<StartMoveEvent>.LocalSubscriber(StartMove, gameObject));
        m_subscriberList.Add(new Event<StartFollowEvent>.LocalSubscriber(StartFollow, gameObject));
        m_subscriberList.Add(new Event<StopMoveEvent>.LocalSubscriber(StopMove, gameObject));
        m_subscriberList.Add(new Event<IsMovingEvent>.LocalSubscriber(IsMoving, gameObject));

        m_subscriberList.Subscribe();

        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void StartMove(StartMoveEvent e)
    {
        m_targetType = TargetType.position;
        m_targetPos = e.target;
        m_target = null;

        if (m_state == State.idle)
            m_duration = 0;
    }

    void StartFollow(StartFollowEvent e)
    {
        m_targetType = TargetType.entity;
        m_targetPos = Vector3.zero;
        m_target = e.target;

        if (m_state == State.idle)
            m_duration = 0;
    }

    void StopMove(StopMoveEvent e)
    {
        m_targetType = TargetType.none;
        m_targetPos = Vector3.zero;
        m_target = null;
    }

    void IsMoving(IsMovingEvent e)
    {
        e.isMoving = m_targetType != TargetType.none;
    }

    Vector3 GetTargetPos()
    {
        if (m_targetType == TargetType.entity)
        {
            if (m_target != null)
                return m_target.transform.position;
        }
        if (m_targetType == TargetType.position)
            return m_targetPos;
        return transform.position;
    }

    float GetColliderRadius()
    {
        var col = GetComponentInChildren<Collider2D>();
        if (col is CircleCollider2D)
            return (col as CircleCollider2D).radius;
        var bounds = col.bounds;
        return bounds.size.magnitude;
    }

    private void FixedUpdate()
    {
        if (Gamestate.instance.paused)
        {
            m_rigidbody.velocity = Vector3.zero;
            Vector3 pos = m_oldPos;
            pos.z = transform.position.z;
            transform.position = pos;
            return;
        }

        m_duration -= Time.deltaTime;

        if(m_state != State.jump)
        {
            m_rigidbody.velocity = Vector2.zero;

            GetOffsetVelocityEvent velocityData = new GetOffsetVelocityEvent();
            Event<GetOffsetVelocityEvent>.Broadcast(velocityData, gameObject);
            if (velocityData.velocityMultiplier < 0.5f)
                m_rigidbody.velocity = velocityData.offsetVelocity;
        }

        switch (m_state)
        {
            case State.idle:
                {
                    if (m_duration <= 0)
                        StartJump();
                    
                    break;
                }
            case State.start:
                {
                    if (m_duration <= 0)
                        RealStartJump();
                    break;
                }
            case State.jump:
                {
                    Jumping();
                    break;
                }
            case State.end:
                {
                    if (m_duration <= 0)
                        RealEndJump();
                    break;
                }
            default:
                break;
        }

        m_oldPos = transform.position;
    }

    void StartJump()
    {
        if (m_targetType == TargetType.none)
            return;

        m_startJump = transform.position;

        Vector2 targetPos = GetTargetPos();

        Vector2 dir = (targetPos - m_startJump).normalized;
        m_endJump = m_startJump + dir * m_jumpDistance;

        var commonData = World.common;

        var radius = GetColliderRadius();
        var hit = Physics2D.CircleCast(m_startJump, radius, dir, m_jumpDistance, commonData.groundLayer.value);
        if(hit.collider != null)
            m_endJump = hit.centroid;

        float dist = (m_endJump - m_startJump).magnitude;
        if(dist < m_jumpDistance / 5)
        {
            m_duration = 0.2f;
            return;
        }

        m_state = State.start;

        EnableCollisions(false);

        PlayAnimationEvent play = new PlayAnimationEvent(m_startJumpAnim, AnimationDirection.none, 0, false);
        Event<PlayAnimationEvent>.Broadcast(play, gameObject);

        GetAnimationDurationEvent duration = new GetAnimationDurationEvent(m_startJumpAnim, AnimationDirection.none);
        Event<GetAnimationDurationEvent>.Broadcast(duration, gameObject);
        m_duration = duration.duration;
    }

    void RealStartJump()
    {
        PlayAnimationEvent play = new PlayAnimationEvent(m_jumpingAnim, AnimationDirection.none, 0, true, true);
        Event<PlayAnimationEvent>.Broadcast(play, gameObject);

        m_state = State.jump;
    }

    void Jumping()
    {
        Vector2 pos = transform.position;
        bool end = false;

        float remainingDistance = (m_endJump - pos).magnitude;
        float totalDistance = (m_endJump - m_startJump).magnitude;

        if (totalDistance <= 0.01f)
        {
            EndJump();
            return;
        }

        float move = m_jumpSpeed * Time.deltaTime;
        if (move >= remainingDistance)
        {
            end = true;
            remainingDistance = 0;
        }
        else remainingDistance -= move;

        float normalizedRemaining = remainingDistance / totalDistance;

        pos = m_endJump * (1 - normalizedRemaining) + m_startJump * (normalizedRemaining);

        Vector3 pos3 = pos;
        pos3.z = transform.position.z;
        m_rigidbody.MovePosition(pos3);

        if (m_localRender != null)
        {
            //offset of the render
            //y = 1 - (2*x-1)^2
            float param = 2 * normalizedRemaining - 1;
            float height = (1 - param * param) * m_jumpHeight;

            pos3 = m_localRender.transform.localPosition;
            pos3.y = height;
            m_localRender.transform.localPosition = pos3;
        }

        if (end)
            EndJump();
    }

    void EndJump()
    {
        if (m_localRender != null)
            m_localRender.transform.localPosition = Vector3.zero;

        EnableCollisions(true);

        m_state = State.end;

        PlayAnimationEvent play = new PlayAnimationEvent(m_endJumpAnim, AnimationDirection.none, 0, false);
        Event<PlayAnimationEvent>.Broadcast(play, gameObject);

        GetAnimationDurationEvent duration = new GetAnimationDurationEvent(m_endJumpAnim, AnimationDirection.none);
        Event<GetAnimationDurationEvent>.Broadcast(duration, gameObject);
        m_duration = duration.duration;

        if(m_instantiateEndJump != null)
        {
            var obj = Instantiate(m_instantiateEndJump);
            obj.transform.position = transform.position;
        }
    }

    void RealEndJump()
    {
        PlayAnimationEvent play = new PlayAnimationEvent(m_idleAnim, AnimationDirection.none, 0, true, true);
        Event<PlayAnimationEvent>.Broadcast(play, gameObject);

        m_duration = m_delayBetweenJump;

        if (m_endMoveAfterEachJump)
        {
            m_targetType = TargetType.none;
            m_target = null;
            m_targetPos = Vector3.zero;
        }

        m_state = State.idle;
    }

    void EnableCollisions(bool enable)
    {
        var cols = GetComponentsInChildren<Collider2D>();
        foreach (var c in cols)
            c.enabled = enable;
    }
}

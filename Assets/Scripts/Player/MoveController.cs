using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    enum TargetType
    {
        none,
        position,
        entity,
    }

    [SerializeField] string m_idleAnim = "Idle";
    [SerializeField] bool m_idleUseDirections = true;
    [SerializeField] bool m_seeDownWhenIdle = false;
    [SerializeField] [ShowIf("m_seeDownWhenIdle")] float m_seeDownAfterDelay = 0;
    [SerializeField] string m_moveAnim = "Move";
    [SerializeField] bool m_moveUseDirection = true;
    [SerializeField] float m_moveSpeed = 1;
    [SerializeField] float m_turnSpeed = 10;
    [SerializeField] float m_targetStopMoveDistance = 1;
    [SerializeField] float m_targetStartMoveDistance = 2;
    [SerializeField] float m_acceleration = 5;

    SubscriberList m_subscriberList = new SubscriberList();

    bool m_moving = false;
    TargetType m_targetType = TargetType.none;
    Vector3 m_targetPos;
    GameObject m_target;

    float m_movingSpeed = 0;
    float m_movingDir = 0;

    Rigidbody2D m_rigidbody;

    float m_idleTimer = 0;
    bool m_oldMovingAnimation;
    AnimationDirection m_oldDirAnimation = AnimationDirection.none;

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

    private void Update()
    {
        if(m_targetType == TargetType.entity && m_target == null)
            m_targetType = TargetType.none;
        if (m_targetType == TargetType.none)
            m_moving = false;

        Vector3 target = GetTargetPos();
        Vector3 dir = target - transform.position;
        float dist = dir.magnitude;
        if (m_moving && dist < m_targetStopMoveDistance && dist < 0.01f)
            m_moving = false;
        else if (!m_moving && dist > m_targetStartMoveDistance)
            m_moving = true;

        if (m_moving)
            ProcessMove(target);
        else ProcessStop();

        UpdateAnimations();
    }

    void ProcessMove(Vector3 target)
    {
        Vector2 dir = target - transform.position;

        float targetAngle = Mathf.Atan2(dir.y, dir.x);
        if (m_movingSpeed < 0.01f)
            m_movingDir = targetAngle;
        else
        {
            float angleOffset = targetAngle - m_movingDir;
            while (angleOffset < -Mathf.PI)
                angleOffset += 2 * Mathf.PI;
            while (angleOffset > Mathf.PI)
                angleOffset -= 2 * Mathf.PI;

            float moveOffset = Mathf.Sign(angleOffset) * Time.deltaTime * m_turnSpeed;
            if (Mathf.Abs(moveOffset) > Mathf.Abs(angleOffset))
                moveOffset = angleOffset;
            m_movingDir += moveOffset;
        }

        m_movingSpeed += Time.deltaTime * m_acceleration;
        if (m_movingSpeed > m_moveSpeed)
            m_movingSpeed = m_moveSpeed;

        if (m_movingSpeed < 0.01)
            return;

        ApplyVelocity();
    }

    void ProcessStop()
    {
        if(m_movingSpeed < 0.01f)
        {
            m_movingSpeed = 0;
            return;
        }

        m_movingSpeed -= Time.deltaTime * m_acceleration;
        if (m_movingSpeed < 0)
            m_movingSpeed = 0;

        ApplyVelocity();
    }

    void ApplyVelocity()
    {
        Vector2 velocity = new Vector2(m_movingSpeed * Mathf.Cos(m_movingDir), m_movingSpeed * Mathf.Sin(m_movingDir));
        if (m_rigidbody != null)
            m_rigidbody.velocity = velocity;
        else
        {
            velocity *= Time.deltaTime;
            Vector3 newPos = transform.position + new Vector3(velocity.x, velocity.y, 0);
            transform.position = newPos;
        }
    }

    void UpdateAnimations()
    {
        bool moving = m_movingSpeed >= 0.1f;

        if (!moving)
        {
            float oldTimer = m_idleTimer;
            m_idleTimer += Time.deltaTime;
            if (oldTimer < m_seeDownAfterDelay && m_idleTimer >= m_seeDownAfterDelay && m_seeDownWhenIdle)
            {
                Vector2 seeDir = AnimationDirectionEx.GetDirection(AnimationDirection.Down);
                m_movingDir = Mathf.Atan2(seeDir.y, seeDir.x);
            }
        }
        else m_idleTimer = 0;

        Vector2 dir = new Vector2(Mathf.Cos(m_movingDir), Mathf.Sin(m_movingDir));
        var animDir = AnimationDirectionEx.GetDirection(dir);

        if(animDir != m_oldDirAnimation || moving != m_oldMovingAnimation || m_oldDirAnimation == AnimationDirection.none)
        {
            string animName = moving ? m_moveAnim : m_idleAnim;

            PlayAnimationEvent e = new PlayAnimationEvent(animName, true);
            if (moving && m_moveUseDirection)
                e.direction = animDir;
            else if (!moving && m_idleUseDirections)
                e.direction = animDir;
            e.layer = 0;

            Event<PlayAnimationEvent>.Broadcast(e, gameObject);
        }

        m_oldDirAnimation = animDir;
        m_oldMovingAnimation = moving;
    }

    void StartMove()
    {
        var target = GetTargetPos();
        Vector2 dir = target - transform.position;
        float dist = dir.sqrMagnitude;

        if (dist > m_targetStopMoveDistance * m_targetStopMoveDistance)
            m_moving = true;
    }

    void StartMove(StartMoveEvent e)
    {
        m_targetType = TargetType.position;
        m_targetPos = e.target;
        StartMove();
    }

    void StartFollow(StartFollowEvent e)
    {
        m_targetType = TargetType.entity;
        m_target = e.target;
        StartMove();
    }

    void StopMove(StopMoveEvent e)
    {
        m_targetType = TargetType.none;
        m_moving = false;
    }

    void IsMoving(IsMovingEvent e)
    {
        e.isMoving = m_moving;
    }

    Vector3 GetTargetPos()
    {
        if(m_targetType == TargetType.entity)
        {
            if (m_target != null)
                return m_target.transform.position;
        }
        if (m_targetType == TargetType.position)
            return m_targetPos;
        return transform.position;
    }
}
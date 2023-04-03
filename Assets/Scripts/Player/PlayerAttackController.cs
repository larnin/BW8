using System.Collections;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] float m_attackDistance = 1;
    [SerializeField] float m_attackDelay = 0.1f;
    [SerializeField] float m_attackRadius = 0.5f;
    [SerializeField] float m_attackDuration = 0.3f;
    [SerializeField] float m_attackCooldown = 0.5f;
    [SerializeField] int m_damage = 1;
    [SerializeField] float m_knockback = 1;

    SubscriberList m_subscriberList = new SubscriberList();

    float m_duration = 0;
    bool m_haveAttacked = false;
    bool m_waiting = true;
    Vector2 m_direction = Vector2.zero;

    private void Awake()
    {
        m_subscriberList.Add(new Event<GetOffsetVelocityEvent>.LocalSubscriber(GetVelocity, gameObject));
        m_subscriberList.Add(new Event<StartUseWeaponEvent>.LocalSubscriber(Use, gameObject));

        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void GetVelocity(GetOffsetVelocityEvent e)
    {
        if(!m_waiting && m_duration < m_attackDuration)
        {
            e.offsetVelocity = Vector2.zero;
            e.velocityMultiplier = 0;
        }
    }

    void Use(StartUseWeaponEvent e)
    {
        if (!m_waiting)
            return;

        GetStatusEvent status = new GetStatusEvent();
        Event<GetStatusEvent>.Broadcast(status, gameObject);
        if (status.rolling)
            return;

        m_waiting = false;
        m_haveAttacked = false;
        m_duration = 0;

        AnimationDirection dir = AnimationDirectionEx.GetDirection(status.direction);
        m_direction = AnimationDirectionEx.GetDirection(dir);

        Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent("Attack", dir, 2), gameObject);
    }

    private void Update()
    {
        if (m_waiting)
            return;

        m_duration += Time.deltaTime;

        if(m_duration >= m_attackDelay && !m_haveAttacked)
        {
            m_haveAttacked = true;

            Vector2 pos = transform.position;
            pos += m_direction * m_attackDistance;

            var cols = Physics2D.OverlapCircleAll(pos, m_attackRadius);

            foreach(var col in cols)
            {
                if (col.gameObject.layer == gameObject.layer)
                    continue;

                Event<HitEvent>.Broadcast(new HitEvent(m_damage, gameObject, m_knockback), col.gameObject);
            }
        }

        if(m_duration >= m_attackCooldown)
        {
            m_waiting = true;
            m_duration = 0;
            m_haveAttacked = false;
        }
    }
}
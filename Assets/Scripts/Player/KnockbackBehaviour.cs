using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class KnockbackBehaviour : MonoBehaviour
{
    [SerializeField] float m_knockbackMultiplier = 1;
    [SerializeField] float m_knockbackDuration = 0.3f;

    SubscriberList m_subscriberList = new SubscriberList();

    float m_duration = 0;
    Vector2 m_velocity = Vector2.zero;

    private void Awake()
    {
        m_subscriberList.Add(new Event<LifeLossEvent>.LocalSubscriber(Damage, gameObject));
        m_subscriberList.Add(new Event<GetOffsetVelocityEvent>.LocalSubscriber(GetVelocity, gameObject));

        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void Damage(LifeLossEvent e)
    {
        float multiplier = e.knockback * m_knockbackMultiplier;
        m_velocity = (transform.position - e.caster.transform.position).normalized * multiplier;
        m_duration = m_knockbackDuration;
    }

    void GetVelocity(GetOffsetVelocityEvent e)
    {
        if (m_duration > 0)
        {
            e.offsetVelocity = m_velocity;
            e.overrideVelocity = true;
        }
    }

    private void Update()
    {
        m_duration -= Time.deltaTime;
    }
}


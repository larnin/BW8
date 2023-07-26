using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AttractBehaviour : MonoBehaviour
{
    [SerializeField] bool m_destroyWhenCatch = false;
    [SerializeField] float m_shakeDuration = 0.5f;
    [SerializeField] float m_attractBaseSpeed = 5;

    SubscriberList m_subscriberList = new SubscriberList();

    bool m_attract = false;
    Vector3 m_initialPosition;
    GameObject m_target;
    float m_shakeTimer;
    float m_speedMultiplier;

    private void Awake()
    {
        m_subscriberList.Add(new Event<CanBeAttractedEvent>.LocalSubscriber(CanBeAttracted, gameObject));
        m_subscriberList.Add(new Event<IsDestroyedWhenCatchEvent>.LocalSubscriber(IsDestroyedWhenCatch, gameObject));
        m_subscriberList.Add(new Event<StartAttractEvent>.LocalSubscriber(StartAttract, gameObject));
        m_subscriberList.Add(new Event<StopAttractEvent>.LocalSubscriber(StopAttract, gameObject));

        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void CanBeAttracted(CanBeAttractedEvent e)
    {
        e.canBeAttracted = true;
    }

    void IsDestroyedWhenCatch(IsDestroyedWhenCatchEvent e)
    {
        e.destroyedWhenCatch = m_destroyWhenCatch;
    }

    void StartAttract(StartAttractEvent e)
    {
        if (m_attract)
            StopAttract(null);

        m_attract = true;
        m_target = e.target;
        m_speedMultiplier = e.speedMultiplier;
        m_shakeTimer = 0;
        m_initialPosition = transform.position;

        Event<SetExclusiveBehaviourEvent>.Broadcast(new SetExclusiveBehaviourEvent(this), gameObject);
    }

    void StopAttract(StopAttractEvent e)
    {
        m_attract = false;

        if (m_shakeTimer < m_shakeDuration)
            transform.position = m_initialPosition;

        CanBeThrownEvent throwEvent = new CanBeThrownEvent();
        Event<CanBeThrownEvent>.Broadcast(throwEvent, gameObject);
        if (!throwEvent.canBeThrown)
        {
            Event<SetDefaultBehaviourEvent>.Broadcast(new SetDefaultBehaviourEvent(), gameObject);
            return;
        }

        SetProjectileDataEvent data = new SetProjectileDataEvent();
        data.caster = null;
        data.damages = 0;
        data.hitLayer = 0;
        data.knockback = 0;
        data.maxDistance = 0;
        data.speed = 0;
        Event<SetProjectileDataEvent>.Broadcast(data, gameObject);
        Event<ThrowEvent>.Broadcast(new ThrowEvent(), gameObject);
    }

    private void Update()
    {
        float duration = m_shakeDuration / m_speedMultiplier;
        if(m_shakeTimer < duration)
        {
            m_shakeTimer += Time.deltaTime;
            if (m_shakeTimer >= duration)
                transform.position = m_initialPosition;
            else
            {
                int cycle = Mathf.FloorToInt(m_shakeTimer * World.vacuum.m_shakeFrequency);
                float absTIme = (m_shakeTimer - (cycle / World.vacuum.m_shakeFrequency)) * World.vacuum.m_shakeFrequency;
                float xOffset = (absTIme - 0.5f) * (cycle % 2 == 0 ? 1 : -1) * World.vacuum.m_shakeAmplitide;
                transform.position = m_initialPosition + new Vector3(xOffset, 0, 0);
            }
        }
        else
        {
            if(m_target == null)
            {
                StopAttract(null);
                return;
            }

            Vector3 newPos = transform.position;
            Vector3 dir = m_target.transform.position - newPos;
            dir.z = 0;
            float dist = dir.magnitude;
            if (dist <= 0.0001f)
                return;
            dir /= dist;
            float multiplier = m_attractBaseSpeed * m_speedMultiplier * Time.deltaTime;
            if (multiplier > dist)
                multiplier = dist;
            dir *= multiplier;
            newPos += dir;

            transform.position = newPos;
        }
    }
}

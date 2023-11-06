using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ExclusiveBehaviour : MonoBehaviour
{
    [SerializeField] MonoBehaviour m_defaultBehaviour;
    [SerializeField] MonoBehaviour[] m_behaviours;

    SubscriberList m_subscriberList = new SubscriberList();

    MonoBehaviour m_currentBehaviour = null;

    private void Awake()
    {
        m_subscriberList.Add(new Event<SetExclusiveBehaviourEvent>.LocalSubscriber(SetExclusive, gameObject));
        m_subscriberList.Add(new Event<GetExclusiveBehaviourEvent>.LocalSubscriber(GetExclusive, gameObject));
        m_subscriberList.Add(new Event<SetDefaultBehaviourEvent>.LocalSubscriber(SetDefault, gameObject));
        m_subscriberList.Add(new Event<GetDefaultBehaviourEvent>.LocalSubscriber(GetDefault, gameObject));
        m_subscriberList.Subscribe();

        if(m_behaviours != null)
        {
            foreach(var b in m_behaviours)
            {
                if (b != null)
                    b.enabled = false;
            }
        }

        m_currentBehaviour = m_defaultBehaviour;

        if (m_currentBehaviour != null)
            m_currentBehaviour.enabled = true;
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void SetExclusive(SetExclusiveBehaviourEvent e)
    {
        SetExclusive(e.behaviour);
    }

    void SetExclusive(MonoBehaviour b)
    {
        if (m_currentBehaviour != null)
            m_currentBehaviour.enabled = false;

        m_currentBehaviour = b;

        if (m_currentBehaviour != null)
            m_currentBehaviour.enabled = true;
    }

    void GetExclusive(GetExclusiveBehaviourEvent e)
    {
        e.behaviour = m_currentBehaviour;
    }

    void SetDefault(SetDefaultBehaviourEvent e)
    {
        SetExclusive(m_defaultBehaviour);
    }

    void GetDefault(GetDefaultBehaviourEvent e)
    {
        e.defaultBehaviour = m_defaultBehaviour;
    }

    private void LateUpdate()
    {
        Event<ExclusiveLateUpdate>.Broadcast(new ExclusiveLateUpdate());
    }
}

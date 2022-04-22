using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class InstanciateOnDeathBehaviour : MonoBehaviour
{
    [SerializeField] GameObject m_prefab = null;
    [SerializeField] float m_destroyDelay = 0;
    [SerializeField] Vector2 m_offset = Vector2.zero;

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<DeathEvent>.LocalSubscriber(OnDeath, gameObject));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnDeath(DeathEvent e)
    {
        Vector3 pos = m_offset;
        pos += transform.position;

        var obj = Instantiate(m_prefab);
        obj.transform.position = pos;
        Destroy(gameObject, m_destroyDelay);
    }
}

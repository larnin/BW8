using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LootAtDeath : MonoBehaviour
{
    [SerializeField] LootList m_lootList;

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
        if (m_lootList == null)
            return;

        var loots = m_lootList.GenerateLoots();

        var pos = transform.position;
        foreach (var l in loots)
            l.transform.position = pos;
    }
}
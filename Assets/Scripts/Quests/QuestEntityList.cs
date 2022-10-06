using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class QuestEntityList : MonoBehaviour
{

    SubscriberList m_subscriberList = new SubscriberList();

    Dictionary<string, List<QuestEntity>> m_entities = new Dictionary<string, List<QuestEntity>>();

    private void Awake()
    {
        m_subscriberList.Add(new Event<RegisterQuestEntityEvent>.Subscriber(OnEntityRegister));
        m_subscriberList.Add(new Event<UnregisterQuestEntityEvent>.Subscriber(OnEntityUnregister));
        m_subscriberList.Add(new Event<GetFirstQuestEntityEvent>.Subscriber(GetEntity));
        m_subscriberList.Add(new Event<GetAllQuestEntityEvent>.Subscriber(GetEntities));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnEntityRegister(RegisterQuestEntityEvent e)
    {
        List<QuestEntity> list;
        m_entities.TryGetValue(e.id, out list);

        if (list == null)
        {
            list = new List<QuestEntity>();
            m_entities.Add(e.id, list);
        }

        if (list.Contains(e.entity))
            return;
        list.Add(e.entity);
    }

    void OnEntityUnregister(UnregisterQuestEntityEvent e)
    {
        List<QuestEntity> list;
        m_entities.TryGetValue(e.id, out list);

        if (list != null)
            list.Remove(e.entity);
    }

    void GetEntity(GetFirstQuestEntityEvent e)
    {
        List<QuestEntity> list;
        m_entities.TryGetValue(e.id, out list);

        if (list == null)
            return;

        if (list.Count == 0)
            return;

        e.entity = list[0];
    }

    void GetEntities(GetAllQuestEntityEvent e)
    {
        List<QuestEntity> list;
        m_entities.TryGetValue(e.id, out list);

        if (list == null)
            return;

        foreach (var entity in list)
            e.entities.Add(entity);
    }

}

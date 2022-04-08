using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NpcList : MonoBehaviour
{
    List<GameObject> m_entities = new List<GameObject>();

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<AddEntityEvent>.Subscriber(AddEntity));
        m_subscriberList.Add(new Event<RemoveEntityEvent>.Subscriber(RemoveEntity));
        m_subscriberList.Add(new Event<RemoveAllEntityEvent>.Subscriber(RemoveAllEntity));

        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    private void Update()
    {
        List<GameObject> nextFrameObjects = new List<GameObject>();

        GetLoadedChunksEvent e = new GetLoadedChunksEvent();
        Event<GetLoadedChunksEvent>.Broadcast(e);

        foreach(var entity in m_entities)
        {
            if (entity == null)
                continue;

            Vector2 pos = entity.transform.position;

            bool isInZone = false;
            foreach(var c in e.chunks)
            {
                if (c.Contains(pos))
                    isInZone = true;
            }

            if (!isInZone)
                Destroy(entity);
            else nextFrameObjects.Add(entity);
        }

        m_entities = nextFrameObjects;
    }

    void AddEntity(AddEntityEvent e)
    {
        foreach (var entity in m_entities)
            if (entity == e.entity)
                return;

        m_entities.Add(e.entity);
    }

    void RemoveEntity(RemoveEntityEvent e)
    {
        for(int i = 0; i < m_entities.Count; i++)
        {
            if(m_entities[i] == e.entity)
            {
                Destroy(m_entities[i]);
                m_entities.RemoveAt(i);
                break;
            }
        }
    }

    void RemoveAllEntity(RemoveAllEntityEvent e)
    {
        for (int i = 0; i < m_entities.Count; i++)
            Destroy(m_entities[i]);

        m_entities.Clear();
    }
}

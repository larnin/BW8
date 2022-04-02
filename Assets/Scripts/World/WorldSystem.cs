using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Chunk
{
    public Vector2Int pos;
    public GameObject instance;

    public Chunk(Vector2Int _pos, GameObject _instance)
    {
        pos = _pos;
        instance = _instance;
    }
}

public class WorldSystem : MonoBehaviour
{
    [SerializeField] WorldObject m_world = null;
    [SerializeField] int m_drawSize = 1;

    SubscriberList m_subscriberList = new SubscriberList();

    Vector2Int m_currentChunk = new Vector2Int(-100000, -100000);

    List<Chunk> m_loadedChunks = new List<Chunk>();

    private void Awake()
    {
        m_subscriberList.Add(new Event<SetWorldEvent>.Subscriber(SetWorld));

        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void SetWorld(SetWorldEvent e)
    {
        if (m_world == e.world)
            return;

        m_world = e.world;

        UpdateWorld(true);
    }

    void UpdateWorld(bool destroyOld)
    {
        //destroy too far chunks
        if (destroyOld)
        {
            foreach(var c in m_loadedChunks)
                Destroy(c.instance);
            m_loadedChunks.Clear();
        }
        else
        {
            List<int> toDestroy = new List<int>();
            for (int i = 0; i < m_loadedChunks.Count; i++)
            {
                var c = m_loadedChunks[i];
                int maxOffset = Mathf.Max(Mathf.Abs(c.pos.x - m_currentChunk.x), Mathf.Abs(c.pos.y - m_currentChunk.y));

                if (maxOffset > m_drawSize)
                    toDestroy.Add(i);
            }

            for (int i = toDestroy.Count - 1; i >= 0; i--)
            {
                var c = m_loadedChunks[toDestroy[i]];
                Destroy(c.instance);
                m_loadedChunks.RemoveAt(toDestroy[i]);
            }
        }

        // add new chunks
        Vector2Int min = m_currentChunk - new Vector2Int(m_drawSize, m_drawSize);
        Vector2Int max = m_currentChunk + new Vector2Int(m_drawSize, m_drawSize);

        for(int i = min.x; i <= max.x; i++)
        {
            for(int j = min.y; j < max.y; j++)
            {
                bool found = false;
                foreach(var c in m_loadedChunks)
                {
                    if(c.pos.x == i && c.pos.y == j)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                    continue;

                InstantiateNewChunk(new Vector2Int(i, j));
            }
        }
    }

    void InstantiateNewChunk(Vector2Int pos)
    {
        if (m_world == null)
            return;

        var chunk = m_world.GetUnclampedChunk(pos);
        if (chunk == null)
            return;

        var instance = Instantiate(chunk, transform);
        Vector3 position = transform.position;
        position.x = pos.x * m_world.chunkSize.x;
        position.y = pos.y * m_world.chunkSize.y;

        instance.transform.position = position;

        m_loadedChunks.Add(new Chunk(pos, instance));
    }

    void OnCenterUpdate(Vector2 pos)
    {
        var newChunk = PosToChunk(pos);

        if(newChunk.x != m_currentChunk.x && newChunk.y != m_currentChunk.y)
        {
            m_currentChunk = newChunk;
            UpdateWorld(false);
        }
    }

    void OnCenterUpdate(CenterUpdatedEvent e)
    {
        OnCenterUpdate(e.pos);
    }

    void OnCenterInstantUpdate(CenterUpdatedEventInstant e)
    {
        OnCenterUpdate(e.pos);
    }

    Vector2Int PosToChunk(Vector2 pos)
    {
        if (m_world == null)
            return Vector2Int.zero;

        var size = m_world.chunkSize;

        pos.x /= size.x;
        pos.y /= size.y;

        return new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
    }
}
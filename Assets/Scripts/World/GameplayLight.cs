using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class GameplayLightData
{
    public Vector2 center;
    public float radius;

    public GameplayLightData(Vector2 _center, float _radius)
    {
        center = _center;
        radius = _radius;
    }
}

public class GameplayLight : MonoBehaviour
{
    [SerializeField] float m_radius;
    int m_ID = -1;

    SubscriberList m_subscriberList = new SubscriberList();

    static Dictionary<int, GameplayLightData> m_lights = new Dictionary<int, GameplayLightData>();
    static int m_nextID = 0;

    private void OnDrawGizmosSelected()
    {
        DebugDraw.Circle2D(transform.position, m_radius, Color.red);
    }

    private void OnEnable()
    {
        if (m_ID == -1)
            ReserveID();
    }

    private void Update()
    {
        UpdateLight(m_ID, transform.position, m_radius);
    }

    private void Awake()
    {
        m_subscriberList.Add(new Event<SetGameplayLightRadiusEvent>.LocalSubscriber(SetRadius, gameObject));

        m_subscriberList.Subscribe();
    }

    private void OnDisable()
    {
        RemoveLight(m_ID);
    }

    private void OnDestroy()
    {
        RemoveLight(m_ID);

        m_subscriberList.Unsubscribe();
    }

    void SetRadius(SetGameplayLightRadiusEvent e)
    {
        m_radius = e.radius;
    }

    static int ReserveID()
    {
        return m_nextID++;
    }

    static void UpdateLight(int id, Vector2 pos, float radius)
    {
        if (!m_lights.ContainsKey(id))
            m_lights.Add(id, new GameplayLightData(pos, radius));
        else
        {
            var data = m_lights[id];
            data.center = pos;
            data.radius = radius;
        }
    }

    static void RemoveLight(int id)
    {
        m_lights.Remove(id);
    }

    static bool IsOnLight(Vector2 pos)
    {
        foreach(var l in m_lights)
        {
            float radius = l.Value.radius;
            float dist = (pos - l.Value.center).sqrMagnitude;
            if (dist < radius * radius)
                return true;
        }
        return false;
    }
}

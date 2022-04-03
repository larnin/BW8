using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] int m_pixelPerUnit = 16;
    [SerializeField] float m_minSpeed = 1;
    [SerializeField] float m_maxSpeed = 10;
    [SerializeField] float m_speedFactor = 1;
    [SerializeField] float m_speedPow = 1;

    SubscriberList m_subscriberList = new SubscriberList();

    Vector2 m_pos;
    Vector2 m_targetPos;

    private void Awake()
    {
        m_subscriberList.Add(new Event<CenterUpdatedEvent>.Subscriber(OnMove));
        m_subscriberList.Add(new Event<CenterUpdatedEventInstant>.Subscriber(OnInstantMove));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void FixedUpdate()
    {
        Vector2 pos = m_pos;
        Vector2 dir = m_targetPos - pos;

        float distance = dir.magnitude;
        if (distance <= 0.001f)
            return;

        dir /= distance;

        float speed = Mathf.Pow(distance, m_speedPow) * m_speedFactor;
        if (speed > m_maxSpeed)
            speed = m_maxSpeed;
        if (speed < m_minSpeed)
            speed = m_minSpeed;

        speed *= Time.deltaTime;
        if (speed > distance)
            speed = distance;

        pos += speed * dir;

        SetPos(pos);
    }

    void OnMove(CenterUpdatedEvent e)
    {
        m_targetPos = e.pos;
    }

    void OnInstantMove(CenterUpdatedEventInstant e)
    {
        SetPos(e.pos);
        m_targetPos = e.pos;
    }

    void SetPos(Vector2 pos)
    {
        m_pos = pos;

        float x = Mathf.Floor(pos.x * m_pixelPerUnit) / m_pixelPerUnit;
        float y = Mathf.Floor(pos.y * m_pixelPerUnit) / m_pixelPerUnit;

        Vector3 position = transform.position;
        position.x = x;
        position.y = y;
        transform.position = position;
    }
}

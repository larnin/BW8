using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NRand;

public class SlimeControler : MonoBehaviour
{
    [SerializeField] float m_idleMinTime = 1;
    [SerializeField] float m_idleMaxTime = 2;
    [SerializeField] float m_idleDetectionMultiplier = 0.5f;
    [SerializeField] float m_jumpHeight = 1;
    [SerializeField] float m_jumpDistance = 2;
    [SerializeField] float m_jumpMoveSpeed = 2;
    [SerializeField] float m_playerDetectionRadius = 5;
    [SerializeField] LayerMask m_playerLayer;
    [SerializeField] LayerMask m_groundLayer;
    [SerializeField] float m_groundTestRadius = 0.5f;

    SubscriberList m_subscriberList = new SubscriberList();

    bool m_jumping = false;
    float m_duration = 0;
    Vector2 m_startJump = Vector2.zero;
    Vector2 m_endJump = Vector2.zero;

    GameObject m_render = null;
    GameObject m_shadow = null;

    bool m_haveFoundPlayer = false;
    Vector2 m_playerPosition = Vector2.zero;

    private void Awake()
    {
        var t = transform.Find("Blob");
        if (t != null)
            m_render = t.gameObject;
        t = transform.Find("Shadow");
        if (t != null)
            m_shadow = t.gameObject;


        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    private void Start()
    {
        var d = new UniformFloatDistribution(m_idleMinTime, m_idleMaxTime);
        m_duration = d.Next(new StaticRandomGenerator<MT19937>());

        if (m_render != null)
        {
            PlayAnimationEvent play = new PlayAnimationEvent("Idle", AnimationDirection.none, 0, true);
            Event<PlayAnimationEvent>.Broadcast(play, m_render);
        }
    }

    private void FixedUpdate()
    {
        if (m_jumping)
            Jumping();
        else
        {
            m_duration -= Time.deltaTime;
            if (m_duration <= 0)
                StartJump();
        }
    }

    void StartJump()
    {
        m_startJump = transform.position;

        Vector2 dir = Vector2.zero;
        if (m_haveFoundPlayer)
            dir = (m_playerPosition - m_startJump).normalized;
        else dir = new UniformVector2CircleSurfaceDistribution(1).Next(new StaticRandomGenerator<MT19937>());

        m_endJump = m_startJump + dir * m_jumpDistance;

        var hit = Physics2D.CircleCast(m_startJump, m_groundTestRadius, dir, m_jumpDistance, m_groundLayer.value);
        if (hit.collider != null)
            m_endJump = hit.centroid;

        m_jumping = true;

        EnableCollisions(false);

        if (m_render != null)
        {
            PlayAnimationEvent play = new PlayAnimationEvent("StartJump", AnimationDirection.none, 1, false);
            Event<PlayAnimationEvent>.Broadcast(play, m_render);
            play = new PlayAnimationEvent("Jumping", AnimationDirection.none, 1, true, true);
            Event<PlayAnimationEvent>.Broadcast(play, m_render);
        }
    }

    void Jumping()
    {
        Vector2 pos = transform.position;
        bool end = false;

        float remainingDistance = (m_endJump - pos).magnitude;
        float totalDistance = (m_endJump - m_startJump).magnitude;

        if(totalDistance <= 0.01f)
        {
            EndJump();
            return;
        }

        float move = m_jumpMoveSpeed * Time.deltaTime;
        if (move >= remainingDistance)
        {
            end = true;
            remainingDistance = 0;
        }
        else remainingDistance -= move;

        float normalizedRemaining = remainingDistance / totalDistance;

        pos = m_endJump * (1 - normalizedRemaining) + m_startJump * (normalizedRemaining);

        Vector3 pos3 = pos;
        pos3.z = transform.position.z;
        transform.position = pos3;

        if (m_render != null)
        {
            //offset of the render
            //y = 1 - (2*x-1)^2
            float param = 2 * normalizedRemaining - 1;
            float height = (1 - param * param) * m_jumpHeight;

            pos3 = m_render.transform.localPosition;
            pos3.y = height;
            m_render.transform.localPosition = pos3;
        }

        if (end)
            EndJump();
    }

    void EndJump()
    {
        if (m_render != null)
            m_render.transform.localPosition = Vector3.zero;

        EnableCollisions(true);

        m_jumping = false;

        TestDuration();

        if (m_render != null)
        {
            PlayAnimationEvent play = new PlayAnimationEvent("EndJump", AnimationDirection.none, 1, false);
            Event<PlayAnimationEvent>.Broadcast(play, m_render); 
        }
    }

    void TestDuration()
    {
        var obj = Physics2D.OverlapCircle(transform.position, m_playerDetectionRadius, m_playerLayer);
        if (obj != null)
        {
            m_haveFoundPlayer = true;
            m_playerPosition = obj.transform.position;
        }
        else m_haveFoundPlayer = false;

        var d = new UniformFloatDistribution(m_idleMinTime, m_idleMaxTime);
        m_duration = d.Next(new StaticRandomGenerator<MT19937>());

        if (m_haveFoundPlayer)
            m_duration *= m_idleDetectionMultiplier;
    }

    void EnableCollisions(bool enable)
    {
        var cols = GetComponentsInChildren<Collider2D>();
        foreach (var c in cols)
            c.enabled = enable;
    }
}


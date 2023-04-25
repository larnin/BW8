using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NRand;

public class MoleControler : MonoBehaviour
{
    enum State
    {
        underground,
        tryGettingOut,
        gettingOut,
        stepOut,
        throwObject,
        stepOutEnd,
        gettingIn,
    }

    [SerializeField] float m_maxMoveRange = 5;
    [SerializeField] float m_playerDetectionRange = 6;
    [SerializeField] float m_minPlayerDistanceOut = 2;
    [SerializeField] float m_maxPlayerDistanceOut = 3;
    [SerializeField] float m_hideDistance = 1.5f;
    [SerializeField] float m_undergroundDuration = 2;
    [SerializeField] float m_outDurationBeforeThrow = 2;
    [SerializeField] float m_outDurationAfterThrow = 1;
    [SerializeField] float m_throwTimeDelay = 0.1f;
    [SerializeField] float m_collisionRadius = 0.2f;
    [SerializeField] ProjectileType m_projectile;
    [SerializeField] float m_projectileSpeed = 5;
    [SerializeField] float m_projectileDistance = 5;


    SubscriberList m_subscriberList = new SubscriberList();

    Rigidbody2D m_rigidbody = null;

    State m_state = State.underground;

    AnimationDirection m_direction = AnimationDirection.Down;
    float m_totalTimer = 1;
    float m_timer = 1;
    bool m_haveThrow = false;

    GameObject m_target = null;
    GameObject m_render = null;
    Collider2D m_collider = null;

    private void Awake()
    {
        m_subscriberList.Add(new Event<DeathEvent>.LocalSubscriber(OnDeath, gameObject));
        m_subscriberList.Subscribe();

        m_rigidbody = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<Collider2D>();

        var t = transform.Find("Render");
        if (t != null)
            m_render = t.gameObject;
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnDeath(DeathEvent e)
    {
        enabled = false;
    }

    private void Update()
    {
        m_rigidbody.velocity = Vector2.zero;
        m_rigidbody.angularVelocity = 0;

        m_timer -= Time.deltaTime;

        switch(m_state)
        {
            case State.underground:
                UpdateUnderground();
                break;
            case State.tryGettingOut:
                UpdateTryGettingOut();
                break;
            case State.gettingOut:
                UpdateGettingOut();
                break;
            case State.stepOut:
                UpdateStepOut();
                break;
            case State.throwObject:
                UpdateThrowObject();
                break;
            case State.stepOutEnd:
                UpdateStepOutEnd();
                break;
            case State.gettingIn:
                UpdateGettingIn();
                break;
        }
    }

    void UpdateTarget()
    {
        CommonData common = World.common;

        var target = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y), m_playerDetectionRange, common.playerLayer);
        if (target == null)
            m_target = null;
        else m_target = target.gameObject;
    }

    void UpdateUnderground()
    {
        if (m_timer <= 0)
            m_state = State.tryGettingOut;

        UpdateTarget();
    }

    void UpdateTryGettingOut()
    {
        CommonData common = World.common;

        if(m_target == null)
        {
            StartGettingOut();
            return;
        }

        var rand = new StaticRandomGenerator<MT19937>();
        var genDir = new UniformVector2CircleSurfaceDistribution();
        var genDist = new UniformFloatDistribution();

        for(int i = 0; i < 10; i++)
        {
            var dir = genDir.Next(rand);
            float maxDist = m_maxPlayerDistanceOut;

            var col = Physics2D.CircleCast(m_target.transform.position, m_collisionRadius, dir, m_maxPlayerDistanceOut, common.groundLayer);
            if (col.collider != null && maxDist > col.distance)
                maxDist = col.distance;

            if (maxDist < m_minPlayerDistanceOut)
                continue;

            float dist = genDist.Next(rand) * (maxDist - m_minPlayerDistanceOut) + m_minPlayerDistanceOut;

            Vector2 pos = m_target.transform.position;
            pos += dir * dist;
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);

            StartGettingOut();
            return;
        }

        UpdateTarget();
    }

    void StartGettingOut()
    {
        m_state = State.gettingOut;

        UpdateDirection();

        m_render.SetActive(true);
        m_collider.enabled = true;

        PlayAnimationEvent play = new PlayAnimationEvent("GetOut", m_direction, 0, false);
        Event<PlayAnimationEvent>.Broadcast(play, m_render);

        GetAnimationDurationEvent duration = new GetAnimationDurationEvent("GetOut", m_direction);
        Event<GetAnimationDurationEvent>.Broadcast(duration, m_render);
        m_timer = duration.duration;
        m_totalTimer = duration.duration;
    }

    void UpdateGettingOut()
    {
        if(m_timer <= 0)
        {
            m_state = State.stepOut;
            m_timer = m_outDurationBeforeThrow;
            m_totalTimer = m_outDurationBeforeThrow;

            UpdateOutAnim(true);
        }
    }

    void UpdateStepOut()
    {
        UpdateTarget();
        UpdateOutAnim();
        CheckPlayer();

        if (m_target == null)
            m_timer = m_outDurationBeforeThrow;

        if(m_timer <= 0)
        {
            m_state = State.throwObject;

            PlayAnimationEvent play = new PlayAnimationEvent("Throw", m_direction, 0, false);
            Event<PlayAnimationEvent>.Broadcast(play, m_render);

            GetAnimationDurationEvent duration = new GetAnimationDurationEvent("Throw", m_direction);
            Event<GetAnimationDurationEvent>.Broadcast(duration, m_render);
            m_timer = duration.duration;
            m_totalTimer = duration.duration;

            m_haveThrow = false;
        }
    }

    void UpdateThrowObject()
    {
        float delay = m_totalTimer - m_timer;
        if((delay > m_throwTimeDelay || m_timer <= 0) && !m_haveThrow)
        {
            m_haveThrow = true;
            ThrowObject();
        }

        if(m_timer <= 0)
        {
            m_state = State.stepOutEnd;
            m_timer = m_outDurationBeforeThrow;
            m_totalTimer = m_outDurationBeforeThrow;

            m_haveThrow = false;

            UpdateOutAnim(true);
        }
    }

    void UpdateStepOutEnd()
    {
        UpdateTarget();
        UpdateOutAnim();
        CheckPlayer();

        if(m_target == null)
        {
            m_timer = m_outDurationBeforeThrow;
            m_state = State.stepOut;
        }

        if (m_timer <= 0)
            StartHide();
    }

    void StartHide()
    {
        m_state = State.gettingIn;

        PlayAnimationEvent play = new PlayAnimationEvent("GetIn", m_direction, 0, false);
        Event<PlayAnimationEvent>.Broadcast(play, m_render);

        GetAnimationDurationEvent duration = new GetAnimationDurationEvent("GetIn", m_direction);
        Event<GetAnimationDurationEvent>.Broadcast(duration, m_render);
        m_timer = duration.duration;
        m_totalTimer = duration.duration;
    }

    void UpdateGettingIn()
    {
        if(m_timer <= 0)
        {
            m_state = State.underground;

            m_render.SetActive(false);
            m_collider.enabled = false;

            m_timer = m_undergroundDuration;
            m_totalTimer = m_undergroundDuration;
        }
    }

    void UpdateDirection()
    {
        if (m_target == null)
            return;

        Vector2 see = m_target.transform.position - transform.position;
        m_direction = AnimationDirectionEx.GetDirection(see);
        if (m_direction == AnimationDirection.none)
            m_direction = AnimationDirection.Down;
    }

    void UpdateOutAnim(bool forceAnim = false)
    {
        AnimationDirection dir = m_direction;
        UpdateDirection();

        if(dir != m_direction || forceAnim)
        {
            PlayAnimationEvent play = new PlayAnimationEvent("OutLoop", m_direction, 0, true);
            Event<PlayAnimationEvent>.Broadcast(play, m_render);
        }
    }

    void ThrowObject()
    {
        if (m_target == null)
            return;

        var projectiles = World.projectiles;
        var commonData = World.common;
        if (projectiles == null || commonData == null)
            return;

        var prefab = projectiles.GetProjectile(m_projectile);
        if (prefab == null)
            return;

        var obj = Instantiate(prefab);

        var projectile = obj.GetComponent<IProjectile>();
        if(projectile != null)
        {
            projectile.SetVelocity(m_projectileSpeed);
            projectile.SetMaxDistance(m_projectileDistance);
            projectile.SetHitLayer(commonData.playerLayer);
            projectile.Throw();
        }

        Vector3 pos = transform.position;
        pos.z -= 0.1f;
        obj.transform.position = pos;

        Vector3 dir = m_target.transform.position - pos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, angle);
        obj.transform.rotation = rot;
    }

    void CheckPlayer()
    {
        if (m_target == null)
            return;

        Vector2 dir = m_target.transform.position - transform.position;
        float dist = dir.sqrMagnitude;

        if (dist <= m_hideDistance * m_hideDistance)
            StartHide();
    }
}

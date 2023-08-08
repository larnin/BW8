using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SimpleProjectile : MonoBehaviour
{
    [SerializeField] float m_speed = 5;
    [SerializeField] float m_distance = 5;
    [SerializeField] int m_damages = 1;
    [SerializeField] float m_knockback = 0;
    [SerializeField] LayerMask m_hitLayer;
    [SerializeField] float m_height = 1;
    [SerializeField] float m_fallTime = 1;
    [SerializeField] float m_radius = 0.5f;

    int m_useDamages;
    float m_useKnockback;
    LayerMask m_useHitLayer;
    float m_useDistance;
    float m_useSpeed;

    float m_traveledDistance = 0;
    bool m_started = false;
    float m_shadowOffset;
    bool m_falling;
    float m_fallTimer;

    Transform m_shadow;
    Transform m_render;

    GameObject m_caster;

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_shadow = transform.Find("Shadow");
        m_render = transform.Find("Render");
        m_shadowOffset = m_height;
        m_falling = false;

        m_subscriberList.Add(new Event<CanBeThrownEvent>.LocalSubscriber(CanBeThrown, gameObject));
        m_subscriberList.Add(new Event<SetProjectileDataEvent>.LocalSubscriber(SetData, gameObject));
        m_subscriberList.Add(new Event<ThrowEvent>.LocalSubscriber(Throw, gameObject));

        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    private void LateUpdate()
    {
        m_render.rotation = Quaternion.identity;
        if(m_shadow != null)
            m_shadow.rotation = Quaternion.identity;
    }

    void FixedUpdate()
    {
        if (!m_started)
            return;
        if (m_falling && m_fallTimer > m_fallTime)
        {
            m_started = false;

            Event<SetDefaultBehaviourEvent>.Broadcast(new SetDefaultBehaviourEvent(), gameObject);

            return;
        }

        float speed = m_useSpeed;

        if (m_falling)
        {
            m_fallTimer += Time.deltaTime;
            float normalizedTime = m_fallTimer / m_fallTime;
            speed = m_useSpeed * (1 - normalizedTime);
        }

        Vector3 pos = transform.position;

        float dist = speed * Time.deltaTime;
        m_traveledDistance += dist;
        Vector3 dir = transform.right * dist;
        if (!m_falling && m_traveledDistance >= m_useDistance)
        {
            m_falling = true;
            m_fallTimer = 0;
        }

        Vector3 newPos = pos + dir;

        float height = GetHeight();

        float delta = height - m_shadowOffset;
        newPos.y += delta;

        m_shadowOffset = height;

        Vector3 movement = newPos - pos;
        Vector3 newMovement = movement;
        if (dist > 0)
        {
            newMovement = ProcessCollide(movement);
            if (newMovement != movement)
            {
                bool setDirection = true;
                Vector3 secondHitMovement = ProcessCollide(newMovement);
                if (secondHitMovement != newMovement)
                {
                    setDirection = false;
                    m_useSpeed = 0;
                    m_falling = true;
                }

                float newDist = newMovement.magnitude;
                if (newDist < 0.01f)
                {
                    setDirection = false;
                    m_useSpeed = 0;
                    m_falling = true;
                }

                if (setDirection)
                {
                    Vector3 newDir = newMovement.normalized;
                    Vector3 up = transform.up;
                    transform.right = newDir;
                    transform.up = up;
                }
            }
        }
        newPos = pos + newMovement;
        transform.position = newPos;

        if (m_shadow != null)
        {
            newPos.y -= m_shadowOffset;
            newPos.z = m_shadow.position.z;
            m_shadow.position = newPos;
        }
    }

    float GetHeight()
    {
        if (!m_falling)
            return m_height;

        float normalizedTime = m_fallTimer / m_fallTime;

        float height = DOVirtual.EasedValue(m_height, 0, normalizedTime, Ease.OutBounce);

        return height;
    }

    Vector3 ProcessCollide(Vector3 dir)
    {
        Vector3 pos = transform.position;
        float dist = dir.magnitude;
        Vector3 normDir = dir / dist;

        var hits = Physics2D.CircleCastAll(pos, m_radius, normDir, dist, m_useHitLayer);

        if (hits.Length == 0)
            return dir;

        bool collide = false;
        float distance = 0;
        Vector2 normal = Vector2.zero;
        GameObject hitObject = null;

        foreach(var hit in hits)
        {
            if (hit.collider.isTrigger)
                continue;

            if (hit.collider.gameObject == m_caster || hit.collider.gameObject == gameObject)
                continue;

            if(!collide || hit.distance < distance)
            {
                collide = true;
                distance = hit.distance;
                normal = hit.normal;
                hitObject = hit.collider.gameObject;
            }
        }

        if(collide)
        {
            if (hitObject != null)
            {
                GameObject caster = m_caster == null ? gameObject : m_caster;
                Event<HitEvent>.Broadcast(new HitEvent(m_useDamages, caster, m_useKnockback), hitObject);
            }

            var newDir = Vector3.Reflect(dir, normal);
            float newDist = dist - distance;
            if (newDist < 0)
                return dir;

            if (!m_falling)
            {
                m_falling = true;
                m_fallTimer = 0;
            }

            if (newDist <= 0.001f)
                return Vector3.zero;

            m_useSpeed *= 0.5f;

            return newDir;
        }

        return dir;
    }

    void Init()
    {
        m_useDamages = m_damages;
        m_useKnockback = m_knockback;
        m_useHitLayer = m_hitLayer;
        m_useDistance = m_distance;
        m_useSpeed = m_speed;
    }

    void CanBeThrown(CanBeThrownEvent e)
    {
        e.canBeThrown = true;
    }

    void SetData(SetProjectileDataEvent e)
    {
        Init();

        if (e.damages >= 0)
            m_useDamages = e.damages;
        if (e.knockback >= 0)
            m_useKnockback = e.knockback;

        m_useHitLayer = e.hitLayer;

        if (e.maxDistance >= 0)
            m_useDistance = e.maxDistance;

        if(e.speed >= 0)
            m_useSpeed = e.speed;

        m_caster = e.caster;
    }

    void Throw(ThrowEvent e)
    {
        m_started = true;
        m_traveledDistance = 0;
        m_falling = false;
        m_fallTimer = 0;
        m_shadowOffset = m_height;

        if (m_shadow != null)
        {
            Vector3 pos = transform.position;
            pos.y -= m_height;
            pos.z = m_shadow.position.z;
            m_shadow.position = pos;
        }

        Event<SetExclusiveBehaviourEvent>.Broadcast(new SetExclusiveBehaviourEvent(this), gameObject);
    }
}
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SimpleProjectile : MonoBehaviour, IProjectile
{
    [SerializeField] float m_speed = 5;
    [SerializeField] float m_distance = 5;
    [SerializeField] int m_damages = 1;
    [SerializeField] float m_knockback = 0;
    [SerializeField] LayerMask m_hitLayer;
    [SerializeField] float m_height = 1;
    [SerializeField] float m_fallTime = 1;
    [SerializeField] float m_radius = 0.5f;

    float m_traveledDistance = 0;
    bool m_started = false;
    float m_shadowOffset;
    bool m_falling;
    float m_fallTimer;

    Transform m_shadow;
    Transform m_render;
    Collider2D m_collider;

    GameObject m_caster;

    ProjectileType m_projectileType = ProjectileType.Stone;

    private void Awake()
    {
        m_collider = GetComponent<Collider2D>();
        m_collider.enabled = false;
        m_shadow = transform.Find("Shadow");
        m_render = transform.Find("Render");
        m_shadowOffset = m_height;
        m_falling = false;
    }

    public void SetDamage(int damages, float knockback)
    {
        if (damages >= 0)
            m_damages = damages;
        if (knockback >= 0)
            m_knockback = knockback;
    }

    public void SetHitLayer(LayerMask hitLayer)
    {
        m_hitLayer = hitLayer;
    }

    public void SetMaxDistance(float distance)
    {
        if (distance >= 0)
            m_distance = distance;
    }

    public void SetVelocity(float speed)
    {
        m_speed = speed;
    }

    public void SetCaster(GameObject caster)
    {
        m_caster = caster;
    }

    public void Throw()
    {
        m_started = true;
        m_traveledDistance = 0;
        m_falling = false;
        m_fallTimer = 0;
        m_shadowOffset = m_height;

        Vector3 pos = transform.position;
        pos.y -= m_height;
        pos.z = m_shadow.position.z;
        m_shadow.position = pos;
    }

    private void LateUpdate()
    {
        m_render.rotation = Quaternion.identity;
        m_shadow.rotation = Quaternion.identity;
    }

    void FixedUpdate()
    {
        if (!m_started)
            return;
        if (m_falling && m_fallTimer > m_fallTime)
            return;

        float speed = m_speed;

        if (m_falling)
        {
            m_fallTimer += Time.deltaTime;
            float normalizedTime = m_fallTimer / m_fallTime;
            speed = m_speed * (1 - normalizedTime);
        }

        Vector3 pos = transform.position;

        float dist = speed * Time.deltaTime;
        m_traveledDistance += dist;
        Vector3 dir = transform.right * dist;
        if (!m_falling && m_traveledDistance >= m_distance)
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
                    m_speed = 0;
                    m_falling = true;
                }

                float newDist = newMovement.magnitude;
                if (newDist < 0.01f)
                {
                    setDirection = false;
                    m_speed = 0;
                    m_falling = true;
                }

                if (setDirection)
                {
                    Vector3 newDir = newMovement.normalized;
                    transform.right = newDir;
                }
            }
        }
        newPos = pos + newMovement;
        transform.position = newPos;

        newPos.y -= m_shadowOffset;
        newPos.z = m_shadow.position.z;
        m_shadow.position = newPos;
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

        var hits = Physics2D.CircleCastAll(pos, m_radius, normDir, dist);

        if (hits.Length == 0)
            return dir;

        bool collide = false;
        float distance = 0;
        Vector2 normal = Vector2.zero;
        GameObject hitObject = null;

        foreach(var hit in hits)
        {
            if (hit.collider.gameObject == m_caster)
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
                int layer = 1 << hitObject.layer;
                if ((layer & m_hitLayer.value) != 0)
                {
                    GameObject caster = m_caster == null ? gameObject : m_caster;
                    Event<HitEvent>.Broadcast(new HitEvent(m_damages, caster, m_knockback), hitObject);
                }
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

            m_speed *= 0.5f;

            return newDir;
        }

        return dir;
    }

    public ProjectileType GetProjectileType()
    {
        return m_projectileType;
    }

    public void SetProjectileType(ProjectileType type)
    {
        m_projectileType = type;
    }

    public void StartAttract(GameObject target)
    {
        throw new System.NotImplementedException();
    }

    public void StopAttract()
    {
        throw new System.NotImplementedException();
    }
}
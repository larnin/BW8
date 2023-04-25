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

    bool m_isThrow = false;
    float m_traveledDistance = 0;
    bool m_started = false;
    float m_shadowOffset;
    bool m_falling;
    float m_fallTimer;

    Transform m_shadow;
    Transform m_render;
    Collider2D m_collider;

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
}
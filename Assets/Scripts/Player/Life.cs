using System.Collections;
using UnityEngine;

public class Life : MonoBehaviour
{
    [SerializeField] int m_maxLife = 5;
    [SerializeField] float m_invincibilityDuration = 0;

    int m_life = 0;
    float m_invincibilityTime = 0;

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<GetLifeEvent>.LocalSubscriber(GetLife, gameObject));
        m_subscriberList.Add(new Event<SetLifeEvent>.LocalSubscriber(SetLife, gameObject));
        m_subscriberList.Add(new Event<SetLifePercentEvent>.LocalSubscriber(SetLifePercent, gameObject));
        m_subscriberList.Add(new Event<RegenLifeEvent>.LocalSubscriber(Regen, gameObject));
        m_subscriberList.Add(new Event<HitEvent>.LocalSubscriber(Damage, gameObject));

        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    private void Start()
    {
        m_life = m_maxLife;
    }

    private void Update()
    {
        if(!Gamestate.instance.paused)
        {
            m_invincibilityTime -= Time.deltaTime;
            if (m_invincibilityTime < 0)
                m_invincibilityTime = 0;
        }
    }

    void GetLife(GetLifeEvent e)
    {
        e.life = m_life;
        e.maxLife = m_maxLife;
    }

    void SetLife(int value)
    {
        if (value < 0)
            value = 0;
        if (value > m_maxLife)
            value = m_maxLife;

        if (value == m_life)
            return;

        int oldLife = m_life;
        m_life = value;

        if(m_life < oldLife)
        {
            if (m_life <= 0)
                Event<DeathEvent>.Broadcast(new DeathEvent(null), gameObject, true);
            else Event<LifeLossEvent>.Broadcast(new LifeLossEvent(null, 0), gameObject, true);
        }
        else Event<LifeHealEvent>.Broadcast(new LifeHealEvent(), gameObject, true);
    }

    void SetLife(SetLifeEvent e)
    {
        SetLife(e.life);
    }

    void SetLifePercent(SetLifePercentEvent e)
    {
        SetLife(Mathf.RoundToInt(e.lifePercent * m_maxLife));
    }

    void Regen(RegenLifeEvent e)
    {
        m_life += e.value;
        if (m_life > m_maxLife)
            m_life = m_maxLife;

        Event<LifeHealEvent>.Broadcast(new LifeHealEvent(), gameObject, true);
    }

    void Damage(HitEvent e)
    {
        if (m_life <= 0)
            return;

        if (m_invincibilityTime > 0)
            return;

        m_life -= e.damage;

        m_invincibilityTime = m_invincibilityDuration;

        if (m_life <= 0)
            Event<DeathEvent>.Broadcast(new DeathEvent(e.caster), gameObject, true);
        else Event<LifeLossEvent>.Broadcast(new LifeLossEvent(e.caster, e.knockback), gameObject, true);
    }
}
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

    void Regen(RegenLifeEvent e)
    {
        m_life += e.value;
        if (m_life > m_maxLife)
            m_life = m_maxLife;
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
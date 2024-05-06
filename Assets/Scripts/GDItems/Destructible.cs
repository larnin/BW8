using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    static string SavePrefix = "Destructible";

    [Serializable]
    class LifeInfo
    {
        public float lifePercent = 100;
        public Sprite sprite = null;
    }

    [SerializeField] List<LifeInfo> m_sprites = new List<LifeInfo>();
    [SerializeField] bool m_save = false;
    [ShowIf("m_save")]
    [SerializeField] string m_saveKey = "";
    [SerializeField] [HideInInspector] bool m_keyGenerated = false;
    [SerializeField] bool m_disableCollisionOnDeath = true;
    [SerializeField] ParticleSystem m_hitParticles = null;

    SubscriberList m_subscriberList = new SubscriberList();

    SpriteRenderer m_renderer;

    private void Awake()
    {
        m_subscriberList.Add(new Event<LifeLossEvent>.LocalSubscriber(OnHit, gameObject));
        m_subscriberList.Add(new Event<DeathEvent>.LocalSubscriber(OnDeath, gameObject));
        m_subscriberList.Add(new Event<LifeHealEvent>.LocalSubscriber(OnHeal, gameObject));

        m_subscriberList.Subscribe();
    }

    private void Start()
    {
        m_renderer = GetComponent<SpriteRenderer>();

        if(m_save)
        {
            bool destroyed = SaveSystem.instance.GetDatas().GetBool(SavePrefix + '/' + m_saveKey + "/destroyed");
            if(destroyed)
                Event<SetLifeEvent>.Broadcast(new SetLifeEvent(0), gameObject);
        }
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    [ShowIf("m_save")]
    [Button("Generate")]
    void GenerateKey()
    {
        m_keyGenerated = true;

        m_saveKey = Guid.NewGuid().ToString("N");
    }

    [OnInspectorGUI]
    void OnInspector()
    {
        if (m_keyGenerated == false)
            GenerateKey();

        if (m_save)
        {
            var destructibles = UnityEngine.Object.FindObjectsOfType<Destructible>();
            foreach (var d in destructibles)
            {
                if (d == this)
                    continue;

                if (d.m_saveKey == m_saveKey)
                {
                    GUILayout.Box("An other destructible have the same save key");
                    break;
                }
            }
        }
    }

    void OnHit(LifeLossEvent e)
    {
        UpdateRender();

        if(m_hitParticles != null)
            m_hitParticles.Play();
    }

    void OnHeal(LifeHealEvent e)
    {
        UpdateRender();
        if (m_disableCollisionOnDeath)
        {
            var colliders = GetComponentsInChildren<Collider2D>();
            foreach (var col in colliders)
                col.enabled = true;
        }
    }

    void OnDeath(DeathEvent e)
    {
        UpdateRender();
        if (m_hitParticles != null)
            m_hitParticles.Play();
        if(m_disableCollisionOnDeath)
        {
            var colliders = GetComponentsInChildren<Collider2D>();
            foreach (var col in colliders)
                col.enabled = false;
        }

        if(m_save)
            SaveSystem.instance.GetDatas().Set(SavePrefix + '/' + m_saveKey + "/destroyed", true);
    }

    void UpdateRender()
    {
        GetLifeEvent lifeData = new GetLifeEvent();
        Event<GetLifeEvent>.Broadcast(lifeData, gameObject);

        float lifePercent100 = lifeData.life * 100.0f / lifeData.maxLife;

        int index = -1;
        for(int i = 0; i < m_sprites.Count; i++)
        {
            if (lifePercent100 > m_sprites[i].lifePercent)
                continue;

            if (index >= 0 && m_sprites[i].lifePercent < m_sprites[i].lifePercent)
                continue;

            index = i;
        }

        if (index < 0 || m_renderer == null)
            return;

        m_renderer.sprite = m_sprites[index].sprite;
    }
}

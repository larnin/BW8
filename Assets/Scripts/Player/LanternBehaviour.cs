using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LanternBehaviour : MonoBehaviour
{
    [SerializeField] bool m_defaultState = false;
    [SerializeField] GameObject m_renderObject = null;
    [SerializeField] ParticleSystem m_particles = null;
    [SerializeField] Sprite m_offSprite = null;
    [SerializeField] Sprite m_onSprite = null;

    [SerializeField] float m_horizontalMoveDistance = 0.5f;
    [SerializeField] float m_horizontalMoveSpeed = 1;

    [SerializeField] float m_verticalMoveDistance = 0.5f;
    [SerializeField] float m_verticalMoveSpeed = 1;

    float m_verticalTimer = 0;
    float m_horizontalTimer = 0;

    bool m_state;

    Vector3 m_renderInitialPos;

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<SetLanternState>.LocalSubscriber(SetState, gameObject));
        m_subscriberList.Subscribe();
    }

    private void Start()
    {
        m_renderInitialPos = m_renderObject.transform.localPosition;
        m_state = m_defaultState;
        UpdateState();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void SetState(SetLanternState e)
    {
        m_state = e.state;
        UpdateState();
    }

    void UpdateState()
    {
        var renderer = m_renderObject?.GetComponent<SpriteRenderer>();

        if (m_state)
        {
            m_particles.Play();
            if (renderer != null)
                renderer.sprite = m_onSprite;
        }
        else
        {
            m_particles.Stop();
            if (renderer != null)
                renderer.sprite = m_offSprite;
        }
    }

    private void Update()
    {
        m_verticalTimer += Time.deltaTime;
        m_horizontalTimer += Time.deltaTime;

        float x = GetOffset(m_horizontalTimer, m_horizontalMoveSpeed, m_horizontalMoveDistance / 2);
        float y = GetOffset(m_verticalTimer, m_verticalMoveSpeed, m_verticalMoveDistance / 2);

        m_renderObject.transform.localPosition = m_renderInitialPos + new Vector3(x, y, 0);
    }

    float GetOffset(float time, float speed, float amplitude)
    {
        return amplitude * (Mathf.Cos(time * speed) + Mathf.Sin(time * speed * 2));
    }
}

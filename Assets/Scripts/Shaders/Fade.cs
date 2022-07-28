using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Fade : MonoBehaviour
{
    const string fadeName = "_Fade";

    [SerializeField] float m_transitionDuration = 0.5f;
    [SerializeField] Material m_material = null;

    SubscriberList m_subscriberList = new SubscriberList();

    [Range(0, 1)]
    [SerializeField] float m_fadePower = 0;

    private void Awake()
    {
        m_subscriberList.Add(new Event<ShowLoadingScreenEvent>.Subscriber(OnFade));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void Start()
    {
        if (m_material == null || m_material.shader == null || !m_material.shader.isSupported)
        {
            enabled = false;
            return;
        }
    }

    void OnFade(ShowLoadingScreenEvent e)
    {
        if (e.start)
        {
            DOVirtual.Float(0, 1, m_transitionDuration, (x) => { m_fadePower = x; });
        }
        else
        {
            DOVirtual.Float(1, 0, m_transitionDuration, (x) => { m_fadePower = x; });
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, m_material);
    }

    private void Update()
    {
        m_material.SetFloat(fadeName, m_fadePower);
    }
}
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FadeUI : MonoBehaviour
{
    static FadeUI instance = null;

    [SerializeField] float m_transitionDuration = 0.5f;
    [SerializeField] float m_transitionDistance = 100;

    SubscriberList m_subscriberList = new SubscriberList();

    Transform m_plane;
    Canvas m_canvas;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        m_subscriberList.Add(new Event<ShowLoadingScreenEvent>.Subscriber(OnFade));
        m_subscriberList.Subscribe();

        m_plane = transform.Find("Image");
        m_plane.localPosition = new Vector3(m_transitionDistance, 0, 0);

        m_canvas = GetComponent<Canvas>();
    }

    private void Update()
    {
        if(m_canvas.worldCamera == null)
        {
            GetUICameraEvent camEvent = new GetUICameraEvent();
            Event<GetUICameraEvent>.Broadcast(camEvent);
            m_canvas.worldCamera = camEvent.camera;
            m_canvas.planeDistance = 0.5f;
        }
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnFade(ShowLoadingScreenEvent e)
    {
        if (e.type != LoadingScreenType.Menu)
            return;

        if (e.start)
        {
            m_plane.localPosition = new Vector3(m_transitionDistance, 0, 0);
            m_plane.DOLocalMoveX(0, m_transitionDuration);
        }
        else
        {
            m_plane.localPosition = new Vector3(0, 0, 0);
            m_plane.DOLocalMoveX(-m_transitionDistance, m_transitionDuration);
        }
    }
}

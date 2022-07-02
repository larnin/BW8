using System.Collections;
using UnityEngine;

public class CameraGroup : MonoBehaviour
{
    SubscriberList m_subscriberList = new SubscriberList();

    static CameraGroup m_instance = null;

    Camera m_camera = null;

    private void Awake()
    {
        if (m_instance != null)
            Destroy(gameObject);

        m_instance = this;

        DontDestroyOnLoad(gameObject);

        var obj = transform.Find("Render");
        if (obj != null)
            m_camera = obj.GetComponentInChildren<Camera>();
        else m_camera = GetComponentInChildren<Camera>();

        m_subscriberList.Add(new Event<GetUICameraEvent>.Subscriber(GetUICamera));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void GetUICamera(GetUICameraEvent e)
    {
        e.camera = m_camera;
    }
}
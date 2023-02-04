using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CameraPlaneOffset : MonoBehaviour
{
    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<SetCameraPlaneOffsetEvent>.Subscriber(SetOffset));

        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void SetOffset(SetCameraPlaneOffsetEvent e)
    {
        transform.position = new Vector3(-e.offset.x, -e.offset.y, 0);
    }
}

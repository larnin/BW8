using DG.Tweening;
using System.Collections;
using UnityEngine;

public class HitFlashBehaviour : MonoBehaviour
{
    const string additiveName = "_Color";

    [SerializeField] float m_flashDuration = 0.2f;

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<LifeLossEvent>.LocalSubscriber(Damage, gameObject));

        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void Damage(LifeLossEvent e)
    {
        var renders = GetComponentsInChildren<Renderer>();

        foreach(var r in renders)
        {
            var mats = r.materials;
            foreach(var m in mats)
            {
                if(m.HasProperty(additiveName))
                {
                    m.SetColor(additiveName, Color.black);
                    DOVirtual.DelayedCall(m_flashDuration, () =>
                    {
                        if (m != null)
                            m.SetColor(additiveName, Color.white);
                    });
                }
            }
        }
    }
}

using System.Collections;
using UnityEngine;
using TMPro;
using NLocalization;

public class PlayerInteract : MonoBehaviour
{
    float m_interactionRadius = 2;
    LayerMask m_interactionMask;
    SubscriberList m_subscriberList = new SubscriberList();

    Interactable m_nearestInteractable = null;
    TMP_Text m_text = null;

    public DialogObject m_dialog;

    private void Awake()
    {
        m_subscriberList.Add(new Event<StartInteractEvent>.LocalSubscriber(OnInteraction, gameObject));
        m_subscriberList.Subscribe();

        m_text = GetComponentInChildren<TMP_Text>();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void Update()
    {
        if(Time.time >= 1 && Time.time - Time.deltaTime < 1)
        {
            DialogPopup.StartDialog(m_dialog);
        }

        return;

        Vector2 currentPos = transform.position;

        var objs = Physics2D.OverlapCircleAll(transform.position, m_interactionRadius, m_interactionMask);
        bool foundNewObject = false;
        Interactable newInteractable = null;
        float newDist = float.MaxValue;

        foreach(var o in objs)
        {
            var comp = o.GetComponent<Interactable>();
            if (comp == null)
                continue;

            Vector2 pos = o.transform.position;
            float dist = (currentPos - pos).sqrMagnitude;
            if(dist < newDist || !foundNewObject)
            {
                foundNewObject = true;
                newInteractable = comp;
                newDist = dist;
            }
        }

        bool updateText = false;

        if(m_nearestInteractable != null && m_nearestInteractable != newInteractable)
        {
            Vector2 pos = m_nearestInteractable.transform.position;
            float dist = (currentPos - pos).sqrMagnitude;
            if (dist >= m_interactionRadius * m_interactionRadius)
            {
                updateText = true;
                m_nearestInteractable = null;
            }

            if (newInteractable != null && newDist < dist)
            {
                updateText = true;
                m_nearestInteractable = null;
            }
        }

        if (m_nearestInteractable == null)
        {
            m_nearestInteractable = newInteractable;
            updateText = true;
        }

        DisplayInteraction(updateText);
    }

    void OnInteraction(StartInteractEvent e)
    {
        if(m_nearestInteractable != null)
        {
            m_nearestInteractable.Interact(gameObject);
        }
    }

    void DisplayInteraction(bool updateText)
    {
        if(m_nearestInteractable == null)
        {
            m_text.gameObject.SetActive(false);
            return;
        }

        m_text.gameObject.SetActive(true);

        var pos = m_nearestInteractable.transform.position;

        Vector3 textPos = m_text.transform.position;
        textPos.x = pos.x;
        textPos.y = pos.y;
        m_text.transform.position = textPos;

        if(updateText)
        {
            var textID = m_nearestInteractable.GetInteractionTextID();
            m_text.text = Loc.Tr(textID);
        }
    }
}
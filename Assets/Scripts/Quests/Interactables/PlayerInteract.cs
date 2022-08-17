using System.Collections;
using UnityEngine;
using TMPro;
using NLocalization;
using System.Collections.Generic;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] float m_interactionRadius = 2;
    [SerializeField] LayerMask m_interactionMask;
    [SerializeField] GameObject m_textPrefab = null;

    SubscriberList m_subscriberList = new SubscriberList();

    Interactable m_nearestInteractable = null;
    GameObject m_textObject = null;

    List<Interactable> m_lastInstantInteractables = new List<Interactable>();

    private void Awake()
    {
        m_subscriberList.Add(new Event<StartInteractEvent>.LocalSubscriber(OnInteraction, gameObject));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void Update()
    {
        Vector2 currentPos = transform.position;

        var objs = Physics2D.OverlapCircleAll(transform.position, m_interactionRadius, m_interactionMask);
        bool foundNewObject = false;
        Interactable newInteractable = null;
        float newDist = float.MaxValue;

        List<Interactable> newInstant = new List<Interactable>();

        foreach(var o in objs)
        {
            var comp = o.GetComponent<Interactable>();
            if (comp == null)
                continue;

            if (!comp.CanInteract())
                continue;
            
            if(comp.IsInstantInteraction())
            {
                newInstant.Add(comp);
                continue;
            }    

            Vector2 pos = o.transform.position;
            float dist = (currentPos - pos).sqrMagnitude;
            if(dist < newDist || !foundNewObject)
            {
                foundNewObject = true;
                newInteractable = comp;
                newDist = dist;
            }
        }

        //trigger instant interactions
        foreach(var i in newInstant)
        {
            if (m_lastInstantInteractables.Contains(i))
                continue;
            i.Interact(gameObject);
        }
        m_lastInstantInteractables = newInstant;

        //update interraction text
        bool updateText = false;

        if(!m_nearestInteractable.CanInteract())
        {
            m_nearestInteractable = null;
            updateText = true;
        }

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
            m_nearestInteractable.Interact(gameObject);
    }

    void DisplayInteraction(bool updateText)
    {
        if(m_nearestInteractable == null && m_textObject != null)
        {
            m_textObject.transform.SetParent(null);
            m_textObject.SetActive(false);
            return;
        }
        if (m_nearestInteractable == null)
            return;

        if (m_textObject == null)
        {
            if (m_textPrefab == null)
                return;
            m_textObject = Instantiate(m_textPrefab);
        }

        if (m_textObject == null)
            return;

        m_textObject.SetActive(true);

        var pos = m_nearestInteractable.transform.position;

        Vector3 textPos = m_textObject.transform.position;
        textPos.x = pos.x;
        textPos.y = pos.y;
        m_textObject.transform.position = textPos;

        if(updateText)
        {
            var textID = m_nearestInteractable.GetInteractionTextID();
            var tmp = m_textObject.GetComponentInChildren<TMP_Text>();
            if(tmp != null)
                tmp.text = Loc.Tr(textID);
        }
    }
}
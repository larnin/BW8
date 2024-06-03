using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMStateDisplayBubble : BSMStateBase
{
    static string textName = "Text";
    static string waitBubbleText = "Wait Bubble";

    GameObject m_bubble;

    public BSMStateDisplayBubble()
    {
        AddAttribute(textName, BSMAttributeObject.CreateUnityObject((DialogObject)null));
        AddAttribute(waitBubbleText, new BSMAttributeObject(false));
    }

    public override void OnBeginUpdate()
    {
        if (m_bubble != null)
            GameObject.Destroy(m_bubble);

        Event<StopMoveEvent>.Broadcast(new StopMoveEvent(), m_controler.gameObject);

        var bubblePrefab = World.common.dialogBubble;
        if (bubblePrefab == null)
        {
            Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
            return;
        }

        var dialogObj = GetUnityObjectAttribute<DialogObject>(textName);
        if(dialogObj == null)
        {
            Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
            return;
        }

        var bubbleObject = GameObject.Instantiate(bubblePrefab);
        bubbleObject.transform.parent = m_controler.transform;

        Event<StartDialogEvent>.Broadcast(new StartDialogEvent(m_controler.gameObject, dialogObj), bubbleObject);

        m_bubble = bubbleObject;

        if(GetBoolAttribute(waitBubbleText, false))
            Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
    }

    public override void Update()
    {
        bool wait = GetBoolAttribute(waitBubbleText, false);

        UpdateBubble(wait);

        if (!wait)
            Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
    }

    public override void UpdateAlways()
    {
        if(m_bubble != null)
            UpdateBubble(false);
    }

    void UpdateBubble(bool sendEndEvent)
    {
        if (m_bubble != null)
        {
            IsDialogEndedEvent data = new IsDialogEndedEvent();
            Event<IsDialogEndedEvent>.Broadcast(data, m_bubble);
            if(data.ended)
            {
                GameObject.Destroy(m_bubble);
                if(sendEndEvent)
                    Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
            }
        }
        else if(sendEndEvent)
            Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
    }
}

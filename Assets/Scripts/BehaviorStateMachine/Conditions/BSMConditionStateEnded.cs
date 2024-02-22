﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class BSMConditionStateEnded : BSMConditionBase
{
    SubscriberList m_subscriberList = new SubscriberList();

    bool m_ended = false;

    public override bool IsValid()
    {
        return m_ended;
    }

    public override void Init() 
    {
        if (m_subscriberList == null)
            m_subscriberList = new SubscriberList();

        m_subscriberList.Add(new Event<BSMStateEndedEvent>.LocalSubscriber(OnStateEnd, GetControler().gameObject));
    }

    public override void BeginUpdate() 
    {
        m_subscriberList.Subscribe();

        m_ended = false;
    }

    public override void EndUpdate() 
    {
        m_subscriberList.Unsubscribe();

        m_ended = false;
    }

    public override void OnDestroy() 
    {
        m_subscriberList.Unsubscribe();
    }

    void OnStateEnd(BSMStateEndedEvent e)
    {
        m_ended = true;
    }
}

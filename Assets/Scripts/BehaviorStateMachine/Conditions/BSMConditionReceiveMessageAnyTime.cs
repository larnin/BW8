using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BSMConditionReceiveMessageAnyTime : BSMConditionBase
{
    static string messageName = "Filter";
    static string countName = "Count";

    SubscriberList m_subscriberList = new SubscriberList();

    int m_nbMessageReceived = 0;

    public BSMConditionReceiveMessageAnyTime()
    {
        AddAttribute(messageName, new BSMAttributeObject(""));
        AddAttribute(countName, new BSMAttributeObject(0));
    }

    public override bool IsValid()
    {
        int count = GetIntAttribute(countName, 1);

        return m_nbMessageReceived >= count;
    }

    public override void Init()
    {
        if (m_subscriberList == null)
            m_subscriberList = new SubscriberList();

        m_subscriberList.Add(new Event<BSMSendMessageEvent>.LocalSubscriber(ReceiveMessage, GetControler().gameObject));
        m_subscriberList.Add(new Event<BSMSendMessageEvent>.Subscriber(ReceiveMessage));

        m_subscriberList.Subscribe();
    }

    public override void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void ReceiveMessage(BSMSendMessageEvent e)
    {
        string filter = GetStringAttribute(messageName, "");

        if (BSMConditionReceiveMessageThisState.IsMessageValid(e.message, filter))
            m_nbMessageReceived++;
    }
}
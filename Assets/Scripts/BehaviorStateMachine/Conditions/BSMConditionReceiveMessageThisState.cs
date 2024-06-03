using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BSMConditionReceiveMessageThisState : BSMConditionBase
{
    static string messageName = "Filter";
    static string countName = "Count";

    SubscriberList m_subscriberList = new SubscriberList();

    int m_nbMessageReceived = 0;

    public BSMConditionReceiveMessageThisState()
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
    }

    public override void BeginUpdate()
    {
        m_nbMessageReceived = 0;
        m_subscriberList.Subscribe();
    }

    public override void EndUpdate()
    {
        m_subscriberList.Unsubscribe();
        m_nbMessageReceived = 0;
    }

    void ReceiveMessage(BSMSendMessageEvent e)
    {
        string filter = GetStringAttribute(messageName, "");

        if (IsMessageValid(e.message, filter))
            m_nbMessageReceived++;
    }

    public static bool IsMessageValid(string message, string filter)
    {
        if(filter.StartsWith('*'))
        {
            if (filter.Length == 1)
                return true;
            if (filter.EndsWith("*"))
                return message.Contains(filter.Substring(1, filter.Length - 2));
            return message.EndsWith(filter.Substring(1));
        }
        if (filter.EndsWith('*'))
            return message.StartsWith(filter.Substring(0, filter.Length - 1));

        return message == filter;
    }
}

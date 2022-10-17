using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class QuestObjectiveItemPickup : QuestObjectiveBase
{
    QuestObjectiveObjectItemPickup m_questObject;
    List<int> m_nbPickup = new List<int>();

    SubscriberList m_subscriberList = new SubscriberList();

    public QuestObjectiveItemPickup(QuestObjectiveObjectItemPickup questObject) : base(questObject)
    {
        m_questObject = questObject;
    }

    protected override void OnStart()
    {
        m_nbPickup.Clear();
        for (int i = 0; i < m_questObject.m_itemList.Count(); i++)
            m_nbPickup.Add(0);

        m_subscriberList.Clear();
        m_subscriberList.Add(new Event<ItemPickedUpEvent>.Subscriber(OnPickupItem));
        m_subscriberList.Subscribe();
    }

    protected override void OnEnd(QuestCompletionState state)
    {
        m_subscriberList.Unsubscribe();
    }

    protected override QuestCompletionState OnUpdate()
    {
        for(int i = 0; i < m_questObject.m_itemList.Count; i++)
        {
            if (m_nbPickup[i] < m_questObject.m_itemList[i].m_count)
                return QuestCompletionState.Running;
        }

        return QuestCompletionState.Completed;
    }

    void OnPickupItem(ItemPickedUpEvent e)
    {
        for(int i = 0; i < m_questObject.m_itemList.Count; i++)
        {
            if (e.type == m_questObject.m_itemList[i].m_itemType)
            {
                m_nbPickup[i] += e.stack;
                break;
            }
        }
    }
}

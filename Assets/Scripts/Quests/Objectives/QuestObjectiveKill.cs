using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class QuestObjectiveKill : QuestObjectiveBase
{
    QuestObjectiveObjectKill m_questObject;
    List<int> m_kills = new List<int>();

    SubscriberList m_subscriberList = new SubscriberList();

    public QuestObjectiveKill(QuestObjectiveObjectKill questObject) : base(questObject)
    {
        m_questObject = questObject;
    }

    protected override void OnStart()
    {
        m_kills.Clear();
        foreach (var k in m_questObject.m_killList)
            m_kills.Add(0);

        m_subscriberList.Clear();
        m_subscriberList.Add(new Event<QuestKillEntityEvent>.Subscriber(OnKill));
        m_subscriberList.Subscribe();
    }

    protected override void OnEnd(QuestCompletionState state)
    {
        m_subscriberList.Unsubscribe();
    }

    protected override QuestCompletionState OnUpdate()
    {
        for(int i = 0; i < m_questObject.m_killList.Count; i++)
        {
            if (m_kills[i] < m_questObject.m_killList[i].m_count)
                return QuestCompletionState.Running;
        }

        return QuestCompletionState.Completed;
    }

    void OnKill(QuestKillEntityEvent e)
    {
        var nameID = e.entity.GetNameID();

        for(int i = 0; i < m_questObject.m_killList.Count; i++)
        {
            if (nameID == m_questObject.m_killList[i].m_entity)
            {
                m_kills[i]++;
                break;
            }
        }
    }
}

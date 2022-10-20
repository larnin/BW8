using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class QuestFailConditionEntityDeath : QuestFailConditionBase
{
    QuestFailConditionObjectEntityDeath m_questFail;
    List<int> m_deaths = new List<int>();

    SubscriberList m_subscriberList = new SubscriberList();

    public QuestFailConditionEntityDeath(QuestFailConditionObjectEntityDeath questFail)
    {
        m_questFail = questFail;
    }

    public override void Start()
    {
        m_deaths.Clear();
        foreach (var k in m_questFail.m_deathList)
            m_deaths.Add(0);

        m_subscriberList.Clear();
        m_subscriberList.Add(new Event<QuestKillEntityEvent>.Subscriber(OnDeath));
    }

    public override void End(QuestCompletionState state)
    {
        m_subscriberList.Unsubscribe();
    }

    public override QuestCompletionState Update()
    {
        for (int i = 0; i < m_questFail.m_deathList.Count; i++)
        {
            if (m_deaths[i] < m_questFail.m_deathList[i].m_count)
                return QuestCompletionState.Running;
        }

        return QuestCompletionState.Failed;
    }

    void OnDeath(QuestKillEntityEvent e)
    {
        var nameID = e.entity.GetNameID();

        for (int i = 0; i < m_questFail.m_deathList.Count; i++)
        {
            if (nameID == m_questFail.m_deathList[i].m_entity)
            {
                m_deaths[i]++;
                break;
            }
        }
    }
}
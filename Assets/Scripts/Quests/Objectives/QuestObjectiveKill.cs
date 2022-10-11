using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class QuestObjectiveKill : QuestObjectiveBase
{
    QuestObjectiveObjectKill m_questObject;
    int m_nbKill;

    SubscriberList m_subscriberList = new SubscriberList();

    public QuestObjectiveKill(QuestObjectiveObjectKill questObject)
    {
        m_questObject = questObject;
    }

    public override void OnCompletion()
    {
        OnEnd();
    }

    public override void OnFail()
    {
        OnEnd();
    }

    public override void OnStart()
    {
        m_nbKill = 0;

        m_subscriberList.Add(new Event<QuestKillEntityEvent>.Subscriber(OnKill));
        m_subscriberList.Subscribe();
    }

    void OnEnd()
    {
        m_subscriberList.Unsubscribe();
    }

    public override QuestCompletionState Update()
    {
        if (m_nbKill >= m_questObject.m_count)
            return QuestCompletionState.Completed;
        return QuestCompletionState.Running;
    }

    void OnKill(QuestKillEntityEvent e)
    {
        if (e.entity.GetNameID() == m_questObject.m_entity)
            m_nbKill++;
    }
}

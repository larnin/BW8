using NLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class QuestObjectiveTalk : QuestObjectiveBase
{
    QuestObjectiveObjectTalk m_questObject;
    QuestEntity m_questEntity;
    bool m_completed = false;

    SubscriberList m_subscriberList = new SubscriberList();

    public QuestObjectiveTalk(QuestObjectiveObjectTalk questObject) : base(questObject)
    {
        m_questObject = questObject;
    }

    protected override void OnStart() 
    {
        m_subscriberList.Clear();
        m_subscriberList.Add(new Event<QuestStartTalkEvent>.Subscriber(OnStartTalk));
        m_subscriberList.Subscribe();
    }

    protected override void OnEnd(QuestCompletionState state)
    {
        DisconnectEntity();
        m_completed = false;

        m_subscriberList.Unsubscribe();
    }

    protected override QuestCompletionState OnUpdate()
    {
        if (m_questEntity == null)
            ConnectNewEntity();

        if (m_completed)
            return QuestCompletionState.Completed;

        return QuestCompletionState.Running;
    }

    void ConnectNewEntity()
    {
        GetFirstQuestEntityEvent getEntity = new GetFirstQuestEntityEvent(m_questObject.m_targetName);
        Event<GetFirstQuestEntityEvent>.Broadcast(getEntity);

        if (getEntity.entity == null)
            return;

        m_questEntity = getEntity.entity;

        m_questEntity.SetDialogInteraction(m_questObject.m_text, m_questObject.m_dialog);
    }

    void DisconnectEntity()
    {
        if(m_questEntity != null)
            m_questEntity.ResetInteraction();

        m_questEntity = null;
    }

    void OnStartTalk(QuestStartTalkEvent e)
    {
        if(e.entity == m_questEntity)
            m_completed = true;
    }
}
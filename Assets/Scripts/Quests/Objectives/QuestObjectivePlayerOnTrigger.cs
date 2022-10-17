using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class QuestObjectivePlayerOnTrigger : QuestObjectiveBase
{
    QuestObjectiveObjectPlayerOnTrigger m_questObject;

    public QuestObjectivePlayerOnTrigger(QuestObjectiveObjectPlayerOnTrigger questObject) : base(questObject)
    {
        m_questObject = questObject;
    }

    protected override QuestCompletionState OnUpdate()
    {
        GetFirstQuestEntityEvent getEntity = new GetFirstQuestEntityEvent(m_questObject.m_triggerName);
        Event<GetFirstQuestEntityEvent>.Broadcast(getEntity);

        if (getEntity.entity == null)
            return QuestCompletionState.Running;

        var collider = getEntity.entity.GetComponent<Collider2D>();
        if (collider == null)
            return QuestCompletionState.Running;

        GetPlayerPositionEvent getPlayerPos = new GetPlayerPositionEvent();
        Event<GetPlayerPositionEvent>.Broadcast(getPlayerPos);

        if (collider.OverlapPoint(getPlayerPos.pos))
            return QuestCompletionState.Completed;

        return QuestCompletionState.Running;
    }
}

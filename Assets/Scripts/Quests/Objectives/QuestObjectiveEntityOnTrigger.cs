using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class QuestObjectiveEntityOnTrigger : QuestObjectiveBase
{
    QuestObjectiveObjectEntityOnTrigger m_questObject;

    public QuestObjectiveEntityOnTrigger(QuestObjectiveObjectEntityOnTrigger questObject)
    {
        m_questObject = questObject;
    }

    public override void OnCompletion() { }

    public override void OnFail() { }

    public override void OnStart() { }

    public override QuestCompletionState Update()
    {
        GetFirstQuestEntityEvent getEntity = new GetFirstQuestEntityEvent(m_questObject.m_triggerName);
        Event<GetFirstQuestEntityEvent>.Broadcast(getEntity);

        if (getEntity.entity == null)
            return QuestCompletionState.Running;

        var collider = getEntity.entity.GetComponent<Collider2D>();
        if (collider == null)
            return QuestCompletionState.Running;

        GetAllQuestEntityEvent getEntities = new GetAllQuestEntityEvent(m_questObject.m_entityName);
        Event<GetAllQuestEntityEvent>.Broadcast(getEntities);

        int nbOnTrigger = 0;
        foreach(var e in getEntities.entities)
        {
            Vector2 pos = e.transform.position;
            if (collider.OverlapPoint(pos))
                nbOnTrigger++;
        }

        if (nbOnTrigger >= m_questObject.m_count)
            return QuestCompletionState.Completed;

        return QuestCompletionState.Running;
    }
}

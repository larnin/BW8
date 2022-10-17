using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class QuestFailConditionTimer : QuestFailConditionBase
{
    QuestFailConditionObjectTimer m_questFail;
    float m_timer = 0;

    public QuestFailConditionTimer(QuestFailConditionObjectTimer questFail)
    {
        m_questFail = questFail;
    }

    public override void Start()
    {
        m_timer = 0;
    }

    public override QuestCompletionState Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer >= m_questFail.m_timer)
            return QuestCompletionState.Failed;

        return QuestCompletionState.Running;
    }
}

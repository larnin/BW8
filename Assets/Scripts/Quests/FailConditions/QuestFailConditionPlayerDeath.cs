using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class QuestFailConditionPlayerDeath : QuestFailConditionBase
{
    QuestFailConditionObjectPlayerDeath m_questFail;

    public QuestFailConditionPlayerDeath(QuestFailConditionObjectPlayerDeath questFail)
    {
        m_questFail = questFail;
    }

    public override QuestCompletionState Update()
    {
        GetPlayerLifeEvent lifeEvent = new GetPlayerLifeEvent();
        lifeEvent.life = 0;
        Event<GetPlayerLifeEvent>.Broadcast(lifeEvent);

        if (lifeEvent.life <= 0)
            return QuestCompletionState.Failed;
        return QuestCompletionState.Running;
    }
}

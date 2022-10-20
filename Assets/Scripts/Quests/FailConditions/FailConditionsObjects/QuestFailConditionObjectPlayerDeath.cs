using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class QuestFailConditionObjectPlayerDeath : QuestFailConditionObjectBase
{
    public override string GetFailConditionName()
    {
        return "Player death";
    }

    public override QuestFailConditionBase MakeFailCondition()
    {
        return new QuestFailConditionPlayerDeath(this);
    }
}
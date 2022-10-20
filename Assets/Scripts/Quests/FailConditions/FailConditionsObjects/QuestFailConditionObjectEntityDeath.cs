using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class QuestFailConditionObjectEntityDeath : QuestFailConditionObjectBase
{
    [SerializeField] public List<QuestObjectiveOneKill> m_deathList = new List<QuestObjectiveOneKill>();

    public override string GetFailConditionName()
    {
        return "Entity death";
    }

    public override QuestFailConditionBase MakeFailCondition()
    {
        return new QuestFailConditionEntityDeath(this);
    }

    protected override void OnInspectorGUI()
    {
#if UNITY_EDITOR
        if (m_deathList == null)
            m_deathList = new List<QuestObjectiveOneKill>();

        QuestObjectiveObjectBase.DrawEntityListGUI(m_deathList);
#endif
    }
}

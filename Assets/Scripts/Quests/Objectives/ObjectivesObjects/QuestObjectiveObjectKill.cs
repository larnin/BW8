using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class QuestObjectiveOneKill
{
    public string m_entity;
    public int m_count;
}


public class QuestObjectiveObjectKill : QuestObjectiveObjectBase
{
    [SerializeField] public List<QuestObjectiveOneKill> m_killList = new List<QuestObjectiveOneKill>(); 

    public override string GetObjectiveName()
    {
        return "Kill";
    }

    public override QuestObjectiveBase MakeObjective()
    {
        return new QuestObjectiveKill(this);
    }

    protected override void OnInspectorGUI()
    {
#if UNITY_EDITOR
        if (m_killList == null)
            m_killList = new List<QuestObjectiveOneKill>();

        DrawEntityListGUI(m_killList);
#endif
    }
}

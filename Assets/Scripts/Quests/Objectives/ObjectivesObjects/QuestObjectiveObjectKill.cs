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

    public override string GetObectiveName()
    {
        return "Kill";
    }

    public override QuestObjectiveBase MakeObjective()
    {
        return new QuestObjectiveKill(this);
    }

    protected override void OnSpecificInspectorGUI()
    {
#if UNITY_EDITOR
        if (m_killList == null)
            m_killList = new List<QuestObjectiveOneKill>();

        int removeIndex = -1;

        for(int i = 0; i < m_killList.Count; i++)
        {
            GUILayout.BeginVertical(visualBoxType);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            QuestObjectiveOneKill oneKill = m_killList[i];
            GUILayout.BeginHorizontal();
            GUILayout.Label("Entity", GUILayout.MaxWidth(100));
            oneKill.m_entity = GUILayout.TextField(oneKill.m_entity);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Count", GUILayout.MaxWidth(100));
            oneKill.m_count = EditorGUILayout.IntField(oneKill.m_count);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(5);
            if (GUILayout.Button("X", GUILayout.MaxWidth(30)))
                removeIndex = i;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        if (GUILayout.Button("Add"))
            m_killList.Add(new QuestObjectiveOneKill());
        if (removeIndex >= 0)
            m_killList.RemoveAt(removeIndex);
#endif
    }
}

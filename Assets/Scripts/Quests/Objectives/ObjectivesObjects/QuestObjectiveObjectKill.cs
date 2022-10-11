using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class QuestObjectiveObjectKill : QuestObjectiveObjectBase
{
    [SerializeField] public string m_entity;
    [SerializeField] public int m_count;

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
        GUILayout.BeginHorizontal();
        GUILayout.Label("Entity", GUILayout.MaxWidth(100));
        m_entity = GUILayout.TextField(m_entity);
        GUILayout.EndHorizontal(); 
        GUILayout.BeginHorizontal();
        GUILayout.Label("Count", GUILayout.MaxWidth(100));
        m_count = EditorGUILayout.IntField(m_count);
        GUILayout.EndHorizontal();
#endif
    }
}

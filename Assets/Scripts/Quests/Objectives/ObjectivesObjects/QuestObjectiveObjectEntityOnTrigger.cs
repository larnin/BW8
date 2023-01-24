using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuestObjectiveObjectEntityOnTrigger : QuestObjectiveObjectBase
{
    [SerializeField] public string m_triggerName;
    [SerializeField] public string m_entityName;
    [SerializeField] public int m_count = 1;

    public override string GetObjectiveName()
    {
        return "Entity on trigger";
    }

    public override QuestObjectiveBase MakeObjective()
    {
        return new QuestObjectiveEntityOnTrigger(this);
    }

    protected override void OnInspectorGUI()
    {
#if UNITY_EDITOR
        GUILayout.BeginHorizontal();
        GUILayout.Label("Trigger Name", GUILayout.MaxWidth(100));
        m_triggerName = GUILayout.TextField(m_triggerName);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Entity Name", GUILayout.MaxWidth(100));
        m_entityName = GUILayout.TextField(m_entityName);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Count", GUILayout.MaxWidth(100));
        m_count = EditorGUILayout.IntField(m_count);
        GUILayout.EndHorizontal();
#endif
    }
}
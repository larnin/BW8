using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuestObjectiveObjectPlayerOnTrigger : QuestObjectiveObjectBase
{
    [SerializeField] public string m_triggerName;

    public override string GetObjectiveName()
    {
        return "Player on trigger";
    }

    public override QuestObjectiveBase MakeObjective()
    {
        return new QuestObjectivePlayerOnTrigger(this);
    }

    protected override void OnInspectorGUI()
    {
#if UNITY_EDITOR
        GUILayout.BeginHorizontal();
        GUILayout.Label("Trigger Name", GUILayout.MaxWidth(100));
        m_triggerName = GUILayout.TextField(m_triggerName);
        GUILayout.EndHorizontal();
#endif
    }
}

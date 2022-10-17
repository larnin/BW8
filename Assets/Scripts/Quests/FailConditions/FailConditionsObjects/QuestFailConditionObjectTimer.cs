using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuestFailConditionObjectTimer : QuestFailConditionObjectBase
{
    [SerializeField] public float m_timer;

    public override string GetFailConditionName()
    {
        return "Timer";
    }

    public override QuestFailConditionBase MakeFailCondition()
    {
        return new QuestFailConditionTimer(this);
    }

    protected override void OnInspectorGUI()
    {
#if UNITY_EDITOR
        GUILayout.BeginHorizontal();
        GUILayout.Label("Timer", GUILayout.MaxWidth(100));
        m_timer = EditorGUILayout.FloatField(m_timer);
        GUILayout.EndHorizontal();
#endif
    }
}
using NLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuestObjectiveObjectTalk : QuestObjectiveObjectBase
{
    [SerializeField] public LocText m_text;
    [SerializeField] public DialogObject m_dialog;
    [SerializeField] public string m_targetName;

    public override string GetObjectiveName()
    {
        return "Talk";
    }

    public override QuestObjectiveBase MakeObjective()
    {
        return new QuestObjectiveTalk(this);
    }

    protected override void OnInspectorGUI()
    {
#if UNITY_EDITOR
        GUILayout.BeginHorizontal();
        GUILayout.Label("Interact text", GUILayout.MaxWidth(100));
        GUILayout.BeginVertical();
        if (m_text == null)
            m_text = new LocText();
        m_text.DrawInspectorGUI();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Dialog", GUILayout.MaxWidth(100));
        m_dialog = EditorGUILayout.ObjectField(m_dialog, typeof(DialogObject), false) as DialogObject;
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Target", GUILayout.MaxWidth(100));
        m_targetName = GUILayout.TextField(m_targetName);
        GUILayout.EndHorizontal();
#endif
    }
}

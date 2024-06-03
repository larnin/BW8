using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[Serializable]
public class QuestCondition
{
    [HideInInspector]
    [SerializeField] int m_id = QuestList.invalidID;

    [HideInInspector]
    [SerializeField] string m_questID = "";

    [HideInInspector]
    [SerializeField] bool m_hideObjective = true;

    [HideInInspector]
    [SerializeField] int m_questObjective = -1;

    [SerializeField] QuestSystem.ObjectiveState m_state;

#if UNITY_EDITOR
    Rect m_setupRect;
#endif

    public void DrawInspectorGUI()
    {
        OnInspectorGUI();
    }

    [OnInspectorGUI]
    private void OnInspectorGUI()
    {
#if UNITY_EDITOR
        if (m_id == QuestList.invalidID)
        {
            EditorGUILayout.HelpBox("ID not set", MessageType.Warning);

            GUILayout.BeginHorizontal();
        }
        else
        {
            var quest = QuestList.GetQuest(m_id);
            if(quest == null)
                EditorGUILayout.HelpBox("Invalid ID " + m_questID + " (" + m_id + ")", MessageType.Error);
            else m_questID = quest.questName;

            GUILayout.BeginHorizontal();

            GUILayout.Label("QuestID: " + m_questID + " (" + m_id + ")");
        }

        if (GUILayout.Button("Setup", GUILayout.Width(100)))
        {
            PopupWindow.Show(m_setupRect, new QuestSelectionPopup(this));
        }
        if (Event.current.type == EventType.Repaint)
            m_setupRect = GUILayoutUtility.GetLastRect();

        GUILayout.EndHorizontal();

        if (m_hideObjective)
        {
            m_hideObjective = !GUILayout.Toggle(!m_hideObjective, "SpecificObjective");
            m_questObjective = -1;
        }
        else
        {
            GUILayout.BeginHorizontal();
            m_hideObjective = !GUILayout.Toggle(!m_hideObjective, "", GUILayout.MaxWidth(40));
            m_questObjective = EditorGUILayout.IntField(m_questObjective, "Objective");
            if (m_questObjective < 0)
                m_questObjective = 0;
            GUILayout.EndHorizontal();
        }
#endif
    }

    public QuestObject GetQuest()
    {
        return QuestList.GetQuest(m_id);
    }

    public int GetQuestID()
    {
        return m_id;
    }

    public void SetQuest(int id)
    {
#if UNITY_EDITOR
        m_id = id;
        var quest = QuestList.GetQuest(m_id);
        if (quest == null)
            m_questID = quest.questName;
#endif
    }

    public bool IsValid()
    {
        if(m_questObjective < 0)
        {
            IsQuestActiveEvent questActive = new IsQuestActiveEvent(m_id);
            Event<IsQuestActiveEvent>.Broadcast(questActive);

            IsQuestCompletedEvent questCompleted = new IsQuestCompletedEvent(m_id);
            Event<IsQuestCompletedEvent>.Broadcast(questCompleted);

            if (m_state == QuestSystem.ObjectiveState.Completed && questCompleted.completed)
                return true;

            if (m_state == QuestSystem.ObjectiveState.Started && questActive.active)
                return true;

            if (m_state == QuestSystem.ObjectiveState.NotStarted && !questActive.active && !questCompleted.completed)
                return true;

            return false;
        }
        else
        {
            IsQuestObjectiveCompletedEvent questData = new IsQuestObjectiveCompletedEvent(m_id, m_questObjective);
            Event<IsQuestObjectiveCompletedEvent>.Broadcast(questData);

            return questData.state == m_state;
        }
    }
}

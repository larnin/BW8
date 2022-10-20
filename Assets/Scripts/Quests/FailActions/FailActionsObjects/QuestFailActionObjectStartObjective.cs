using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuestFailActionObjectStartObjective : QuestFailActionObjectBase
{
    [SerializeField] public int m_questID = QuestObject.invalidQuestID;
    [SerializeField] public int m_objectiveIndex = 0;

    public override string GetFailActionName()
    {
        return "Start Objective";
    }

    public override QuestFailActionBase MakeFailAction()
    {
        return new QuestFailActionStartObjective(this);
    }

    protected override void OnInspectorGUI()
    {
#if UNITY_EDITOR
        var quest = QuestList.GetQuest(m_questID);
        if (quest == null && m_questID != QuestObject.invalidQuestID)
            EditorGUILayout.HelpBox("A quest ID defined is not valid or have been removed", MessageType.Error);

        if (quest != null && (m_objectiveIndex < 0 || m_objectiveIndex >= quest.GetObjectiveNb()))
            EditorGUILayout.HelpBox("This quest only have " + quest.GetObjectiveNb() + " objectives", MessageType.Error);

        var names = QuestList.GetAllQuestsNames();
        names.Insert(0, "None");
        int index = QuestList.GetQuestIndex(m_questID);
        index++;

        int newIndex = EditorGUILayout.Popup("Next Quest", index, names.ToArray());
        if (newIndex != index)
        {
            if (newIndex <= 0)
                m_questID = QuestObject.invalidQuestID;
            else
            {
                var parentQuest = QuestList.GetQuestFromIndex(newIndex - 1);
                if (parentQuest == null)
                    m_questID = QuestObject.invalidQuestID;
                else m_questID = parentQuest.questID;
            }
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Objective index", GUILayout.MaxWidth(100));
        m_objectiveIndex = EditorGUILayout.IntField(m_objectiveIndex);
        if (m_objectiveIndex < 0)
            m_objectiveIndex = 0;
        GUILayout.EndHorizontal();
#endif
    }
}

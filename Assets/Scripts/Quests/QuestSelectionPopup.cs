using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NLocalization;

#if UNITY_EDITOR
using UnityEditor;
class QuestSelectionPopup : PopupWindowContent
{
    string m_filter = "";
    QuestCondition m_condition = null;

    Vector2 m_scrollPos = Vector2.zero;

    public QuestSelectionPopup(QuestCondition cond)
    {
        m_condition = cond;
    }

    public override void OnGUI(Rect rect)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Filter:", GUILayout.Width(40));
        m_filter = GUILayout.TextField(m_filter);
        GUILayout.EndHorizontal();

        m_scrollPos = GUILayout.BeginScrollView(m_scrollPos);
        int nbQuest = QuestList.GetQuestNb();
        for (int i = 0; i < nbQuest; i++)
        {
            var quest = QuestList.GetQuestFromIndex(i);

            int id = quest.questID;
            string nameID = quest.questName;
            if (!Loc.ProcessFilter(nameID, m_filter))
                continue;

            if (GUILayout.Button(nameID))
            {
                m_condition.SetQuest(id);
                editorWindow.Close();
            }
        }
        GUILayout.EndScrollView();
    }
}

#endif
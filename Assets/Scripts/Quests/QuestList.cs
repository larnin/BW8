using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class QuestList
{
    static string m_path = "";
    static Dictionary<int,QuestObject> m_quests = null;
    static int m_nextQuestID = 0;

    static void Load(string path = "Quests")
    {
        if (m_quests == null)
            m_quests = new Dictionary<int, QuestObject>();

        m_path = path;
        m_quests.Clear();

        var quests = Resources.LoadAll<ScriptableObject>(path);

        foreach (var i in quests)
        {
            QuestObject quest = i as QuestObject;
            if (quest != null)
            {
                m_quests.Add(quest.questID, quest);
                if (quest.questID >= m_nextQuestID)
                    m_nextQuestID = quest.questID + 1;
            }
        }
    }

    public static int GetQuestNb()
    {
        if (m_quests == null)
            Load();

        return m_quests.Count;
    }

    public static List<String> GetAllQuestsNames()
    {
        if (m_quests == null)
            Load();

        List<string> quests = new List<string>();

        foreach(var q in m_quests)
        {
            quests.Add(q.Value.questName);
        }

        return quests;
    }

    public static List<int> GetAllQuestsID()
    {
        if (m_quests == null)
            Load();

        List<int> quests = new List<int>();

        foreach (var q in m_quests)
        {
            quests.Add(q.Value.questID);
        }

        return quests;
    }

    public static QuestObject GetQuestFromIndex(int index)
    {
        if (m_quests == null)
            Load();

        if (m_quests.Count <= index || index < 0)
            return null;

        return m_quests.ElementAt(index).Value;
    }

    public static int GetQuestIndex(int questID)
    {
        if (m_quests == null)
            Load();

        for (int i = 0; i < m_quests.Count; i++)
        {
            var quest = m_quests.ElementAt(i);
            if (quest.Value.questID == questID)
                return i;
        }
        return -1;
    }

    public static int GetQuestIndex(string questName)
    {
        QuestObject quest = GetQuest(questName);
        if (quest == null)
            return -1;
        return GetQuestIndex(quest.questID);
    }

    public static bool HaveQuest(int questID)
    {
        return GetQuest(questID) != null;
    }

    public static bool HaveQuest(string questName)
    {
        return GetQuest(questName) != null;
    }

    public static QuestObject GetQuest(int questID)
    {
        if (m_quests == null)
            Load();

        QuestObject quest = null;
        m_quests.TryGetValue(questID, out quest);

        return quest;
    }

    public static QuestObject GetQuest(string questName)
    {
        if (m_quests == null)
            Load();

        foreach (var quest in m_quests)
        {
            if (quest.Value.questName == questName)
                return quest.Value;
        }
        return null;
    }

    public static int AddQuest(string questName)
    {
#if UNITY_EDITOR
        if (m_quests == null)
            Load();

        int id = m_nextQuestID;
        m_nextQuestID++;

        QuestObject quest = ScriptableObjectEx.CreateAsset<QuestObject>(m_path, questName);
        quest.questName = questName;
        quest.questID = id;
        EditorUtility.SetDirty(quest);

        m_quests.Add(id, quest);

        AssetDatabase.SaveAssets();
        return id;
#else
        return QuestObject.invalidQuestID;
#endif
    }
}

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class QuestObject : SerializedScriptableObject
{
    public const int invalidQuestID = -1;

    [HideInInspector] [SerializeField] string m_questName;
    public string questName { get { return m_questName; } set { m_questName = value; } }

    [HideInInspector] [SerializeField] int m_questID;
    public int questID { get { return m_questID; } set { m_questID = value; } }

    [HideInInspector] [SerializeField] int m_parentQuestID;
    public int parentQuestID { get { return m_parentQuestID; } set { m_parentQuestID = value; } }

    [HideInInspector] [SerializeField] List<QuestObjectiveObjectBase> m_objectives = new List<QuestObjectiveObjectBase>();
    public int GetObjectiveNb() 
    {
        if (m_objectives == null)
            return 0;
        return m_objectives.Count; 
    }
    
    public QuestObjectiveObjectBase GetObjective(int index)
    {
        if (m_objectives == null)
            return null;
        
        if (index < 0 || index >= m_objectives.Count)
            return null;
        return m_objectives[index];
    }

#if UNITY_EDITOR
    public void AddObjectiveEditor(QuestObjectiveObjectBase objective)
    {
        if (m_objectives == null)
            m_objectives = new List<QuestObjectiveObjectBase>();
        m_objectives.Add(objective);
    }

    public void RemoveObjectiveEditor(int index)
    {
        if (m_objectives == null)
            return;

        if (index < 0 || index >= m_objectives.Count)
            return;

        m_objectives.RemoveAt(index);
    }

    public void MoveObjectiveEditor(int fromIndex, int toIndex)
    {
        if (m_objectives == null)
            return;

        if (fromIndex < 0 || fromIndex >= m_objectives.Count)
            return;
        if (toIndex < 0 || toIndex >= m_objectives.Count)
            return;

        var obj = m_objectives[fromIndex];

        m_objectives.RemoveAt(fromIndex);
        m_objectives.Insert(toIndex, obj);
    }
#endif
}
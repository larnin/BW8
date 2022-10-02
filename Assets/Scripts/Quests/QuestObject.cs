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



}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class QuestObjectiveObjectBase
{
    [SerializeField] public List<QuestFailConditionObjectBase> m_failConditions = new List<QuestFailConditionObjectBase>();

#if UNITY_EDITOR
    bool m_failConditionListOpen = false;
    List<bool> m_failConditionsOpen = new List<bool>();
#endif

    protected const string visualBoxType = "box";

    public abstract string GetObjectiveName();

    public abstract QuestObjectiveBase MakeObjective();

    public void InspectorGUI()
    {
        DrawFailConditions();
        
        OnInspectorGUI();
    }

    protected virtual void OnInspectorGUI() { }

    public static void DrawItemListGUI(List<QuestObjectiveOneItem> itemList)
    {
#if UNITY_EDITOR
        int removeIndex = -1;

        var itemNames = Enum.GetNames(typeof(ItemType));

        for (int i = 0; i < itemList.Count; i++)
        {
            GUILayout.BeginVertical(visualBoxType);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            var oneItem = itemList[i];
            GUILayout.BeginHorizontal();
            GUILayout.Label("Item", GUILayout.MaxWidth(100));
            oneItem.m_itemType = (ItemType)EditorGUILayout.Popup((int)oneItem.m_itemType, itemNames);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Count", GUILayout.MaxWidth(100));
            oneItem.m_count = EditorGUILayout.IntField(oneItem.m_count);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(5);
            if (GUILayout.Button("X", GUILayout.MaxWidth(30)))
                removeIndex = i;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        if (GUILayout.Button("Add"))
            itemList.Add(new QuestObjectiveOneItem());
        if (removeIndex >= 0)
            itemList.RemoveAt(removeIndex);
#endif
    }

    public static void DrawEntityListGUI(List<QuestObjectiveOneKill> entityList)
    {
#if UNITY_EDITOR
        int removeIndex = -1;

        for (int i = 0; i < entityList.Count; i++)
        {
            GUILayout.BeginVertical(visualBoxType);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            QuestObjectiveOneKill oneKill = entityList[i];
            GUILayout.BeginHorizontal();
            GUILayout.Label("Entity", GUILayout.MaxWidth(100));
            oneKill.m_entity = GUILayout.TextField(oneKill.m_entity);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Count", GUILayout.MaxWidth(100));
            oneKill.m_count = EditorGUILayout.IntField(oneKill.m_count);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(5);
            if (GUILayout.Button("X", GUILayout.MaxWidth(30)))
                removeIndex = i;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        if (GUILayout.Button("Add"))
            entityList.Add(new QuestObjectiveOneKill());
        if (removeIndex >= 0)
            entityList.RemoveAt(removeIndex);
#endif
    }

    void DrawFailConditions()
    {
#if UNITY_EDITOR
        if (m_failConditions == null || m_failConditions.Count == 0)
            return;

        GUILayout.Space(5);
        m_failConditionListOpen = EditorGUILayout.BeginFoldoutHeaderGroup(m_failConditionListOpen, "Fail conditions");
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (m_failConditionListOpen)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            for (int i = 0; i < m_failConditions.Count; i++)
            {
                DrawOneFailCondition(i);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
#endif
    }

    void DrawOneFailCondition(int failConditionIndex)
    {
#if UNITY_EDITOR
        if (m_failConditionsOpen == null)
            m_failConditionsOpen = new List<bool>();
        while (m_failConditionsOpen.Count <= failConditionIndex)
            m_failConditionsOpen.Add(false);

        var failCondition = m_failConditions[failConditionIndex];

        GUILayout.Space(5);

        m_failConditionsOpen[failConditionIndex] = EditorGUILayout.BeginFoldoutHeaderGroup(m_failConditionsOpen[failConditionIndex], failCondition.GetFailConditionName(), null, 
            (rect)=> { ShowOneFailConditionContextMenu(rect, failConditionIndex); });
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (!m_failConditionsOpen[failConditionIndex])
            return;

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginVertical();
        failCondition.InspectorGUI();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
#endif
    }

    void ShowOneFailConditionContextMenu(Rect position, int failConditionIndex)
    {
#if UNITY_EDITOR
        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("Delete"), false, () => { RemoveFailCondition(failConditionIndex); });
        menu.DropDown(position);
#endif
    }

    void RemoveFailCondition(int failConditionIndex)
    {
        if (failConditionIndex < 0 || failConditionIndex >= m_failConditions.Count)
            return;

        m_failConditions.RemoveAt(failConditionIndex);
    }
}

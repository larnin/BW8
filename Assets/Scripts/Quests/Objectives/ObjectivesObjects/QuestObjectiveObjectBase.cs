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
    [SerializeField] public QuestFailActionObjectBase m_failAction = null;

#if UNITY_EDITOR
    bool m_failConditionListOpen = false;
    List<bool> m_failConditionsOpen = new List<bool>();
    bool m_failActionOpen = false;
#endif

    protected const string visualBoxType = "box";

    public abstract string GetObjectiveName();

    public abstract QuestObjectiveBase MakeObjective();

    public void InspectorGUI()
    {
        DrawFailConditions();
        DrawFailAction();
        
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

    void DrawFailAction()
    {
#if UNITY_EDITOR
        if (m_failConditions == null || m_failConditions.Count == 0)
            return;

        string failActionName = "None";
        if (m_failAction != null)
            failActionName = m_failAction.GetFailActionName();

        GUILayout.Space(5);
        m_failActionOpen = EditorGUILayout.BeginFoldoutHeaderGroup(m_failActionOpen, "Fail action - " + failActionName, null, ShowFailActionContextMenu);
        EditorGUILayout.EndFoldoutHeaderGroup();

        if(m_failActionOpen && m_failAction != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            m_failAction.InspectorGUI();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
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

    void ShowFailActionContextMenu(Rect position)
    {
#if UNITY_EDITOR

        const string baseMenu = "Change action";

        var allObjectiveTypes = AppDomain.CurrentDomain.GetAssemblies()
         .SelectMany(assembly => assembly.GetTypes())
         .Where(type => type.IsSubclassOf(typeof(QuestFailActionObjectBase))).ToArray();

        var menu = new GenericMenu();
        if (m_failAction != null)
            menu.AddItem(new GUIContent("Remove action"), false, RemoveFailAction);
        foreach (var t in allObjectiveTypes)
        {
            const string baseString = "QuestFailActionObject";
            string name = t.Name;
            if (name.StartsWith(baseString))
                name = name.Substring(baseString.Length);
            menu.AddItem(new GUIContent(baseMenu + "/" + name), false, () => { NewFailAction(t); });
        }
        menu.DropDown(position);
#endif
    }

    void RemoveFailAction()
    {
        m_failAction = null;
    }

    void NewFailAction(Type failActionType)
    {
        m_failAction = Activator.CreateInstance(failActionType) as QuestFailActionObjectBase;
    }
}

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
    protected const string visualBoxType = "box";

    public abstract string GetObectiveName();

    public abstract QuestObjectiveBase MakeObjective();

    public void OnInspectorGUI()
    {
        OnSpecificInspectorGUI();
    }

    protected abstract void OnSpecificInspectorGUI();

    protected void DrawItemListGUI(List<QuestObjectiveOneItem> itemList)
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
}

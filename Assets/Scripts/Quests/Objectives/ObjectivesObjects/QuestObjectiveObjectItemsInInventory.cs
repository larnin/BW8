using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class QuestObjectiveObjectItemsInInventory : QuestObjectiveObjectBase
{
    [SerializeField] public List<QuestObjectiveOneItem> m_itemList = new List<QuestObjectiveOneItem>();
    [SerializeField] public bool m_removeOnCompletion = false;

    public override string GetObectiveName()
    {
        return "Items in inventory";
    }

    public override QuestObjectiveBase MakeObjective()
    {
        return new QuestObjectiveItemInInventory(this);
    }

    protected override void OnSpecificInspectorGUI()
    {
#if UNITY_EDITOR
        if (m_itemList == null)
            m_itemList = new List<QuestObjectiveOneItem>();

        DrawItemListGUI(m_itemList);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Remove On Completion");
        m_removeOnCompletion = GUILayout.Toggle(m_removeOnCompletion, m_removeOnCompletion ? "Enabled" : "Disabled");
        GUILayout.EndHorizontal();
#endif
    }
}

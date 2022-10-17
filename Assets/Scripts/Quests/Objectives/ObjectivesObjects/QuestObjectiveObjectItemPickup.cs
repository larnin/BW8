using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class QuestObjectiveOneItem
{
    public ItemType m_itemType;
    public int m_count;
}

public class QuestObjectiveObjectItemPickup : QuestObjectiveObjectBase
{
    [SerializeField] public List<QuestObjectiveOneItem> m_itemList = new List<QuestObjectiveOneItem>();

    public override string GetObjectiveName()
    {
        return "Item pickup";
    }

    public override QuestObjectiveBase MakeObjective()
    {
        return new QuestObjectiveItemPickup(this);
    }

    protected override void OnInspectorGUI()
    {
#if UNITY_EDITOR
        if (m_itemList == null)
            m_itemList = new List<QuestObjectiveOneItem>();

        DrawItemListGUI(m_itemList);
#endif
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class QuestObjectiveObjectGiveItem : QuestObjectiveObjectBase
{
    [SerializeField] public List<QuestObjectiveOneItem> m_itemList = new List<QuestObjectiveOneItem>();

    public override string GetObjectiveName()
    {
        return "Give item";
    }

    public override QuestObjectiveBase MakeObjective()
    {
        return new QuestObjectiveGiveItem(this);
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

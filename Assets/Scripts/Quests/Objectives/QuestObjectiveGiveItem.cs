using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class QuestObjectiveGiveItem : QuestObjectiveBase
{
    QuestObjectiveObjectGiveItem m_questObject;
    
    public QuestObjectiveGiveItem(QuestObjectiveObjectGiveItem questObject) : base(questObject)
    {
        m_questObject = questObject;
    }

    protected override QuestCompletionState OnUpdate()
    {
        foreach(var o in m_questObject.m_itemList)
        {
            AddInventoryItemEvent addItem = new AddInventoryItemEvent(o.m_itemType, o.m_count);
            Event<AddInventoryItemEvent>.Broadcast(addItem);
        }

        return QuestCompletionState.Completed;
    }
}

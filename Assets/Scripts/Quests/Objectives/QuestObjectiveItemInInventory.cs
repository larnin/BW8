using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class QuestObjectiveItemInInventory : QuestObjectiveBase
{
    QuestObjectiveObjectItemsInInventory m_questObject;

    public QuestObjectiveItemInInventory(QuestObjectiveObjectItemsInInventory questObject)
    {
        m_questObject = questObject;
    }

    public override void OnCompletion() 
    { 
        if(m_questObject.m_removeOnCompletion)
        { 
            foreach(var item in m_questObject.m_itemList)
            {
                Event<RemoveInventoryItemEvent>.Broadcast(new RemoveInventoryItemEvent(item.m_itemType, item.m_count));
            }
        }
    }

    public override void OnFail() { }

    public override void OnStart() { }

    public override QuestCompletionState Update()
    {
        foreach(var item in m_questObject.m_itemList)
        {
            GetInventoryItemEvent itemData = new GetInventoryItemEvent(item.m_itemType);
            Event<GetInventoryItemEvent>.Broadcast(itemData);
            if (itemData.stack < item.m_count)
                return QuestCompletionState.Failed;
        }

        return QuestCompletionState.Completed;  
    }
}

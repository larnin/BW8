using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum ItemType
{
    Heart,
    Money,
}

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] int m_size = 10;

    class InventoryItem
    {
        public ItemType itemType;
        public int stack;
    }

    int m_money = 0;
    List<InventoryItem> m_inventory = new List<InventoryItem>();

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_inventory.Clear();
        for (int i = 0; i < m_size; i++)
            m_inventory.Add(new InventoryItem());

        m_subscriberList.Add(new Event<PickupEvent>.LocalSubscriber(Pickup, gameObject));
        m_subscriberList.Add(new Event<GetPlayerMoneyEvent>.Subscriber(GetMoney));
        m_subscriberList.Add(new Event<GetInventorySizeEvent>.Subscriber(GetInventorySize));
        m_subscriberList.Add(new Event<GetIventorySlotEvent>.Subscriber(GetInventorySlot));
        m_subscriberList.Add(new Event<GetInventoryItemEvent>.Subscriber(GetInventoryItem));
        m_subscriberList.Add(new Event<RemoveInventorySlotEvent>.Subscriber(RemoveInventorySlot));
        m_subscriberList.Add(new Event<RemoveInventoryItemEvent>.Subscriber(RemoveInventoryItem));
        m_subscriberList.Add(new Event<AddInventoryItemEvent>.Subscriber(AddInventoryItem));
        m_subscriberList.Add(new Event<MoveInventorySlotEvent>.Subscriber(MoveInventorySlot));
        m_subscriberList.Add(new Event<GetInventoryStorageCapacityEvent>.Subscriber(GetInventoryStorageCapacity));

        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    int AddItem(ItemType type, int stack)
    {
        switch (type)
        {
            case ItemType.Heart:
                Event<RegenLifeEvent>.Broadcast(new RegenLifeEvent(stack), gameObject);
                return stack;
            case ItemType.Money:
                m_money += stack;
                return stack;
            default:
                return AddItemInventory(type, stack);
        }
    }

    int AddItemInventory(ItemType type, int stack)
    {
        int initialStack = stack;
        int maxStack = LootType.GetMaxStack(type);

        var slots = GetSlotsOfType(type);

        foreach(var s in slots)
        {
            var item = m_inventory[s];
            int add = maxStack - item.stack;
            if(add >= stack)
            {
                item.stack += stack;
                return initialStack;
            }
            stack -= add;
            item.stack = maxStack;
        }

        foreach(var item in m_inventory)
        {
            if (item.stack > 0)
                continue;

            item.itemType = type;
            if(stack <= maxStack)
            {
                item.stack = stack;
                return initialStack;
            }
            item.stack = maxStack;
            stack -= maxStack;
        }

        return initialStack - stack;
    }

    List<int> GetSlotsOfType(ItemType type)
    {
        List<int> slots = new List<int>();

        for(int i = 0; i < m_size; i++)
        {
            if (m_inventory[i].stack <= 0)
                continue;
            if (m_inventory[i].itemType == type)
                slots.Add(i);
        }

        return slots;
    }

    int GetItem(ItemType type)
    {
        switch (type)
        {
            case ItemType.Heart:
                return 0;
            case ItemType.Money:
                return m_money;
            default:
                return GetItemInventory(type);
        }
    }

    int GetItemInventory(ItemType type)
    {
        int nb = 0;

        for(int i = 0; i < m_size; i++)
        {
            if (m_inventory[i].itemType == type)
                nb += m_inventory[i].stack;
        }

        return nb;
    }

    int RemoveItem(ItemType type, int count)
    {
        switch (type)
        {
            case ItemType.Heart:
                return 0;
            case ItemType.Money:
                if(m_money > count)
                {
                    m_money -= count;
                    return count;
                }
                else
                {
                    int temp = m_money;
                    m_money = 0;
                    return temp;
                }
            default:
                return RemoveItemInventory(type, count);
        }
    }

    int RemoveItemInventory(ItemType type, int count)
    {
        int initialCount = count;
        var slots = GetSlotsOfType(type);

        if (slots.Count == 0)
            return 0;

        for(int i = 0; i < slots.Count; i++)
        {
            int index = slots.Count - 1 - i;
            var item = m_inventory[slots[index]];

            if(item.stack >= count)
            {
                item.stack -= count;
                return initialCount;
            }
            count -= item.stack;
            item.stack = 0;
        }

        return initialCount - count;
    }

    int GetStorageCapacity(ItemType type)
    {
        switch (type)
        {
            case ItemType.Heart:
                return int.MaxValue;
            case ItemType.Money:
                return int.MaxValue - m_money;
            default:
                return GetStorageCapacityInventory(type);
        }
    }

    int GetStorageCapacityInventory(ItemType type)
    {
        int count = 0;

        int maxStack = LootType.GetMaxStack(type);

        foreach (var item in m_inventory)
        {
            if (item.stack <= 0)
            {
                count += maxStack;
                continue;
            }

            if (item.itemType != type)
                continue;

            count += item.stack;
        }

        return count;
    }

    void Pickup(PickupEvent e)
    {
        Event<ItemPickedUpEvent>.Broadcast(new ItemPickedUpEvent(e.type, e.stack));

        AddItem(e.type, e.stack);
    }

    void GetMoney(GetPlayerMoneyEvent e)
    {
        e.money = m_money;
    }

    void GetInventorySize(GetInventorySizeEvent e)
    {
        e.size = m_size;
    }

    void GetInventorySlot(GetIventorySlotEvent e)
    {
        if(e.slot < 0 || e.slot >= m_size)
        {
            e.stack = 0;
            return;
        }

        e.stack = m_inventory[e.slot].stack;
        e.type = m_inventory[e.slot].itemType;
    }

    void GetInventoryItem(GetInventoryItemEvent e)
    {
        e.stack = GetItem(e.type);
    }

    void RemoveInventorySlot(RemoveInventorySlotEvent e)
    {
        if (e.slot < 0 || e.slot >= m_size)
            return;

        if (e.stack < 0 || e.stack >= m_inventory[e.slot].stack)
        {
            e.removedStack = m_inventory[e.slot].stack;
            m_inventory[e.slot].stack = 0;
        }
        else
        {
            e.removedStack = e.stack;
            m_inventory[e.slot].stack -= e.stack;
        }
    }
   
    void RemoveInventoryItem(RemoveInventoryItemEvent e)
    {
        e.removedStack = RemoveItem(e.type, e.stack);
    }

    void AddInventoryItem(AddInventoryItemEvent e)
    {
        AddItem(e.type, e.stack);
    }

    void MoveInventorySlot(MoveInventorySlotEvent e)
    {
        if(e.oldSlot < 0 || e.oldSlot >= m_size || e.newSlot < 0 || e.newSlot >= m_size)
        {
            e.moved = false;
            return;
        }

        ItemType tempType = m_inventory[e.newSlot].itemType;
        int tempStack = m_inventory[e.newSlot].stack;

        m_inventory[e.newSlot].itemType = m_inventory[e.oldSlot].itemType;
        m_inventory[e.newSlot].stack = m_inventory[e.oldSlot].stack;

        m_inventory[e.oldSlot].itemType = tempType;
        m_inventory[e.oldSlot].stack = tempStack;
    }

    void GetInventoryStorageCapacity(GetInventoryStorageCapacityEvent e)
    {
        e.count = GetStorageCapacity(e.type);
    }
}

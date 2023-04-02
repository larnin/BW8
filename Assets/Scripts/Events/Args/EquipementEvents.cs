using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SetEquipementEvent
{
    public EquipementSlot slot;
    public ItemType itemType;
    public bool oldToInventory;

    public SetEquipementEvent(EquipementSlot _slot, ItemType _itemType, bool _oldToInventory = false)
    {
        slot = _slot;
        itemType = _itemType;
        oldToInventory = _oldToInventory;
    }
}

public class DeleteEquipementEvent
{
    public EquipementSlot slot;
    public bool oldToInventory;

    public DeleteEquipementEvent(EquipementSlot _slot, bool _oldToInventory = false)
    {
        slot = _slot;
        oldToInventory = _oldToInventory;
    }
}

public class EquipItemEvent
{
    public EquipementSlot slot;
    public int inventorySlot;

    public EquipItemEvent(EquipementSlot _slot, int _inventorySlot)
    {
        slot = _slot;
        inventorySlot = _inventorySlot;
    }
}

public class UnequipItemEvent
{
    public EquipementSlot slot;

    public UnequipItemEvent(EquipementSlot _slot)
    {
        slot = _slot;
    }
}

public class GetEquipementEvent
{
    public EquipementSlot slot;

    bool empty = true;
    ItemType itemType;

    public GetEquipementEvent(EquipementSlot _slot)
    {
        slot = _slot;
    }

    public void SetItem(ItemType? _item)
    {
        empty = _item == null;
        itemType = _item ?? ItemType.Heart;
    }

    public bool IsEmpty() { return empty; }
    public ItemType GetItem() { return itemType; }
}


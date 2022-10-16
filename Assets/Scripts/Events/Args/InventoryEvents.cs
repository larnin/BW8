using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PickupEvent
{
    public ItemType type;
    public int stack;

    public PickupEvent(ItemType _type, int _stack)
    {
        type = _type;
        stack = _stack;
    }
}

public class ItemPickedUpEvent
{
    public ItemType type;
    public int stack;

    public ItemPickedUpEvent(ItemType _type, int _stack)
    {
        type = _type;
        stack = _stack;
    }
}

public class GetInventorySizeEvent
{
    public int size;
}

public class GetIventorySlotEvent
{
    public int slot;

    public ItemType type;
    public int stack;

    public GetIventorySlotEvent(int _slot)
    {
        slot = _slot;
    }
}

public class GetInventoryItemEvent
{
    public ItemType type;
    public int stack;

    public GetInventoryItemEvent(ItemType _type)
    {
        type = _type;
    }
}

public class RemoveInventorySlotEvent
{
    public int slot;

    public RemoveInventorySlotEvent(int _slot)
    {
        slot = _slot;
    }
}

public class RemoveInventoryItemEvent
{
    public ItemType type;
    public int stack;

    public int removedStack;

    public RemoveInventoryItemEvent(ItemType _type, int _stack)
    {
        type = _type;
        stack = _stack;
    }
}

public class AddInventoryItemEvent
{
    public ItemType type;
    public int stack;

    public AddInventoryItemEvent(ItemType _type, int _stack)
    {
        type = _type;
        stack = _stack;
    }
}

public class MoveInventorySlotEvent
{
    public int oldSlot;
    public int newSlot;

    public bool moved;

    public MoveInventorySlotEvent(int _oldSlot, int _newSlot)
    {
        oldSlot = _oldSlot;
        newSlot = _newSlot;
    }
}

public class GetInventoryStorageCapacityEvent
{
    public ItemType type;
    public int count;

    public GetInventoryStorageCapacityEvent(ItemType _type)
    {
        type = _type;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DisplaySpecialItemEvent
{
    public ItemType item;

    public DisplaySpecialItemEvent(ItemType _item)
    {
        item = _item;
    }
}
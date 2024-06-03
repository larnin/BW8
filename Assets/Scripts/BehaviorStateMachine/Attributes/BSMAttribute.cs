using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMAttribute
{
    public string ID;
    public bool automatic = false;
    public string name;

    public BSMAttributeData data = new BSMAttributeData();

    public BSMAttribute()
    {
        ID = Guid.NewGuid().ToString();
    }

    public BSMAttribute Clone()
    {
        var attribute = new BSMAttribute();
        attribute.ID = ID;
        attribute.automatic = automatic;
        attribute.name = name;
        attribute.data = data.Clone();

        return attribute;
    }
}

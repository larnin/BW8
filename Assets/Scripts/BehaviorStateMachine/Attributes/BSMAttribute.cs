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

    public void Load(JsonObject obj)
    {
        var idElt = obj.GetElement("ID");
        if (idElt != null && idElt.IsJsonString())
            ID = idElt.String();

        var autoElt = obj.GetElement("Auto");
        if (autoElt != null && autoElt.IsJsonNumber())
            automatic = autoElt.Int() != 0;

        var nameElt = obj.GetElement("Name");
        if (nameElt != null && nameElt.IsJsonString())
            name = nameElt.String();

        data.Load(obj);
    }

    public JsonObject Save()
    {
        JsonObject obj = new JsonObject();

        obj.AddElement("ID", ID);
        obj.AddElement("Auto", automatic ? 1 : 0);
        obj.AddElement("Name", name);
        data.Save(obj);

        return obj;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BSMAttributeObject
{
    public bool useAttribute = false;

    public BSMAttributeData data = new BSMAttributeData();

    public string attributeID;

    public BSMAttributeObject(BSMAttributeType type)
    {
        data.SetType(type);
    }

    public void Load(JsonObject obj)
    {
        var useElt = obj.GetElement("Use");
        if (useElt != null && useElt.IsJsonNumber())
            useAttribute = useElt.Int() != 0;

        if(useAttribute)
        {
            var attElt = obj.GetElement("ID");
            if (attElt != null && attElt.IsJsonString())
                attributeID = attElt.String();
        }
        data.Load(obj);
    }

    public JsonObject Save()
    {
        JsonObject obj = new JsonObject();

        obj.AddElement("Use", useAttribute ? 1 : 0);

        if (useAttribute)
            obj.AddElement("ID", attributeID);
        data.Save(obj);

        return obj;
    }
}

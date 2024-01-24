using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public abstract class BSMConditionBase
{
    public abstract void Load(JsonObject obj);
    public abstract void Save(JsonObject obj);

    public static string GetName(Type type)
    {
        const string startString = "BSMCondition";

        string name = type.Name;
        if (name.StartsWith(startString))
            name = name.Substring(startString.Length);

        return name;
    }

    public static BSMConditionBase LoadCondition(JsonObject obj)
    {
        var typeObj = obj.GetElement("Type");
        if (typeObj == null || !typeObj.IsJsonString())
            return null;

        string typeName = typeObj.String();
        try
        {
            Type t = Type.GetType(typeName);

            var instance = Activator.CreateInstance(t) as BSMConditionBase;
            if (instance == null)
                return null;

            instance.Load(obj);
            return instance;
        }
        catch(Exception)
        {
            return null;
        }
    }

    public static JsonObject SaveCondition(BSMConditionBase condition)
    {
        var type = condition.GetType();
        string typeName = type.FullName;

        JsonObject obj = new JsonObject();
        obj.AddElement("Type", typeName);

        condition.Save(obj);

        return obj;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public abstract class BSMConditionBase
{
    public static string GetName(Type type)
    {
        const string startString = "BSMCondition";

        string name = type.Name;
        if (name.StartsWith(startString))
            name = name.Substring(startString.Length);

        return name;
    }

    public abstract VisualElement GetElement();
}

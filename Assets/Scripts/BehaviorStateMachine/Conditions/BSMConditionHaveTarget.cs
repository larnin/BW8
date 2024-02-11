using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMConditionHaveTarget : BSMConditionBase
{
    static string targetName = "Target";

    public BSMConditionHaveTarget()
    {
        AddAttribute(targetName, new BSMAttributeObject((GameObject)null));
    }

    public override bool IsValid()
    {
        var target = GetGameObjectAttribute(targetName);
        return target != null;
    }

    public override void Load(JsonObject obj) { }

    public override void Save(JsonObject obj) { }
}

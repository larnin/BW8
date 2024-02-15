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
        AddAttribute(targetName, BSMAttributeObject.Create((GameObject)null));
    }

    public override bool IsValid()
    {
        var target = GetUnityObjectAttribute<GameObject>(targetName);
        return target != null;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMConditionDistanceToTarget : BSMConditionBase
{
    static string targetName = "Target";
    static string distanceName = "Distance";

    public BSMConditionDistanceToTarget()
    {
        AddAttribute(targetName, BSMAttributeObject.CreateUnityObject((GameObject)null));
        AddAttribute(distanceName, new BSMAttributeObject(1.0f));
    }

    public override bool IsValid()
    {
        var target = GetUnityObjectAttribute<GameObject>(targetName);
        if (target == null)
            return false;

        float distance = GetFloatAttribute(distanceName, 0);

        Vector2 currentPos = m_state.GetControler().transform.position;
        Vector2 targetPos = target.transform.position;

        float sqrLen = (currentPos - targetPos).sqrMagnitude;

        return sqrLen < distance * distance;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMActionInstantiateAtPos : BSMActionBase
{
    static string prefabName = "Prefab";
    static string posName = "Position";

    public BSMActionInstantiateAtPos()
    {
        AddAttribute(prefabName, BSMAttributeObject.CreateUnityObject((GameObject)null));
        AddAttribute(posName, new BSMAttributeObject(Vector3.zero));
    }

    public override void Exec()
    {
        var pos = GetVector3Attribute(posName, Vector3.zero);
        var prefab = GetUnityObjectAttribute(prefabName, (GameObject)null);

        if(prefab != null)
        {
            var obj = GameObject.Instantiate(prefab);
            obj.transform.position = pos;
        }
    }
}

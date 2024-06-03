using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NRand;

public class BSMActionGetRandomPosition : BSMActionBase
{
    static string minName = "MinRange";
    static string maxName = "MaxRange";
    static string outName = "OutPos";

    public BSMActionGetRandomPosition()
    {
        AddAttribute(minName, new BSMAttributeObject(0.0f));
        AddAttribute(maxName, new BSMAttributeObject(1.0f));

        var posAttribute = new BSMAttributeObject(Vector3.zero);
        posAttribute.SetDisplayType(BSMAttributeDisplayType.forceAttribute);
        AddAttribute(outName, posAttribute);
    }

    public override void Exec()
    {
        float min = GetFloatAttribute(minName, 0.0f);
        float max = GetFloatAttribute(maxName, 1.0f);

        var distrib = new UniformVector2AnnulusDistribution(min, max);

        var pos = distrib.Next(new StaticRandomGenerator<MT19937>());

        var currentPos = GetControler().transform.position;
        currentPos.x += pos.x;
        currentPos.y += pos.y;

        var attribute = GetAttribute(outName);

        var writeAttribute = GetControler().GetAttribute(attribute.attributeID);
        writeAttribute.data.SetVector3(currentPos);
    }
}

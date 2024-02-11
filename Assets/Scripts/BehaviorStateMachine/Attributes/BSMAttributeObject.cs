﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMAttributeObject
{
    public bool useAttribute = false;

    public BSMAttributeData data = new BSMAttributeData();

    public string attributeID;

    public BSMAttributeObject() { }

    public BSMAttributeObject(float defaultValue)
    {
        data.SetType(BSMAttributeType.attributeFloat);
        data.SetFloat(defaultValue);
    }

    public BSMAttributeObject(int defaultValue)
    {
        data.SetType(BSMAttributeType.attributeInt);
        data.SetInt(defaultValue);
    }

    public BSMAttributeObject(string defaultValue)
    {
        data.SetType(BSMAttributeType.attributeString);
        data.SetString(defaultValue);
    }

    public BSMAttributeObject(GameObject defaultValue)
    {
        data.SetType(BSMAttributeType.attributeGameObject);
        data.SetGameObject(defaultValue);
    }

    public BSMAttributeObject(Vector2 defaultValue)
    {
        data.SetType(BSMAttributeType.attributeVector2);
        data.SetVector2(defaultValue);
    }

    public BSMAttributeObject(Vector3 defaultValue)
    {
        data.SetType(BSMAttributeType.attributeVector3);
        data.SetVector3(defaultValue);
    }

    public BSMAttributeObject(Vector2Int defaultValue)
    {
        data.SetType(BSMAttributeType.attributeVector2Int);
        data.SetVector2Int(defaultValue);
    }

    public BSMAttributeObject(Vector3Int defaultValue)
    {
        data.SetType(BSMAttributeType.attributeVector3Int);
        data.SetVector3Int(defaultValue);
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

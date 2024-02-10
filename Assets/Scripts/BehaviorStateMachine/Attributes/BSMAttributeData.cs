﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum BSMAttributeType
{
    attributeFloat,
    attributeInt,
    attributeString,
    attributeGameObject,
}

public class BSMAttributeData
{
    public BSMAttributeType attributeType = BSMAttributeType.attributeInt;
    public object data;

    public BSMAttributeData Clone()
    {
        var newData = new BSMAttributeData();
        newData.attributeType = attributeType;
        newData.data = data;

        return newData;
    }

    public void SetType(BSMAttributeType type)
    {
        attributeType = type;
        data = null;
    }

    public void SetFloat(float value)
    {
        if (attributeType != BSMAttributeType.attributeFloat)
        {
            Debug.LogError("Attribute have a type " + AttributeName(attributeType));
            return;
        }
        data = new float?(value);
    }

    public float GetFloat(float defaultValue = 0)
    {
        if (attributeType != BSMAttributeType.attributeFloat)
        {
            Debug.LogError("Attribute have a type " + AttributeName(attributeType));
            return defaultValue;
        }

        if (data == null || !(data is float?))
            return defaultValue;

        return (data as float?) ?? defaultValue;
    }

    public void SetInt(int value)
    {
        if (attributeType != BSMAttributeType.attributeInt)
        {
            Debug.LogError("Attribute have a type " + AttributeName(attributeType));
            return;
        }
        data = new int?(value);
    }

    public int GetInt(int defaultValue = 0)
    {
        if (attributeType != BSMAttributeType.attributeInt)
        {
            Debug.LogError("Attribute have a type " + AttributeName(attributeType));
            return defaultValue;
        }

        if (data == null || !(data is int?))
            return defaultValue;

        return (data as int?) ?? defaultValue;
    }

    public void SetString(string value)
    {
        if (attributeType != BSMAttributeType.attributeString)
        {
            Debug.LogError("Attribute have a type " + AttributeName(attributeType));
            return;
        }
        data = value;
    }

    public string GetString(string defaultValue = null)
    {
        if (attributeType != BSMAttributeType.attributeString)
        {
            Debug.LogError("Attribute have a type " + AttributeName(attributeType));
            return defaultValue;
        }

        if (data == null || !(data is string))
            return defaultValue;

        return (data as string) ?? defaultValue;
    }

    public void SetGameObject(GameObject value)
    {
        if (attributeType != BSMAttributeType.attributeGameObject)
        {
            Debug.LogError("Attribute have a type " + AttributeName(attributeType));
            return;
        }
        data = value;
    }

    public GameObject GetGameObject(GameObject defaultValue = null)
    {
        if (attributeType != BSMAttributeType.attributeGameObject)
        {
            Debug.LogError("Attribute have a type " + AttributeName(attributeType));
            return defaultValue;
        }

        if (data == null || !(data is GameObject))
            return defaultValue;

        return (data as GameObject) ?? defaultValue;
    }

    public static string AttributeName(BSMAttributeType type)
    {
        string name = type.ToString();
        if (name.StartsWith("attribute"))
            return name.Substring(9);
        return name;
    }

    public void Load(JsonObject obj)
    {
        var typeElt = obj.GetElement("Type");
        if (typeElt != null && typeElt.IsJsonString())
        {
            var typeStr = typeElt.String();
            Enum.TryParse(typeStr, true, out attributeType);
        }

        var dataElt = obj.GetElement("Data");
        if (dataElt != null)
        {
            switch (attributeType)
            {
                case BSMAttributeType.attributeInt:
                    if (dataElt.IsJsonNumber())
                        SetInt(dataElt.Int());
                    break;
                case BSMAttributeType.attributeFloat:
                    if (dataElt.IsJsonNumber())
                        SetFloat(dataElt.Float());
                    break;
                case BSMAttributeType.attributeString:
                    if (dataElt.IsJsonString())
                        SetString(dataElt.String());
                    break;
                case BSMAttributeType.attributeGameObject:
                    break;
                default:
                    Debug.LogError("Unknow attribute value type");
                    break;
            }
        }
    }

    public void Save(JsonObject obj)
    {
        obj.AddElement("Type", attributeType.ToString());

        switch (attributeType)
        {
            case BSMAttributeType.attributeInt:
                obj.AddElement("Data", GetInt());
                break;
            case BSMAttributeType.attributeFloat:
                obj.AddElement("Data", GetFloat());
                break;
            case BSMAttributeType.attributeString:
                obj.AddElement("Data", GetString());
                break;
            case BSMAttributeType.attributeGameObject:
                break;
            default:
                Debug.LogError("Unknow attribute value type");
                break;
        }
    }
}

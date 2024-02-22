using System;
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

    public BSMAttributeObject(bool defaultValue)
    {
        data.SetType(BSMAttributeType.attributeBool);
        data.SetBool(defaultValue);
    }

    public BSMAttributeObject(string defaultValue)
    {
        data.SetType(BSMAttributeType.attributeString);
        data.SetString(defaultValue);
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

    public static BSMAttributeObject CreateUnityObject<T>(T defaultValue) where T : UnityEngine.Object
    {
        var attribute = new BSMAttributeObject();
        attribute.data.SetType(BSMAttributeType.attributeUnityObject);
        attribute.data.customType = typeof(T);
        attribute.data.SetUnityObject(defaultValue);

        return attribute;
    }

    public static BSMAttributeObject CreateEnum<T>(T defaultValue) where T : struct, System.Enum
    {
        var attribute = new BSMAttributeObject();
        attribute.data.SetType(BSMAttributeType.attributeEnum);
        attribute.data.customType = typeof(T);
        attribute.data.SetEnum(defaultValue);

        return attribute;
    }
}

using System;
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
    attributeUnityObject,
    attributeVector2,
    attributeVector3,
    attributeVector2Int,
    attributeVector3Int,
}

public class BSMAttributeData
{
    public BSMAttributeType attributeType = BSMAttributeType.attributeInt;
    public object data;
    public Type customType;

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
        if (type == BSMAttributeType.attributeUnityObject)
            customType = typeof(GameObject);
        data = null;
    }

    public void SetStruct<T>(T value, BSMAttributeType type) where T : struct
    {
        if (attributeType != type)
        {
            Debug.LogError("Attribute have a type " + AttributeName(attributeType));
            return;
        }
        data = new T?(value);
    }

    public T GetStruct<T>(T defaultValue, BSMAttributeType type) where T : struct
    {
        if (attributeType != type)
        {
            Debug.LogError("Attribute have a type " + AttributeName(attributeType));
            return defaultValue;
        }

        if (data == null || !(data is T?))
            return defaultValue;

        return (data as T?) ?? defaultValue;
    }

    public void SetClass<T>(T value, BSMAttributeType type) where T : class
    {
        if (attributeType != type)
        {
            Debug.LogError("Attribute have a type " + AttributeName(attributeType));
            return;
        }
        data = value;
    }

    public T GetClass<T>(T defaultValue, BSMAttributeType type) where T : class
    {
        if (attributeType != type)
        {
            Debug.LogError("Attribute have a type " + AttributeName(attributeType));
            return defaultValue;
        }

        if (data == null || !(data is T))
            return defaultValue;

        return (data as T) ?? defaultValue;
    }

    public void SetFloat(float value)
    {
        SetStruct(value, BSMAttributeType.attributeFloat);
    }

    public float GetFloat(float defaultValue = 0)
    {
        return GetStruct(defaultValue, BSMAttributeType.attributeFloat);
    }

    public void SetInt(int value)
    {
        SetStruct(value, BSMAttributeType.attributeInt);
    }

    public int GetInt(int defaultValue = 0)
    {
        return GetStruct(defaultValue, BSMAttributeType.attributeInt);
    }

    public void SetString(string value)
    {
        SetClass(value, BSMAttributeType.attributeString);
    }

    public string GetString(string defaultValue = null)
    {
        return GetClass(defaultValue, BSMAttributeType.attributeString);
    }

    public void SetUnityObject<T>(T value) where T : UnityEngine.Object
    {
        if(typeof(T) != customType)
            Debug.LogError("Attribute have a type " + customType.Name);
        SetClass(value, BSMAttributeType.attributeUnityObject);
    }

    public void SetUnityObject(Type type, UnityEngine.Object value)
    {
        if (type != customType)
            Debug.LogError("Attribute have a type " + customType.Name);
        SetClass(value, BSMAttributeType.attributeUnityObject);
    }

    public T GetUnityObject<T>(T defaultValue = null) where T : UnityEngine.Object
    {
        if (typeof(T) != customType)
            Debug.LogError("Attribute have a type " + customType.Name);
        return GetClass(defaultValue, BSMAttributeType.attributeUnityObject);
    }

    public UnityEngine.Object GetUnityObject(Type type, UnityEngine.Object defaultValue = null)
    {
        if (type != customType)
            Debug.LogError("Attribute have a type " + customType.Name);
        return GetClass(defaultValue, BSMAttributeType.attributeUnityObject);
    }

    public void SetVector2(Vector2 value)
    {
        SetStruct(value, BSMAttributeType.attributeVector2);
    }

    public Vector2 GetVector2()
    {
        return GetVector2(Vector2.zero);
    }

    public Vector2 GetVector2(Vector2 defaultValue)
    {
        return GetStruct(defaultValue, BSMAttributeType.attributeVector2);
    }

    public void SetVector3(Vector3 value)
    {
        SetStruct(value, BSMAttributeType.attributeVector3);
    }

    public Vector3 GetVector3()
    {
        return GetVector3(Vector3.zero);
    }

    public Vector3 GetVector3(Vector3 defaultValue)
    {
        return GetStruct(defaultValue, BSMAttributeType.attributeVector3);
    }

    public void SetVector2Int(Vector2Int value)
    {
        SetStruct(value, BSMAttributeType.attributeVector2Int);
    }

    public Vector2Int GetVector2Int()
    {
        return GetVector2Int(Vector2Int.zero);
    }

    public Vector2Int GetVector2Int(Vector2Int defaultValue)
    {
        return GetStruct(defaultValue, BSMAttributeType.attributeVector2Int);
    }

    public void SetVector3Int(Vector3Int value)
    {
        SetStruct(value, BSMAttributeType.attributeVector3Int);
    }

    public Vector3Int GetVector3Int()
    {
        return GetVector3Int(Vector3Int.zero);
    }

    public Vector3Int GetVector3Int(Vector3Int defaultValue)
    {
        return GetStruct(defaultValue, BSMAttributeType.attributeVector3Int);
    }

    public static string AttributeName(BSMAttributeType type)
    {
        string name = type.ToString();
        if (name.StartsWith("attribute"))
            return name.Substring(9);
        return name;
    }
}

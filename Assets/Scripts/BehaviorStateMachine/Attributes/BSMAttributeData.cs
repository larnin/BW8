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
    attributeBool,
    attributeString,
    attributeUnityObject,
    attributeVector2,
    attributeVector3,
    attributeVector2Int,
    attributeVector3Int,
    attributeEnum,
}

public class BSMAttributeData
{
    public BSMAttributeType attributeType = BSMAttributeType.attributeInt;
    public object data;

    public string customTypeName;

    public Type customType 
    { 
        get
        {
            if (customTypeName == null)
                return null;
            return Type.GetType(customTypeName);
        }
        set
        {
            if (value == null)
                customTypeName = null;
            else customTypeName = value.AssemblyQualifiedName;
        }
    }

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
        if (type == BSMAttributeType.attributeEnum)
            customType = null;
        data = null;
    }

    public void Set<T>(T value, BSMAttributeType type)
    {
        if (attributeType != type)
        {
            Debug.LogError("Attribute have a type " + AttributeName(attributeType));
            return;
        }
        data = value;
    }

    public T Get<T>(T defaultValue, BSMAttributeType type)
    {
        if (attributeType != type)
        {
            Debug.LogError("Attribute have a type " + AttributeName(attributeType));
            return defaultValue;
        }

        if (data == null || !(data is T))
            return defaultValue;

        return (T)data;
    }

    public void SetFloat(float value)
    {
        Set(value, BSMAttributeType.attributeFloat);
    }

    public float GetFloat(float defaultValue = 0)
    {
        return Get(defaultValue, BSMAttributeType.attributeFloat);
    }

    public void SetInt(int value)
    {
        Set(value, BSMAttributeType.attributeInt);
    }

    public int GetInt(int defaultValue = 0)
    {
        return Get(defaultValue, BSMAttributeType.attributeInt);
    }

    public void SetBool(bool value)
    {
        Set(value, BSMAttributeType.attributeBool);
    }

    public bool GetBool(bool defaultValue = false)
    {
        return Get(defaultValue, BSMAttributeType.attributeBool);
    }

    public void SetString(string value)
    {
        Set(value, BSMAttributeType.attributeString);
    }

    public string GetString(string defaultValue = null)
    {
        return Get(defaultValue, BSMAttributeType.attributeString);
    }

    public void SetUnityObject<T>(T value) where T : UnityEngine.Object
    {
        if(typeof(T) != customType)
            Debug.LogError("Attribute have a type " + customType.Name);
        Set(value, BSMAttributeType.attributeUnityObject);
    }

    public void SetUnityObject(Type type, UnityEngine.Object value)
    {
        if (type != customType)
            Debug.LogError("Attribute have a type " + customType.Name);
        Set(value, BSMAttributeType.attributeUnityObject);
    }

    public T GetUnityObject<T>(T defaultValue = null) where T : UnityEngine.Object
    {
        if (typeof(T) != customType)
            Debug.LogError("Attribute have a type " + customType.Name);
        return Get(defaultValue, BSMAttributeType.attributeUnityObject);
    }

    public UnityEngine.Object GetUnityObject(Type type, UnityEngine.Object defaultValue = null)
    {
        if (type != customType)
            Debug.LogError("Attribute have a type " + customType.Name);
        return Get(defaultValue, BSMAttributeType.attributeUnityObject);
    }

    public void SetEnum<T>(T value) where T : struct, System.Enum
    {
        if (typeof(T) != customType)
            Debug.LogError("Attribute have a type " + customType.Name);
        Set(value, BSMAttributeType.attributeEnum);
    }

    public void SetEnum(Type type, object value)
    {
        if (type != customType)
            Debug.LogError("Attribute have a type " + customType.Name);
        Set(value, BSMAttributeType.attributeEnum);
    }

    public T GetEnum<T>(T defaultValue = default(T)) where T : struct, System.Enum
    {
        if (typeof(T) != customType)
            Debug.LogError("Attribute have a type " + customType.Name);
        return Get(defaultValue, BSMAttributeType.attributeEnum);
    }

    public object GetEnum(Type type, object defaultValue = null)
    {
        if (type != customType)
            Debug.LogError("Attribute have a type " + customType.Name);
        return Get(defaultValue, BSMAttributeType.attributeEnum);
    }

    public void SetVector2(Vector2 value)
    {
        Set(value, BSMAttributeType.attributeVector2);
    }

    public Vector2 GetVector2()
    {
        return GetVector2(Vector2.zero);
    }

    public Vector2 GetVector2(Vector2 defaultValue)
    {
        return Get(defaultValue, BSMAttributeType.attributeVector2);
    }

    public void SetVector3(Vector3 value)
    {
        Set(value, BSMAttributeType.attributeVector3);
    }

    public Vector3 GetVector3()
    {
        return GetVector3(Vector3.zero);
    }

    public Vector3 GetVector3(Vector3 defaultValue)
    {
        return Get(defaultValue, BSMAttributeType.attributeVector3);
    }

    public void SetVector2Int(Vector2Int value)
    {
        Set(value, BSMAttributeType.attributeVector2Int);
    }

    public Vector2Int GetVector2Int()
    {
        return GetVector2Int(Vector2Int.zero);
    }

    public Vector2Int GetVector2Int(Vector2Int defaultValue)
    {
        return Get(defaultValue, BSMAttributeType.attributeVector2Int);
    }

    public void SetVector3Int(Vector3Int value)
    {
        Set(value, BSMAttributeType.attributeVector3Int);
    }

    public Vector3Int GetVector3Int()
    {
        return GetVector3Int(Vector3Int.zero);
    }

    public Vector3Int GetVector3Int(Vector3Int defaultValue)
    {
        return Get(defaultValue, BSMAttributeType.attributeVector3Int);
    }

    public static string AttributeName(BSMAttributeType type)
    {
        string name = type.ToString();
        if (name.StartsWith("attribute"))
            return name.Substring(9);
        return name;
    }
}

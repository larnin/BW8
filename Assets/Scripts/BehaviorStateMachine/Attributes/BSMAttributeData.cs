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
    attributeGameObject,
    attributeVector2,
    attributeVector3,
    attributeVector2Int,
    attributeVector3Int,
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

    public void SetGameObject(GameObject value)
    {
        SetClass(value, BSMAttributeType.attributeGameObject);
    }

    public GameObject GetGameObject(GameObject defaultValue = null)
    {
        return GetClass(defaultValue, BSMAttributeType.attributeGameObject);
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
                case BSMAttributeType.attributeVector2:
                    SetVector2(Json.ToVector2(dataElt.JsonArray()));
                    break;
                case BSMAttributeType.attributeVector3:
                    SetVector3(Json.ToVector3(dataElt.JsonArray()));
                    break;
                case BSMAttributeType.attributeVector2Int:
                    SetVector2Int(Json.ToVector2Int(dataElt.JsonArray()));
                    break;
                case BSMAttributeType.attributeVector3Int:
                    SetVector3Int(Json.ToVector3Int(dataElt.JsonArray()));
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
            case BSMAttributeType.attributeVector2:
                obj.AddElement("Data", Json.FromVector2(GetVector2()));
                break;
            case BSMAttributeType.attributeVector3:
                obj.AddElement("Data", Json.FromVector3(GetVector3()));
                break;
            case BSMAttributeType.attributeVector2Int:
                obj.AddElement("Data", Json.FromVector2Int(GetVector2Int()));
                break;
            case BSMAttributeType.attributeVector3Int:
                obj.AddElement("Data", Json.FromVector3Int(GetVector3Int()));
                break;
            default:
                Debug.LogError("Unknow attribute value type");
                break;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class BSMAttributeNamedObject
{
    public string name;
    public BSMAttributeObject attribute;
}

public abstract class BSMAttributeHolder
{
    [SerializeField] protected List<BSMAttributeNamedObject> m_attributes = new List<BSMAttributeNamedObject>();

    protected void AddAttribute(string name, BSMAttributeObject attribute)
    {
        BSMAttributeNamedObject namedAttribute = new BSMAttributeNamedObject();
        namedAttribute.name = name;
        namedAttribute.attribute = attribute;

        m_attributes.Add(namedAttribute);
    }

    public List<BSMAttributeNamedObject> GetAttributes()
    {
        return m_attributes;
    }

    protected BSMAttributeObject GetAttribute(string name)
    {
        foreach(var a in m_attributes)
        {
            if (a.name == name)
                return a.attribute;
        }

        return null;
    }


    public abstract BSMControler GetControler();

    public T GetAttribute<T>(string name, BSMAttributeType attributeType, T defaultValue)
    {
        var attribute = GetAttribute(name);
        if (attribute == null)
            return defaultValue;

        if (attribute.useAttribute)
        {
            var controler = GetControler();
            if (controler == null)
                return defaultValue;
            var realAttribute = controler.GetAttribute(attribute.attributeID);
            if (realAttribute == null)
                return default;
            return realAttribute.data.Get(defaultValue, attributeType);
        }
        else return attribute.data.Get(defaultValue, attributeType);
    }

    public int GetIntAttribute(string name, int defaultValue = 0)
    {
        return GetAttribute(name, BSMAttributeType.attributeInt, defaultValue);
    }

    public bool GetBoolAttribute(string name, bool defaultValue = false)
    {
        return GetAttribute(name, BSMAttributeType.attributeBool, defaultValue);
    }

    public float GetFloatAttribute(string name, float defaultValue = 0)
    {
        return GetAttribute(name, BSMAttributeType.attributeFloat, defaultValue);
    }

    public string GetStringAttribute(string name, string defaultValue = "")
    {
        return GetAttribute(name, BSMAttributeType.attributeString, defaultValue);
    }

    public T GetUnityObjectAttribute<T>(string name, T defaultValue = null) where T : UnityEngine.Object
    {
        var attribute = GetAttribute(name);
        if (attribute == null)
            return defaultValue;

        if (attribute.useAttribute)
        {
            var controler = GetControler();
            if (controler == null)
                return defaultValue;
            var realAttribute = controler.GetAttribute(attribute.attributeID);
            if (realAttribute == null)
                return default;
            return realAttribute.data.GetUnityObject(defaultValue);
        }
        else return attribute.data.GetUnityObject(defaultValue);
    }

    public T GetEnumAttribute<T>(string name, T defaultValue = default(T)) where T : struct, System.Enum
    {
        var attribute = GetAttribute(name);
        if (attribute == null)
            return defaultValue;

        if (attribute.useAttribute)
        {
            var controler = GetControler();
            if (controler == null)
                return defaultValue;
            var realAttribute = controler.GetAttribute(attribute.attributeID);
            if (realAttribute == null)
                return default;
            return realAttribute.data.GetEnum(defaultValue);
        }
        else return attribute.data.GetEnum(defaultValue);
    }

    public Vector2 GetVector2Attribute(string name) { return GetVector2Attribute(name, Vector2.zero); }
    public Vector2 GetVector2Attribute(string name, Vector2 defaultValue)
    {
        return GetAttribute(name, BSMAttributeType.attributeVector2, defaultValue);
    }

    public Vector3 GetVector3Attribute(string name) { return GetVector3Attribute(name, Vector3.zero); }
    public Vector3 GetVector3Attribute(string name, Vector3 defaultValue)
    {
        return GetAttribute(name, BSMAttributeType.attributeVector3, defaultValue);
    }

    public Vector2Int GetVector2IntAttribute(string name) { return GetVector2IntAttribute(name, Vector2Int.zero); }
    public Vector2Int GetVector2IntAttribute(string name, Vector2Int defaultValue)
    {
        return GetAttribute(name, BSMAttributeType.attributeVector2Int, defaultValue);
    }

    public Vector3Int GetVector3IntAttribute(string name) { return GetVector3IntAttribute(name, Vector3Int.zero); }
    public Vector3Int GetVector3IntAttribute(string name, Vector3Int defaultValue)
    {
        return GetAttribute(name, BSMAttributeType.attributeVector3Int, defaultValue);
    }
}

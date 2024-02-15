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

    protected void LoadAttributes(JsonObject obj)
    {
        var attElt = obj.GetElement("Attributes");
        if(attElt != null && attElt.IsJsonArray())
        {
            var attArray = attElt.JsonArray();

            foreach(var aElt in attArray)
            {
                if (!aElt.IsJsonObject())
                    continue;

                var aObj = aElt.JsonObject();

                var nameElt = aObj.GetElement("Name");
                if (nameElt == null || !nameElt.IsJsonString())
                    continue;

                string name = nameElt.String();

                BSMAttributeObject attribute = new BSMAttributeObject();
                attribute.Load(aObj);

                foreach(var a in m_attributes)
                {
                    if(a.name == name && a.attribute.data.attributeType == attribute.data.attributeType)
                    {
                        a.attribute = attribute;
                        break;
                    }
                }
            }
        }
    }

    protected void SaveAttributes(JsonObject obj)
    {
        if (m_attributes.Count == 0)
            return;

        JsonArray array = new JsonArray();
        foreach(var attribute in m_attributes)
        {
            JsonObject attObj = attribute.attribute.Save();
            attObj.AddElement("Name", attribute.name);
            array.Add(attObj);
        }

        obj.AddElement("Attributes", array);
    }

    public abstract BSMControler GetControler();

    public T GetStructAttribute<T>(string name, BSMAttributeType attributeType, T defaultValue) where T : struct
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
            return realAttribute.data.GetStruct(defaultValue, attributeType);
        }
        else return attribute.data.GetStruct(defaultValue, attributeType);
    }

    public T GetClassAttribute<T>(string name, BSMAttributeType attributeType, T defaultValue) where T : class
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
            return realAttribute.data.GetClass(defaultValue, attributeType);
        }
        else return attribute.data.GetClass(defaultValue, attributeType);
    }

    public int GetIntAttribute(string name, int defaultValue = 0)
    {
        return GetStructAttribute(name, BSMAttributeType.attributeInt, defaultValue);
    }

    public float GetFloatAttribute(string name, float defaultValue = 0)
    {
        return GetStructAttribute(name, BSMAttributeType.attributeFloat, defaultValue);
    }

    public string GetStringAttribute(string name, string defaultValue = "")
    {
        return GetClassAttribute(name, BSMAttributeType.attributeString, defaultValue);
    }

    public GameObject GetGameObjectAttribute(string name, GameObject defaultValue = null)
    {
        return GetClassAttribute(name, BSMAttributeType.attributeGameObject, defaultValue);
    }

    public Vector2 GetVector2Attribute(string name) { return GetVector2Attribute(name, Vector2.zero); }
    public Vector2 GetVector2Attribute(string name, Vector2 defaultValue)
    {
        return GetStructAttribute(name, BSMAttributeType.attributeVector2, defaultValue);
    }

    public Vector3 GetVector3Attribute(string name) { return GetVector3Attribute(name, Vector3.zero); }
    public Vector3 GetVector3Attribute(string name, Vector3 defaultValue)
    {
        return GetStructAttribute(name, BSMAttributeType.attributeVector3, defaultValue);
    }

    public Vector2Int GetVector2IntAttribute(string name) { return GetVector2IntAttribute(name, Vector2Int.zero); }
    public Vector2Int GetVector2IntAttribute(string name, Vector2Int defaultValue)
    {
        return GetStructAttribute(name, BSMAttributeType.attributeVector2Int, defaultValue);
    }

    public Vector3Int GetVector3IntAttribute(string name) { return GetVector3IntAttribute(name, Vector3Int.zero); }
    public Vector3Int GetVector3IntAttribute(string name, Vector3Int defaultValue)
    {
        return GetStructAttribute(name, BSMAttributeType.attributeVector3Int, defaultValue);
    }
}

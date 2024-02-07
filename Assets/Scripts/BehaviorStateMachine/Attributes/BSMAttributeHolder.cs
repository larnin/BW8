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
    protected List<BSMAttributeNamedObject> m_attributes = new List<BSMAttributeNamedObject>();

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

    public abstract int GetIntAttribute(string name, int defaultValue = 0);
    public abstract float GetFloatAttribute(string name, float defaultValue = 0);
    public abstract string GetStringAttribute(string name, string defaultValue = "");
    public abstract GameObject GetGameObjectAttribute(string name, GameObject defaultValue = null);
}

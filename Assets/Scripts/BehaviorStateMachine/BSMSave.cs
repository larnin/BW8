using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMSaveNode
{
    public string ID;
    public string name;
    public BSMNodeType nodeType;
    public Rect position;

    public object data;

    public List<string> outNodes = new List<string>();

    public void Load(JsonObject obj)
    {
        var idElt = obj.GetElement("ID");
        if (idElt != null && idElt.IsJsonString())
            ID = idElt.String();

        var nameElt = obj.GetElement("Name");
        if (nameElt != null && nameElt.IsJsonString())
            name = nameElt.String();

        var typeElt = obj.GetElement("Type");
        if(typeElt != null && typeElt.IsJsonString())
        {
            var typeStr = typeElt.String();
            Enum.TryParse(typeStr, true, out nodeType);
        }

        var posElt = obj.GetElement("Pos");
        if (posElt != null && posElt.IsJsonArray())
            position = Json.ToRect(posElt.JsonArray(), position);

        var dataElt = obj.GetElement("Data");
        if (dataElt != null && dataElt.IsJsonObject())
        {
            var dataObj = dataElt.JsonObject();
            if (nodeType == BSMNodeType.Condition)
                data = BSMConditionBase.LoadCondition(dataObj);
            else if (nodeType == BSMNodeType.State)
                data = BSMStateBase.LoadState(dataObj);
            else if(nodeType == BSMNodeType.Goto)
            {
                var IDElt = dataObj.GetElement("LabelID");
                if (IDElt != null && IDElt.IsJsonString())
                    data = IDElt.String();
            }
        }

        outNodes.Clear();
        var outElt = obj.GetElement("OutNodes");
        if(outElt != null && outElt.IsJsonArray())
        {
            var outArray = outElt.JsonArray();
            foreach(var e in outArray)
            {
                if (e.IsJsonString())
                    outNodes.Add(e.String());
            }
        }
    }

    public JsonObject Save()
    {
        JsonObject obj = new JsonObject();

        obj.AddElement("ID", ID);
        obj.AddElement("Name", name);
        obj.AddElement("Type", nodeType.ToString());
        obj.AddElement("Pos", Json.FromRect(position));

        if(data != null)
        {
            JsonObject dataObj = null;
            if (nodeType == BSMNodeType.Condition)
                dataObj = BSMConditionBase.SaveCondition(data as BSMConditionBase);
            else if (nodeType == BSMNodeType.State)
                dataObj = BSMStateBase.SaveState(data as BSMStateBase);
            else if(nodeType == BSMNodeType.Goto)
            {
                dataObj = new JsonObject();
                dataObj.AddElement("LabelID", data as string);
            }

            if (dataObj != null)
                obj.AddElement("Data", dataObj);
        }

        JsonArray outArray = new JsonArray();
        foreach (var o in outNodes)
            outArray.Add(o);

        obj.AddElement("OutNodes", outArray);

        return obj;
    }
}

public class BSMSaveData
{
    public List<BSMSaveNode> nodes = new List<BSMSaveNode>();
    public List<BSMAttribute> attributes = new List<BSMAttribute>();

    public void Load(JsonObject obj)
    {
        nodes.Clear();
        var nodesElt = obj.GetElement("Nodes");
        if(nodesElt != null && nodesElt.IsJsonArray())
        {
            var nodesArray = nodesElt.JsonArray();

            foreach(var nodeElt in nodesArray)
            {
                if (!nodeElt.IsJsonObject())
                    continue;

                BSMSaveNode node = new BSMSaveNode();
                node.Load(nodeElt.JsonObject());

                nodes.Add(node);
            }
        }

        attributes.Clear();
        var attributesElt = obj.GetElement("Attributes");
        if(attributesElt != null && attributesElt.IsJsonArray())
        {
            var attributesArray = attributesElt.JsonArray();

            foreach(var attributeElt in attributesArray)
            {
                if (!attributeElt.IsJsonObject())
                    continue;

                BSMAttribute attribute = new BSMAttribute();
                attribute.Load(attributeElt.JsonObject());

                attributes.Add(attribute);
            }
        }
    }

    public JsonObject Save()
    {
        JsonObject obj = new JsonObject();

        JsonArray nodesArray = new JsonArray();
        foreach (var node in nodes)
            nodesArray.Add(node.Save());

        obj.AddElement("Nodes", nodesArray);

        JsonArray attributesArray = new JsonArray();
        foreach (var attribute in attributes)
            attributesArray.Add(attribute.Save());

        obj.AddElement("Attributes", attributesArray);

        return obj;
    }
}
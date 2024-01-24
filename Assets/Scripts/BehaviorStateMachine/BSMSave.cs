using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface BSMSerializable
{ 
    public void Load(JsonObject obj);
    public void Save(JsonObject obj);
}

public enum BSMSaveNodeType
{
    Label,
    Goto,
    Condition,
    State,
}

public class BSMSaveNode
{
    public int id;
    public string name;
    public BSMSaveNodeType nodeType;
    public Vector2 position;

    public object data;

    public List<int> outNodes = new List<int>();

    public void Load(JsonObject obj)
    {
        var idElt = obj.GetElement("ID");
        if (idElt != null && idElt.IsJsonNumber())
            id = idElt.Int();

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
            position = Json.ToVector2(posElt.JsonArray(), position);

        var dataElt = obj.GetElement("Data");
        if (dataElt != null && dataElt.IsJsonObject())
        {
            var dataObj = dataElt.JsonObject();
            if (nodeType == BSMSaveNodeType.Condition)
                data = BSMConditionBase.LoadCondition(dataObj);
            else if (nodeType == BSMSaveNodeType.State)
                data = BSMStateBase.LoadState(dataObj);
        }

        outNodes.Clear();
        var outElt = obj.GetElement("OutNodes");
        if(outElt != null && outElt.IsJsonArray())
        {
            var outArray = outElt.JsonArray();
            foreach(var e in outArray)
            {
                if (e.IsJsonNumber())
                    outNodes.Add(e.Int());
            }
        }
    }

    public JsonObject Save()
    {
        JsonObject obj = new JsonObject();

        obj.AddElement("ID", id);
        obj.AddElement("Name", name);
        obj.AddElement("Type", nodeType.ToString());
        obj.AddElement("Pos", Json.FromVector2(position));

        if(data != null)
        {
            JsonObject dataObj = null;
            if (nodeType == BSMSaveNodeType.Condition)
                dataObj = BSMConditionBase.SaveCondition(data as BSMConditionBase);
            else if (nodeType == BSMSaveNodeType.State)
                dataObj = BSMStateBase.SaveState(data as BSMStateBase);

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
    }

    public JsonObject Save()
    {
        JsonObject obj = new JsonObject();

        JsonArray nodesArray = new JsonArray();
        foreach (var node in nodes)
            nodesArray.Add(node.Save());

        obj.AddElement("Nodes", nodesArray);

        return obj;
    }
}
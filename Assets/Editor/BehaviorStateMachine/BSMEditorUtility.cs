using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public static class BSMEditorUtility
{
    public static VisualElement AddClasses(this VisualElement element, params string[] classNames)
    {
        foreach (string className in classNames)
        {
            element.AddToClassList(className);
        }

        return element;
    }

    public static VisualElement AddStyleSheets(this VisualElement element, params string[] styleSheetNames)
    {
        foreach (string styleSheetName in styleSheetNames)
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load(styleSheetName);

            element.styleSheets.Add(styleSheet);
        }

        return element;
    }

    public static Port CreatePort(this BSMNode node, string portName = "", Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single)
    {
        Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));

        port.portName = portName;

        return port;
    }

    public static Edge ConnectNodes(BSMNode first, BSMNode second)
    {
        if (first.outputContainer == null)
            return null;
        Port outPort = null;
        foreach(Port port in first.outputContainer.Children())
        {
            if (port == null)
                continue;
            outPort = port;
            break;
        }
        if (outPort == null)
            return null;
        if (second.inputContainer == null)
            return null;
        Port inPort = null;
        foreach(Port port in second.inputContainer.Children())
        {
            if (port == null)
                continue;
            inPort = port;
            break;
        }

        return outPort.ConnectTo(inPort);
    }

    public static TextField CreateTextField(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
    {
        TextField textField = new TextField()
        {
            value = value,
            label = label
        };

        if (onValueChanged != null)
        {
            textField.RegisterValueChangedCallback(onValueChanged);
        }

        return textField;
    }

    public static TextField CreateTextArea(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
    {
        TextField textArea = CreateTextField(value, label, onValueChanged);

        textArea.multiline = true;

        return textArea;
    }

    public static List<Edge> GetAllOutEdge(BSMNode node)
    {
        List<Edge> edges = new List<Edge>();

        foreach(Port port in node.outputContainer.Children())
        {
            if (port == null)
                continue;

            foreach (var e in port.connections)
                edges.Add(e);
        }

        return edges;
    }

    public static BSMNodeType GetType(BSMNode node)
    {
        if (node is BSMNodeCondition)
            return BSMNodeType.Condition;
        else if (node is BSMNodeLabel)
            return BSMNodeType.Label;
        else if (node is BSMNodeState)
            return BSMNodeType.State;
        else if (node is BSMNodeGoto)
            return BSMNodeType.Goto;
        else Debug.LogError("Unknow node type " + node.ToString());

        return BSMNodeType.Label;
    }
}

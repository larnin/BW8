using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
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

    public static Label CreateLabel(string text, float margin = 0)
    {
        var label = new Label(text);
        if (margin > 0)
        {
            label.style.paddingBottom = margin;
            label.style.paddingLeft = margin;
            label.style.paddingRight = margin;
            label.style.paddingTop = margin;
        }
        return label;
    }

    public static Button CreateButton(string text, Action onClick = null)
    {
        Button button = new Button(onClick)
        {
            text = text
        };

        return button;
    }

    public static Foldout CreateFoldout(string title, bool collapsed = false)
    {
        Foldout foldout = new Foldout()
        {
            text = title,
            value = !collapsed
        };

        return foldout;
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

    public static FloatField CreateFloatField(float value, string title, EventCallback<ChangeEvent<float>> onValueChanged = null)
    {
        FloatField field = new FloatField()
        {
            value = value,
            label = title,
        };
        if (onValueChanged != null)
            field.RegisterValueChangedCallback(onValueChanged);
        return field;
    }

    public static IntegerField CreateIntField(int value, string title, EventCallback<ChangeEvent<int>> onValueChanged = null)
    {
        IntegerField field = new IntegerField()
        {
            value = value,
            label = title,
        };
        if (onValueChanged != null)
            field.RegisterValueChangedCallback(onValueChanged);
        return field;
    }

    public static Toggle CreateCheckbox(string title, bool toggled, EventCallback<ChangeEvent<bool>> onValueChanged = null)
    {
        Toggle checkbox = new Toggle(title)
        {
            value = toggled,
        };

        if (onValueChanged != null)
            checkbox.RegisterValueChangedCallback(onValueChanged);

        return checkbox;
    }

    public static ObjectField CreateObjectField(string title, Type objectType, bool allowSceneItems, UnityEngine.Object obj, EventCallback<ChangeEvent<UnityEngine.Object>> onValueChanged = null)
    {
        ObjectField field = new ObjectField(title)
        {
            value = obj,
            objectType = objectType,
            allowSceneObjects = allowSceneItems,
        };

        if (onValueChanged != null)
            field.RegisterValueChangedCallback(onValueChanged);

        return field;
    }

    public static PropertyField CreatePropertyField(string title, SerializedProperty property, EventCallback<SerializedPropertyChangeEvent> onValueChanged = null)
    {
        PropertyField field = new PropertyField(property, title);

        if (onValueChanged != null)
            field.RegisterValueChangeCallback(onValueChanged);

        return field;
    }

    public static VisualElement CreateHorizontalLayout()
    {
        var header = new VisualElement();
        header.style.flexDirection = FlexDirection.Row;
        header.style.justifyContent = Justify.SpaceBetween;

        return header;
    }

    public static Foldout CreateFoldout(string label)
    {
        Foldout foldout = new Foldout()
        {
            text = label,
        };

        return foldout;
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

    public static void SetContainerStyle(VisualElement element, float margin, Color borderColor, float borderWidth, float borderRadius, Color backgroundColor)
    {
        element.style.marginTop = margin;
        element.style.marginBottom = margin;
        element.style.marginLeft = margin;
        element.style.marginRight = margin;

        element.style.borderBottomColor = borderColor;
        element.style.borderLeftColor = borderColor;
        element.style.borderRightColor = borderColor;
        element.style.borderTopColor = borderColor;

        element.style.borderBottomWidth = borderWidth;
        element.style.borderLeftWidth = borderWidth;
        element.style.borderRightWidth = borderWidth;
        element.style.borderTopWidth = borderWidth;

        element.style.borderBottomLeftRadius = borderRadius;
        element.style.borderBottomRightRadius = borderRadius;
        element.style.borderTopLeftRadius = borderRadius;
        element.style.borderTopRightRadius = borderRadius;

        element.style.backgroundColor = backgroundColor;
    }
}

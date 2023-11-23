using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public static class BSMUtility
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

    public static Port CreatePort(this BSMNode node, string portName = "", Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single)
    {
        Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));

        port.portName = portName;

        return port;
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
}

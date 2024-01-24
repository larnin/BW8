using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public static class BSMUtility
{
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

    public static TextField CreateTextField(string title, string text, Action<string> callback = null)
    {
        TextField field = new TextField(title)
        {
            value = text
        };
        if (callback != null)
            field.RegisterValueChangedCallback(x => { callback(x.newValue); });
        return field;
    }

    public static Toggle CreateCheckbox(string title, bool toggled, Action<bool> callback = null)
    {
        Toggle checkbox = new Toggle(title)
        {
            value = toggled,
        };

        if (callback != null)
            checkbox.RegisterValueChangedCallback(x => { callback(x.newValue); });

        return checkbox;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class BSMAttributeView : BSMDropdownCallback
{
    BSMAttribute m_attribute;
    BSMAttributesWindow m_window;

    bool m_singleLine;
    bool m_allowSceneItems;
    Action<object> m_onValueChange;

    Button m_typeButton;
    Button m_subtypeButton;
    VisualElement m_valueContainer;

    VisualElement m_mainContainer;

    bool m_selectSubtype = false;

    public BSMAttributeView(BSMAttribute attribute, BSMAttributesWindow window, bool singleLine = false, bool allowSceneItems = false, Action<object> onValueChange = null)
    {
        m_attribute = attribute;
        m_window = window;
        m_singleLine = singleLine;
        m_allowSceneItems = allowSceneItems;
        m_onValueChange = onValueChange;
    }

    public BSMAttribute GetAttribute()
    {
        return m_attribute;
    }

    public VisualElement GetElement()
    {
        if (m_singleLine)
            m_mainContainer = BSMEditorUtility.CreateHorizontalLayout();
        else m_mainContainer = new VisualElement();

        FillMainContainer();

        return m_mainContainer;
    }

    void FillMainContainer()
    {
        m_mainContainer.Clear();

        if (m_attribute.automatic)
        {
            m_mainContainer.Add(BSMEditorUtility.CreateLabel(m_attribute.name, 4));
            if (!m_singleLine)
            {
                string type = GetTypeText();
                if (m_attribute.data.attributeType == BSMAttributeType.attributeUnityObject)
                    type = m_attribute.data.customType.Name;
                m_mainContainer.Add(BSMEditorUtility.CreateLabel("Type: " + type, 4));
            }

            m_typeButton = null;
        }
        else
        {

            TextField nameField = BSMEditorUtility.CreateTextField(m_attribute.name, null, callback =>
            {
                TextField target = (TextField)callback.target;

                target.value = callback.newValue.RemoveSpecialCharacters();

                m_attribute.name = target.value;

                if (m_window != null)
                    m_window.ProcessErrors();
            });
            m_mainContainer.Add(nameField);

            VisualElement typeContainer = BSMEditorUtility.CreateHorizontalLayout();

            typeContainer.Add(BSMEditorUtility.CreateLabel("Type", 4));

            m_typeButton = BSMEditorUtility.CreateButton(GetTypeText(), CreateTypePopup);
            m_typeButton.style.flexGrow = 2;
            typeContainer.Add(m_typeButton);

            m_mainContainer.Add(typeContainer);

            if (m_attribute.data.attributeType == BSMAttributeType.attributeUnityObject)
            {
                VisualElement subtypeContainer = BSMEditorUtility.CreateHorizontalLayout();

                subtypeContainer.Add(BSMEditorUtility.CreateLabel("Subtype", 4));

                m_subtypeButton = BSMEditorUtility.CreateButton(m_attribute.data.customType.Name, CreateSubTypePopup);
                m_subtypeButton.style.flexGrow = 2;
                subtypeContainer.Add(m_subtypeButton);

                m_mainContainer.Add(subtypeContainer);
            }
        }

        m_valueContainer = new VisualElement();
        m_mainContainer.Add(m_valueContainer);
        UpdateValue();

        UpdateStyle(false);
    }

    string GetTypeText()
    {
        return BSMAttributeData.AttributeName(m_attribute.data.attributeType);
    }

    void CreateTypePopup()
    {
        if (m_typeButton == null)
            return;

        var pos = m_typeButton.LocalToWorld(new Vector2(0, 0));
        pos.y -= 100;
        var rect = new Rect(pos, new Vector2(200, 100));

        List<string> choices = new List<string>();
        var values = Enum.GetValues(typeof(BSMAttributeType));
        foreach (var v in values)
            choices.Add(BSMAttributeData.AttributeName((BSMAttributeType)v));

        m_selectSubtype = false;

        UnityEditor.PopupWindow.Show(rect, new BSMSimpleDropdownPopup(choices, this));
    }

    List<Type> GetAllValidSubTypes()
    {
        List<Type> types = new List<Type>();

        var unityTypes = typeof(UnityEngine.Object).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(UnityEngine.Object)));
        foreach (var type in unityTypes)
        {
            if (type.IsAbstract)
                continue;
            types.Add(type);
        }

        var localTypes = typeof(BSMAttribute).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(UnityEngine.Object)));
        foreach (var type in localTypes)
        {
            if (type.IsAbstract)
                continue;
            types.Add(type);
        }

        return types;
    }

    void CreateSubTypePopup()
    {
        if (m_subtypeButton == null)
            return;

        var pos = m_subtypeButton.LocalToWorld(new Vector2(0, 0));
        pos.y -= 100;
        var rect = new Rect(pos, new Vector2(200, 100));

        List<string> choices = new List<string>();
        var types = GetAllValidSubTypes();
        foreach (var type in types)
            choices.Add(type.Name);

        m_selectSubtype = true;

        UnityEditor.PopupWindow.Show(rect, new BSMSearchDropdownPopup(choices, this));
    }

    public void SendResult(int result)
    {
        bool updateContainer = false;
        if (m_selectSubtype)
        {
            var types = GetAllValidSubTypes();
            if(result >= 0 && result < types.Count)
            {
                m_attribute.data.customType = types[result];
                m_attribute.data.data = null;

                m_subtypeButton.text = types[result].Name;
            }
        }
        else
        {
            var newType = (BSMAttributeType)result;
            if (newType == BSMAttributeType.attributeUnityObject || m_attribute.data.attributeType == BSMAttributeType.attributeUnityObject)
                updateContainer = true;

            m_attribute.data.SetType(newType);
            m_typeButton.text = GetTypeText();
        }

        if (updateContainer)
            FillMainContainer();
        else UpdateValue();
    }

    void UpdateValue()
    {
        m_valueContainer.Clear();

        var element = CreateElement(m_attribute.data, m_allowSceneItems, m_onValueChange);

        if(element != null)
        {
            VisualElement layout = BSMEditorUtility.CreateHorizontalLayout();
            if (!m_singleLine)
                layout.Add(BSMEditorUtility.CreateLabel("Value", 4));

            element.style.flexGrow = 2;
            layout.Add(element);

            m_valueContainer.Add(layout);
        }
    }

    public void UpdateStyle(bool error)
    {
        if (m_mainContainer == null)
            return;

        if(error)
            BSMEditorUtility.SetContainerStyle(m_mainContainer, 2, BSMNode.errorBorderColor, 1, 3, BSMNode.errorBackgroundColor);
        else BSMEditorUtility.SetContainerStyle(m_mainContainer, 2, new Color(0.4f, 0.4f, 0.4f), 1, 3, new Color(0.15f, 0.15f, 0.15f));
    }

    public static VisualElement CreateElement(BSMAttributeData data, bool allowSceneItems, Action<object> onValueChangeCallback)
    {
        switch (data.attributeType)
        {
            case BSMAttributeType.attributeFloat:
                {
                    FloatField valueField = BSMEditorUtility.CreateFloatField(data.GetFloat(0), null, callback =>
                    {
                        FloatField target = (FloatField)callback.target;

                        target.value = callback.newValue;

                        data.SetFloat(target.value);

                        if (onValueChangeCallback != null)
                            onValueChangeCallback(data.data);
                    });

                    return valueField;
                }
            case BSMAttributeType.attributeInt:
                {
                    IntegerField valueField = BSMEditorUtility.CreateIntField(data.GetInt(0), null, callback =>
                    {
                        IntegerField target = (IntegerField)callback.target;

                        target.value = callback.newValue;

                        data.SetInt(target.value);

                        if (onValueChangeCallback != null)
                            onValueChangeCallback(data.data);
                    });

                    return valueField;
                }
            case BSMAttributeType.attributeBool:
                {
                    string value = data.GetBool(false) ? "True" : "False";
                    Button valueButton = BSMEditorUtility.CreateButton(value);
                    valueButton.clicked += () =>
                    {
                        bool oldValue = data.GetBool(false);
                        bool newValue = !oldValue;

                        valueButton.text = newValue ? "True" : "False";

                        data.SetBool(newValue);

                        if (onValueChangeCallback != null)
                            onValueChangeCallback(data.data);
                    };

                    return valueButton;
                }
            case BSMAttributeType.attributeString:
                {
                    TextField nameField = BSMEditorUtility.CreateTextField(data.GetString(""), null, callback =>
                    {
                        TextField target = (TextField)callback.target;

                        target.value = callback.newValue;

                        data.SetString(target.value);

                        if (onValueChangeCallback != null)
                            onValueChangeCallback(data.data);
                    });

                    return nameField;
                }
            case BSMAttributeType.attributeUnityObject:
                {
                    ObjectField field = BSMEditorUtility.CreateObjectField("", data.customType, allowSceneItems, data.GetUnityObject(data.customType), callback =>
                    {
                        ObjectField target = (ObjectField)callback.target;

                        var gameObject = callback.newValue;

                        target.value = gameObject;

                        data.SetUnityObject(data.customType, gameObject);

                        if (onValueChangeCallback != null)
                            onValueChangeCallback(data.data);
                    });

                    return field;
                }
            case BSMAttributeType.attributeVector2:
                {
                    Vector2Field field = BSMEditorUtility.CreateVector2Field(data.GetVector2(), null, callback =>
                    {
                        Vector2Field target = (Vector2Field)callback.target;

                        target.value = callback.newValue;

                        data.SetVector2(target.value);

                        if (onValueChangeCallback != null)
                            onValueChangeCallback(data.data);
                    });

                    return field;
                }
            case BSMAttributeType.attributeVector3:
                {
                    Vector3Field field = BSMEditorUtility.CreateVector3Field(data.GetVector3(), null, callback =>
                    {
                        Vector3Field target = (Vector3Field)callback.target;

                        target.value = callback.newValue;

                        data.SetVector3(target.value);

                        if (onValueChangeCallback != null)
                            onValueChangeCallback(data.data);
                    });

                    return field;
                }
            case BSMAttributeType.attributeVector2Int:
                {
                    Vector2IntField field = BSMEditorUtility.CreateVector2IntField(data.GetVector2Int(), null, callback =>
                    {
                        Vector2IntField target = (Vector2IntField)callback.target;

                        target.value = callback.newValue;

                        data.SetVector2Int(target.value);

                        if (onValueChangeCallback != null)
                            onValueChangeCallback(data.data);
                    });

                    return field;
                }
            case BSMAttributeType.attributeVector3Int:
                {
                    Vector3IntField field = BSMEditorUtility.CreateVector3IntField(data.GetVector3Int(), null, callback =>
                    {
                        Vector3IntField target = (Vector3IntField)callback.target;

                        target.value = callback.newValue;

                        data.SetVector3Int(target.value);

                        if (onValueChangeCallback != null)
                            onValueChangeCallback(data.data);
                    });

                    return field;
                }

            default:
                {
                    Debug.LogError("Unknow BSM Attribute type");
                    break;
                }
        }

        return null;
    }
}

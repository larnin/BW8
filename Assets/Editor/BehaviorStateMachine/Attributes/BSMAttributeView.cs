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

    Button m_typeButton;
    VisualElement m_valueContainer;

    public BSMAttributeView(BSMAttribute attribute)
    {
        m_attribute = attribute;
    }

    public BSMAttribute GetAttribute()
    {
        return m_attribute;
    }

    public VisualElement GetElement()
    {
        VisualElement element = new VisualElement();
        BSMEditorUtility.SetContainerStyle(element, 2, new Color(0.4f, 0.4f, 0.4f), 1, 3, new Color(0.15f, 0.15f, 0.15f));

        if (m_attribute.automatic)
        {
            element.Add(BSMEditorUtility.CreateLabel(m_attribute.name, 4));
            element.Add(BSMEditorUtility.CreateLabel("Type: " + GetTypeText()));

            m_typeButton = null;
        }
        else
        {

            TextField nameField = BSMEditorUtility.CreateTextField(m_attribute.name, null, callback =>
            {
                TextField target = (TextField)callback.target;

                target.value = callback.newValue.RemoveSpecialCharacters();

                m_attribute.name = target.value;
            });
            element.Add(nameField);

            VisualElement typeContainer = BSMEditorUtility.CreateHorizontalLayout();

            typeContainer.Add(BSMEditorUtility.CreateLabel("Type", 4));

            m_typeButton = BSMEditorUtility.CreateButton(GetTypeText(), CreateTypePopup);
            m_typeButton.style.flexGrow = 2;
            typeContainer.Add(m_typeButton);

            element.Add(typeContainer);
        }

        m_valueContainer = new VisualElement();
        element.Add(m_valueContainer);
        UpdateValue();

        return element;
    }

    string GetTypeText()
    {
        return BSMAttribute.AttributeName(m_attribute.attributeType);
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
            choices.Add(BSMAttribute.AttributeName((BSMAttributeType)v));

        UnityEditor.PopupWindow.Show(rect, new BSMSimpleDropdownPopup(choices, this));
    }

    public void SendResult(int result)
    {
        m_attribute.SetType((BSMAttributeType)result);
        m_typeButton.text = GetTypeText();

        UpdateValue();
    }

    void UpdateValue()
    {
        m_valueContainer.Clear();

        VisualElement layout = null;
        if (m_attribute.attributeType != BSMAttributeType.attributeGameObject)
        {
            layout = BSMEditorUtility.CreateHorizontalLayout();
            layout.Add(BSMEditorUtility.CreateLabel("Value"));
        }

        switch(m_attribute.attributeType)
        {
            case BSMAttributeType.attributeFloat:
                {
                    IntegerField valueField = BSMEditorUtility.CreateIntField(m_attribute.GetInt(0), null, callback =>
                    {
                        IntegerField target = (IntegerField)callback.target;

                        target.value = callback.newValue;

                        m_attribute.SetInt(target.value);
                    });

                    valueField.style.flexGrow = 2;
                    layout.Add(valueField);
                    break;
                }
            case BSMAttributeType.attributeInt:
                {
                    IntegerField valueField = BSMEditorUtility.CreateIntField(m_attribute.GetInt(0), null, callback =>
                    {
                        IntegerField target = (IntegerField)callback.target;

                        target.value = callback.newValue;

                        m_attribute.SetInt(target.value);
                    });

                    valueField.style.flexGrow = 2;
                    layout.Add(valueField);
                    break;
                }
            case BSMAttributeType.attributeString:
                {
                    TextField nameField = BSMEditorUtility.CreateTextField(m_attribute.GetString(""), null, callback =>
                    {
                        TextField target = (TextField)callback.target;

                        target.value = callback.newValue;

                        m_attribute.SetString(target.value);
                    });

                    nameField.style.flexGrow = 2;
                    layout.Add(nameField);
                    break;
                }
            case BSMAttributeType.attributeGameObject:
                {

                    break;
                }
            default:
                {
                    Debug.LogError("Unknow BSM Attribute type");
                    break;
                }
        }

        if (layout != null)
            m_valueContainer.Add(layout);
    }

}

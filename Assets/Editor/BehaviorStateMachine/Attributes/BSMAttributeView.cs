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

    Button m_typeButton;
    VisualElement m_valueContainer;

    VisualElement m_mainContainer;

    public BSMAttributeView(BSMAttribute attribute, BSMAttributesWindow window)
    {
        m_attribute = attribute;
        m_window = window;
    }

    public BSMAttribute GetAttribute()
    {
        return m_attribute;
    }

    public VisualElement GetElement()
    {
        m_mainContainer = new VisualElement();

        if (m_attribute.automatic)
        {
            m_mainContainer.Add(BSMEditorUtility.CreateLabel(m_attribute.name, 4));
            m_mainContainer.Add(BSMEditorUtility.CreateLabel("Type: " + GetTypeText()));

            m_typeButton = null;
        }
        else
        {

            TextField nameField = BSMEditorUtility.CreateTextField(m_attribute.name, null, callback =>
            {
                TextField target = (TextField)callback.target;

                target.value = callback.newValue.RemoveSpecialCharacters();

                m_attribute.name = target.value;

                m_window.ProcessErrors();
            });
            m_mainContainer.Add(nameField);

            VisualElement typeContainer = BSMEditorUtility.CreateHorizontalLayout();

            typeContainer.Add(BSMEditorUtility.CreateLabel("Type", 4));

            m_typeButton = BSMEditorUtility.CreateButton(GetTypeText(), CreateTypePopup);
            m_typeButton.style.flexGrow = 2;
            typeContainer.Add(m_typeButton);

            m_mainContainer.Add(typeContainer);
        }

        m_valueContainer = new VisualElement();
        m_mainContainer.Add(m_valueContainer);
        UpdateValue();

        UpdateStyle(false);

        return m_mainContainer;
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

        UnityEditor.PopupWindow.Show(rect, new BSMSimpleDropdownPopup(choices, this));
    }

    public void SendResult(int result)
    {
        m_attribute.data.SetType((BSMAttributeType)result);
        m_typeButton.text = GetTypeText();

        UpdateValue();
    }

    void UpdateValue()
    {
        m_valueContainer.Clear();

        VisualElement layout = null;
        if (m_attribute.data.attributeType != BSMAttributeType.attributeGameObject)
        {
            layout = BSMEditorUtility.CreateHorizontalLayout();
            layout.Add(BSMEditorUtility.CreateLabel("Value"));
        }

        switch(m_attribute.data.attributeType)
        {
            case BSMAttributeType.attributeFloat:
                {
                    FloatField valueField = BSMEditorUtility.CreateFloatField(m_attribute.data.GetFloat(0), null, callback =>
                    {
                        FloatField target = (FloatField)callback.target;

                        target.value = callback.newValue;

                        m_attribute.data.SetFloat(target.value);
                    });

                    valueField.style.flexGrow = 2;
                    layout.Add(valueField);
                    break;
                }
            case BSMAttributeType.attributeInt:
                {
                    IntegerField valueField = BSMEditorUtility.CreateIntField(m_attribute.data.GetInt(0), null, callback =>
                    {
                        IntegerField target = (IntegerField)callback.target;

                        target.value = callback.newValue;

                        m_attribute.data.SetInt(target.value);
                    });

                    valueField.style.flexGrow = 2;
                    layout.Add(valueField);
                    break;
                }
            case BSMAttributeType.attributeString:
                {
                    TextField nameField = BSMEditorUtility.CreateTextField(m_attribute.data.GetString(""), null, callback =>
                    {
                        TextField target = (TextField)callback.target;

                        target.value = callback.newValue;

                        m_attribute.data.SetString(target.value);
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

    public void UpdateStyle(bool error)
    {
        if (m_mainContainer == null)
            return;

        if(error)
            BSMEditorUtility.SetContainerStyle(m_mainContainer, 2, BSMNode.errorBorderColor, 1, 3, BSMNode.errorBackgroundColor);
        else BSMEditorUtility.SetContainerStyle(m_mainContainer, 2, new Color(0.4f, 0.4f, 0.4f), 1, 3, new Color(0.15f, 0.15f, 0.15f));
    }

}

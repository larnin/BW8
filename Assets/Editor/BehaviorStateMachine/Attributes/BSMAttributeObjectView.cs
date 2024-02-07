using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class BSMAttributeObjectView : BSMDropdownCallback
{
    BSMAttributeObject m_object;
    BSMNode m_node;

    VisualElement m_parentElement;
    Button m_attributeButton;

    public BSMAttributeObjectView(BSMAttributeObject obj, BSMNode node)
    {
        m_object = obj;
        m_node = node;
    }

    public BSMAttributeObject GetObject()
    {
        return m_object;
    }

    public VisualElement GetElement()
    {
        m_parentElement = new VisualElement();
        m_parentElement.style.flexGrow = 2;
        Draw();

        return m_parentElement;
    }

    public void SendResult(int result)
    {
        var attributes = m_node.GetGraph().GetWindow().GetAttributesWindow().GetAttributes();

        int index = 0;
        foreach (var a in attributes)
        {
            if (a.data.attributeType != m_object.data.attributeType)
                continue;

            if(index == result)
            {
                m_object.attributeID = a.ID;
                break;
            }

            index++;
        }

        Draw();
    }

    void Draw()
    {
        if (m_parentElement == null)
            return;

        m_parentElement.Clear();

        VisualElement horizontal = BSMEditorUtility.CreateHorizontalLayout();

        var box = BSMEditorUtility.CreateCheckbox("", m_object.useAttribute, callback =>
        {
            Toggle target = (Toggle)callback.target;

            target.value = callback.newValue;

            m_object.useAttribute = target.value;

            Draw();
        });

        horizontal.Add(box);

        m_attributeButton = null;
        if (m_object.useAttribute)
        {
            m_attributeButton = BSMEditorUtility.CreateButton(GetAttributeName(), CreatePopup);
            m_attributeButton.style.flexGrow = 2;
            horizontal.Add(m_attributeButton);
        }
        else DrawAttribute(horizontal);

        m_parentElement.Add(horizontal);
    }

    void CreatePopup()
    {
        var attributes = m_node.GetGraph().GetWindow().GetAttributesWindow().GetAttributes();

        List<string> attributesNames = new List<string>();

        foreach(var a in attributes)
        {
            if (a.data.attributeType != m_object.data.attributeType)
                continue;

            attributesNames.Add(a.name);
        }

        var pos = m_attributeButton.LocalToWorld(new Vector2(0, 0));
        pos.y -= 100;
        var rect = new Rect(pos, new Vector2(200, 100));

        UnityEditor.PopupWindow.Show(rect, new BSMSearchDropdownPopup(attributesNames, this));
    }

    string GetAttributeName()
    {
        var attributes = m_node.GetGraph().GetWindow().GetAttributesWindow().GetAttributes();

        foreach(var a in attributes)
        {
            if (a.ID == m_object.attributeID)
                return a.name;
        }

        return "Not Set";
    }

    void UpdateAttributeName()
    {
        if (m_attributeButton == null)
            return;
        m_attributeButton.text = GetAttributeName();
    }

    void DrawAttribute(VisualElement layout)
    {
        switch (m_object.data.attributeType)
        {
            case BSMAttributeType.attributeFloat:
                {
                    FloatField valueField = BSMEditorUtility.CreateFloatField(m_object.data.GetFloat(0), null, callback =>
                    {
                        FloatField target = (FloatField)callback.target;

                        target.value = callback.newValue;

                        m_object.data.SetFloat(target.value);
                    });

                    valueField.style.flexGrow = 2;
                    layout.Add(valueField);
                    break;
                }
            case BSMAttributeType.attributeInt:
                {
                    IntegerField valueField = BSMEditorUtility.CreateIntField(m_object.data.GetInt(0), null, callback =>
                    {
                        IntegerField target = (IntegerField)callback.target;

                        target.value = callback.newValue;

                        m_object.data.SetInt(target.value);
                    });

                    valueField.style.flexGrow = 2;
                    layout.Add(valueField);
                    break;
                }
            case BSMAttributeType.attributeString:
                {
                    TextField nameField = BSMEditorUtility.CreateTextField(m_object.data.GetString(""), null, callback =>
                    {
                        TextField target = (TextField)callback.target;

                        target.value = callback.newValue;

                        m_object.data.SetString(target.value);
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class BSMAttributesWindow
{
    VisualElement m_parent;

    List<BSMAttributeView> m_attributes = new List<BSMAttributeView>();

    public BSMAttributesWindow()
    {
        AddAutomaticAttributes();
    }

    public void SetParent(VisualElement parent)
    {
        m_parent = parent;
        Draw();
    }

    void Draw()
    {
        if (m_parent == null)
            return;

        m_parent.Clear();

        var label = new Label("Attributes");
        m_parent.Add(label);

        var scrollView = new ScrollView();
        m_parent.Add(scrollView);

        Foldout foldout = BSMEditorUtility.CreateFoldout("Automatic Attributes");
        DrawAttributes(foldout.contentContainer, true);
        scrollView.contentContainer.Add(foldout);

        scrollView.contentContainer.Add(BSMEditorUtility.CreateLabel("User Attributes"));

        DrawAttributes(scrollView.contentContainer, false);

        scrollView.contentContainer.Add(BSMEditorUtility.CreateButton("Add Attribute", AddAttribute));
    }

    void DrawAttributes(VisualElement parent, bool automaticAttributes)
    {
        for(int i = 0; i < m_attributes.Count; i++)
        {
            var attribute = m_attributes[i];

            if (attribute.GetAttribute().automatic != automaticAttributes)
                continue;

            if (automaticAttributes)
                parent.Add(attribute.GetElement());
            else
            {
                VisualElement container = BSMEditorUtility.CreateHorizontalLayout();
                var attributeElement = attribute.GetElement();
                attributeElement.style.flexGrow = 2;
                container.Add(attributeElement);

                int attributeIndex = i;
                Button button = BSMEditorUtility.CreateButton("X", () => { RemoveAttribute(attributeIndex); });
                button.style.width = 20;
                container.Add(button);

                parent.Add(container);
            }
        }
    }

    void RemoveAttribute(int index)
    {
        if (index < 0 || index >= m_attributes.Count)
            return;

        m_attributes.RemoveAt(index);

        Draw();
    }

    void AddAttribute()
    {
        BSMAttribute attribute = new BSMAttribute();
        attribute.name = "New attribute";

        BSMAttributeView view = new BSMAttributeView(attribute);
        m_attributes.Add(view);

        Draw();
    }

    void AddAutomaticAttributes()
    {
        var names = new string[] {"Aggro"};
        var types = new BSMAttributeType[] { BSMAttributeType.attributeGameObject };

        int nbAttribute = (names.Length < types.Length) ? names.Length : types.Length;
        for(int i = 0; i < nbAttribute; i++)
        {
            bool found = false;
            foreach(var v in m_attributes)
            {
                var a = v.GetAttribute();
                if (a.automatic && a.name == names[i] && a.attributeType == types[i])
                {
                    found = true;
                    break;
                }
            }

            if (found)
                continue;

            BSMAttribute attribute = new BSMAttribute();
            attribute.automatic = true;
            attribute.attributeType = types[i];
            attribute.name = names[i];

            BSMAttributeView view = new BSMAttributeView(attribute);
            m_attributes.Add(view);
        }
    }

    public void Save(BSMSaveData saveData)
    {
        saveData.attributes.Clear();

        foreach (var attribute in m_attributes)
            saveData.attributes.Add(attribute.GetAttribute());
    }

    public void Load(BSMSaveData saveData)
    {
        m_attributes.Clear();

        foreach(var attribute in saveData.attributes)
        {
            BSMAttributeView view = new BSMAttributeView(attribute);
            m_attributes.Add(view);
        }

        AddAutomaticAttributes();
        Draw();
    }
}


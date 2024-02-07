using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class BSMAttributesWindow
{
    BSMGraph m_editorWindow;

    VisualElement m_parent;

    List<BSMAttributeView> m_attributes = new List<BSMAttributeView>();

    public BSMAttributesWindow(BSMGraph editorWindow)
    {
        m_editorWindow = editorWindow;

        AddAutomaticAttributes();

        ProcessErrors();
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

        ProcessErrors();
    }

    void AddAttribute()
    {
        BSMAttribute attribute = new BSMAttribute();
        attribute.name = "New attribute";

        BSMAttributeView view = new BSMAttributeView(attribute, this);
        m_attributes.Add(view);

        Draw();

        ProcessErrors();
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
                if (a.automatic && a.name == names[i] && a.data.attributeType == types[i])
                {
                    found = true;
                    break;
                }
            }

            if (found)
                continue;

            BSMAttribute attribute = new BSMAttribute();
            attribute.automatic = true;
            attribute.data.attributeType = types[i];
            attribute.name = names[i];

            BSMAttributeView view = new BSMAttributeView(attribute, this);
            m_attributes.Add(view);
        }

        for(int i = 0; i < m_attributes.Count; i++)
        {
            var attribute = m_attributes[i].GetAttribute();
            if (!attribute.automatic)
                continue;

            bool found = false;
            for(int j = 0; j < nbAttribute; j++)
            {
                if(attribute.name == names[i] && attribute.data.attributeType == types[i])
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                attribute.automatic = false;
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
            BSMAttributeView view = new BSMAttributeView(attribute, this);
            m_attributes.Add(view);
        }

        AddAutomaticAttributes();
        Draw();

        ProcessErrors();
    }

    public void ProcessErrors()
    {
        ClearErrors();

        foreach (var attribute in m_attributes)
            attribute.UpdateStyle(false);

        for (int i = 0; i < m_attributes.Count; i++)
        {
            int namesError = 1;

            var attribute = m_attributes[i].GetAttribute();

            for (int j = i + 1; j < m_attributes.Count; j++)
            {
                if (m_attributes[j].GetAttribute().name == attribute.name)
                {
                    if (namesError == 1)
                        m_attributes[i].UpdateStyle(true);
                    m_attributes[j].UpdateStyle(true);

                    namesError++;
                }
            }

            if (namesError > 1)
                AddError(namesError + " Attributes are named " + attribute.name + ", names must be unique");
        }
    }

    void AddError(string error)
    {
        if (m_editorWindow != null)
            m_editorWindow.AddError(error, "Attribute");
    }

    void ClearErrors()
    {
        if (m_editorWindow != null)
            m_editorWindow.ClearErrors("Attribute");
    }

    public List<BSMAttribute> GetAttributes()
    {
        List<BSMAttribute> list = new List<BSMAttribute>();

        foreach (var a in m_attributes)
            list.Add(a.GetAttribute());

        return list;
    }
}


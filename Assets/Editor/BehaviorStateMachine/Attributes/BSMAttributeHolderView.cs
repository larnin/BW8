using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public abstract class BSMAttributeHolderView
{
    protected BSMNode m_node;

    class BSMAttributeNamedObjectView
    {
        public string name;
        public BSMAttributeObjectView attribute;
    }

    BSMAttributeHolder m_attributeHolder;

    List<BSMAttributeNamedObjectView> m_objects = new List<BSMAttributeNamedObjectView>();

    public BSMAttributeHolderView(BSMNode node)
    {
        m_node = node;
    }

    protected void SetAttributeHolder(BSMAttributeHolder attributeHolder)
    {
        m_attributeHolder = attributeHolder;

        var attributes = m_attributeHolder.GetAttributes();

        m_objects.Clear();

        if(attributes != null)
        {
            foreach (var attribute in attributes)
            {
                BSMAttributeObjectView view = new BSMAttributeObjectView(attribute.attribute, m_node);
                BSMAttributeNamedObjectView namedView = new BSMAttributeNamedObjectView();
                namedView.attribute = view;
                namedView.name = attribute.name;

                m_objects.Add(namedView);
            }
        }
        
    }

    public VisualElement GetElement()
    {
        VisualElement holder = new VisualElement();

        VisualElement attributesHolder = new VisualElement();

        foreach(var attribute in m_objects)
        {
            VisualElement layout = BSMEditorUtility.CreateHorizontalLayout();
            layout.Add(BSMEditorUtility.CreateLabel(attribute.name, 4));

            VisualElement objElement = attribute.attribute.GetElement();
            objElement.style.flexGrow = 2;
            layout.Add(objElement);

            attributesHolder.Add(layout);
        }

        holder.Add(attributesHolder);

        var objectElement = GetObjectElement();
        if (objectElement != null)
            holder.Add(objectElement);

        var actionsElement = GetActionsElement();
        if (actionsElement != null)
            holder.Add(actionsElement);

        return holder;
    }

    protected abstract VisualElement GetActionsElement();

    protected abstract VisualElement GetObjectElement();
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(BSMControler))]
public class BSMControlerView : Editor
{
    VisualElement m_attributeContainer;
    List<BSMAttributeView> m_attributes = new List<BSMAttributeView>();

    private void OnEnable()
    {
        LoadControler();
    }

    public override VisualElement CreateInspectorGUI()
    {
        var obj = target as BSMControler;
        if (obj == null)
            return null;

        VisualElement layout = new VisualElement();

        var behaviourElement = BSMEditorUtility.CreateObjectField("Behaviour", typeof(BSMScriptableObject), false, obj.behaviour, OnBehaviourSet);
        layout.Add(behaviourElement);

        layout.Add(BSMEditorUtility.CreateLabel("Attributes", 4));

        m_attributeContainer = new VisualElement();
        layout.Add(m_attributeContainer);

        LoadControler();

        return layout;
    }

    void OnBehaviourSet(ChangeEvent<UnityEngine.Object> change)
    {
        var obj = target as BSMControler;
        if (obj == null)
            return;

        obj.behaviour = change.newValue as BSMScriptableObject;

        var element = change.target as ObjectField;
        if (element != null)
            element.value = obj.behaviour;

        LoadControler();

        EditorUtility.SetDirty(obj);
    }

    void LoadControler()
    {
        var obj = target as BSMControler;
        if (obj == null)
            return;

        obj.LoadFromEditor();

        LoadAttributes();
    }

    void LoadAttributes()
    {
        if (m_attributeContainer == null)
            return;

        var obj = target as BSMControler;
        if (obj == null)
            return;

        m_attributeContainer.Clear();
        m_attributes.Clear();
        int nbAttributes = obj.GetAttributeNB();
        for(int i = 0; i < nbAttributes; i++)
        {
            var a = obj.GetAttributeFromIndex(i);
            if (a == null)
                continue;
            if (a.automatic)
                continue;

            var attribute = a.Clone();
            attribute.automatic = true;

            if (obj.HaveControlerAttribute(attribute.ID))
                attribute.data.data = obj.GetControlerAttribute(attribute.ID);

            int index = m_attributes.Count;
            var view = new BSMAttributeView(attribute, null, true, true, (x) => OnAttributeChange(index, x));
            m_attributes.Add(view);

            var attributeElt = view.GetElement();
            m_attributeContainer.Add(attributeElt);
        }
    }

    void OnAttributeChange(int index, object data)
    {
        var obj = target as BSMControler;
        if (obj == null)
            return;

        if (index < 0 || index >= m_attributes.Count)
            return;

        var a = m_attributes[index].GetAttribute();

        obj.SetControlerAttribute(a.ID, a.data.data);

        EditorUtility.SetDirty(obj);
    }
}

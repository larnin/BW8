using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class BSMErrorWindow
{
    VisualElement m_parent;
    List<string> m_lines = new List<string>();

    public void SetParent(VisualElement parent)
    {
        m_parent = parent;
        Draw();
    }

    public void AddError(string value)
    {
        m_lines.Add(value);
        Draw();
    }

    public void ClearErrors()
    {
        m_lines.Clear();
        Draw();
    }

    void Draw()
    {
        if (m_parent == null)
            return;

        m_parent.Clear();

        var label = new Label("Errors");
        m_parent.Add(label);

        var scrollView = new ScrollView();
        m_parent.Add(scrollView);

        foreach(var l in m_lines)
        {
            var line = new Label(l);
            scrollView.Add(line);
        }
    }
}

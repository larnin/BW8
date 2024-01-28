using NLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public interface BSMStateNodePopupCallback
{
    public void SetState(BSMStateBase state);
}

public class BSMStateNodePopup : PopupWindowContent
{
    string m_filter = "";
    BSMStateNodePopupCallback m_node;

    Vector2 m_scrollPos = Vector2.zero;

    public BSMStateNodePopup(BSMStateNodePopupCallback node)
    {
        m_node = node;
    }

    public override void OnGUI(Rect rect)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Filter:", GUILayout.Width(40));
        m_filter = GUILayout.TextField(m_filter);
        GUILayout.EndHorizontal();

        var types = typeof(BSMStateBase).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(BSMStateBase)));

        m_scrollPos = GUILayout.BeginScrollView(m_scrollPos);
        foreach (var t in types)
        {
            string name = BSMStateBase.GetName(t);

            if (!Loc.ProcessFilter(name, m_filter))
                continue;

            if (GUILayout.Button(name))
            {
                SetState(t);
                editorWindow.Close();
            }
        }
        GUILayout.EndScrollView();
    }

    void SetState(Type t)
    {
        if (m_node == null)
            return;

        var state = Activator.CreateInstance(t) as BSMStateBase;
        if (state != null)
            m_node.SetState(state);
    }
}

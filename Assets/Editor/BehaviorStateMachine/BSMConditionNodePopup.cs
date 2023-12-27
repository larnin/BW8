using NLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class BSMConditionNodePopup : PopupWindowContent
{
    string m_filter = "";
    BSMConditionNode m_node;

    Vector2 m_scrollPos = Vector2.zero;

    public BSMConditionNodePopup(BSMConditionNode node)
    {
        m_node = node;
    }

    public override void OnGUI(Rect rect)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Filter:", GUILayout.Width(40));
        m_filter = GUILayout.TextField(m_filter);
        GUILayout.EndHorizontal();

        var types = typeof(BSMConditionBase).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(BSMConditionBase)));

        m_scrollPos = GUILayout.BeginScrollView(m_scrollPos);
        foreach (var t in types)
        {
            if (!Loc.ProcessFilter(t.Name, m_filter))
                continue;

            if (GUILayout.Button(t.Name))
            {
                SetCondition(t);
                editorWindow.Close();
            }
        }
        GUILayout.EndScrollView();
    }

    void SetCondition(Type t)
    {
        if (m_node == null)
            return;

        //todo
    }
}

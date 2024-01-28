using NLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class BSMSearchDropdownPopup : PopupWindowContent
{
    string m_filter = "";
    List<string> m_values = new List<string>();
    BSMDropdownCallback m_callback;

    Vector2 m_scrollPos = Vector2.zero;

    public BSMSearchDropdownPopup(List<string> values, BSMDropdownCallback callback)
    {
        m_values = values;
        m_callback = callback;
    }

    public override void OnGUI(Rect rect)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Filter:", GUILayout.Width(40));
        m_filter = GUILayout.TextField(m_filter);
        GUILayout.EndHorizontal();

        m_scrollPos = GUILayout.BeginScrollView(m_scrollPos);

        for (int i = 0; i < m_values.Count; i++)
        {
            if (!Loc.ProcessFilter(m_values[i], m_filter))
                continue;

            if (GUILayout.Button(m_values[i]))
            {
                if (m_callback != null)
                    m_callback.SendResult(i);
                editorWindow.Close();
            }
        }

        GUILayout.EndScrollView();
    }
}

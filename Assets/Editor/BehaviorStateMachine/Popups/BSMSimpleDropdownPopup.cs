using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public interface BSMDropdownCallback
{
    public void SendResult(int result);
}


public class BSMSimpleDropdownPopup : PopupWindowContent
{
    List<string> m_values = new List<string>();
    BSMDropdownCallback m_callback;

    Vector2 m_scrollPos = Vector2.zero;

    public BSMSimpleDropdownPopup(List<string> values, BSMDropdownCallback callback)
    {
        m_values = values;
        m_callback = callback;
    }

    public override void OnGUI(Rect rect)
    {
        m_scrollPos = GUILayout.BeginScrollView(m_scrollPos);

        for (int i = 0; i < m_values.Count; i++)
        {
            if(GUILayout.Button(m_values[i]))
            {
                if (m_callback != null)
                    m_callback.SendResult(i);
                editorWindow.Close();
            }

        }

        GUILayout.EndScrollView();
    }
}

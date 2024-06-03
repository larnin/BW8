using NLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public interface BSMAddActionPopupCallback
{
    public void AddAction(string name, BSMActionBase action);
}

public class BSMAddActionPopup : PopupWindowContent, BSMDropdownCallback
{
    List<string> m_names;
    BSMAddActionPopupCallback m_callback;

    int m_currentNameIndex = 0;

    string m_filter = "";
    Vector2 m_scrollPos = Vector2.zero;

    public BSMAddActionPopup(BSMAddActionPopupCallback callback)
    {
        m_names = new List<string>();
        m_callback = callback;
    }

    public BSMAddActionPopup(List<string> names, BSMAddActionPopupCallback callback)
    {
        m_names = names;
        m_callback = callback;
    }

    public override void OnGUI(Rect rect)
    {
        GUILayout.Label("Add action");

        if(m_names.Count >= 1)
        {
            if (m_currentNameIndex < 0 || m_currentNameIndex >= m_names.Count)
                m_currentNameIndex = 0;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Group: ");
            if (GUILayout.Button(m_names[m_currentNameIndex]))
                CreateCategoryPopup();
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Filter:", GUILayout.Width(40));
        m_filter = GUILayout.TextField(m_filter);
        GUILayout.EndHorizontal();

        var types = typeof(BSMActionBase).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(BSMActionBase)));

        m_scrollPos = GUILayout.BeginScrollView(m_scrollPos);
        foreach (var t in types)
        {
            string name = BSMActionBase.GetName(t);

            if (!Loc.ProcessFilter(name, m_filter))
                continue;

            if (GUILayout.Button(name))
            {
                if (m_names == null || m_names.Count == 0)
                    AddAction("", t);
                else AddAction(m_names[m_currentNameIndex], t);
                editorWindow.Close();
            }
        }
        GUILayout.EndScrollView();
    }

    void CreateCategoryPopup()
    {
        var rect = GUILayoutUtility.GetLastRect();
        rect.x += 10;
        rect.y += 10;

        UnityEditor.PopupWindow.Show(rect, new BSMSearchDropdownPopup(m_names, this));
    }

    public void SendResult(int result)
    {
        if (m_names == null)
            return;

        if (result >= 0 && result < m_names.Count)
            m_currentNameIndex = result;
    }

    void AddAction(string name, Type type)
    {
        if (m_callback == null)
            return;

        var action = Activator.CreateInstance(type) as BSMActionBase;
        if (action != null)
            m_callback.AddAction(name, action);
    }
}

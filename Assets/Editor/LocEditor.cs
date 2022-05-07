using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace NLocalization
{
    public class LocEditor : OdinMenuEditorWindow
    {
        [MenuItem("Game/Localization")]
        public static void OpenWindow()
        {
            GetWindow<LocEditor>().Show();
        }

        enum Tabs
        {
            Languages,
            Exports,
            Stats,
            Settings,
        }

        private LocList m_langs;
        private int m_currentTab;

        public LocList locList { get { return m_langs; } }

        protected override void OnEnable()
        {
            m_langs = new LocList();
            m_langs.Load();

            base.OnEnable();
        }

        protected override void OnGUI()
        {
            int newTab = GUILayout.Toolbar(m_currentTab, Enum.GetNames(typeof(Tabs)));
            if (newTab != m_currentTab)
            {
                m_currentTab = newTab;
                ChangeTab();
            }

            base.OnGUI();
        }

        public void ChangeTab()
        {
            ForceMenuTreeRebuild();
        }


        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            Tabs tab = (Tabs)m_currentTab;

            switch(tab)
            {
                case Tabs.Languages:
                    BuildMenuTreeLanguages(tree);
                    break;
                case Tabs.Exports:
                    BuildMenuTreeExports(tree);
                    break;
                case Tabs.Settings:
                    BuildMenuTreeSettings(tree);
                    break;
                case Tabs.Stats:
                    BuildMenuTreeStats(tree);
                    break;
                default:
                    Debug.LogError("No tree for " + tab.ToString() + " tab");
                    break;
            }

            return tree;
        }

        void BuildMenuTreeLanguages(OdinMenuTree tree)
        {
            tree.Add("All languages", new LocEditorLangsTab("", this));

            int nbLangs = m_langs.GetNbLang();
            for(int i = 0; i < nbLangs; i++)
            {
                var lang = m_langs.GetLanguage(i);
                tree.Add(lang.languageName, new LocEditorLangsTab(lang.languageID, this));
            }

            tree.Add("Add language", new LocEditorNewLangTab(this));

        }

        void BuildMenuTreeExports(OdinMenuTree tree)
        {

        }

        void BuildMenuTreeStats(OdinMenuTree tree)
        {

        }

        void BuildMenuTreeSettings(OdinMenuTree tree)
        {

        }
    }

    public class LocEditorLangsTab
    {
        string m_langID;
        LocEditor m_editor;
        string m_filter;

        public LocEditorLangsTab(string langID, LocEditor editor)
        {
            m_langID = langID;
            m_editor = editor;
        }

        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            GUILayout.Label("Filter");
            m_filter = GUILayout.TextField(m_filter);

            if (m_langID.Length <= 0)
                DrawOneLang();
            else DrawMultiLangs();
        }

        void DrawOneLang()
        {
            var list = m_editor.locList;
            var table = list.GetTable();

            var lang = list.GetLanguage(m_langID);
            if(lang == null)
            {
                EditorGUILayout.HelpBox("No asset for lang " + m_langID, MessageType.Error);
                return;
            }

            var grayStyle = new GUIStyle(GUI.skin.button);
            grayStyle.normal.textColor = Color.gray;

            int nbText = table.Count();
            for(int i = 0; i < nbText; i++)
            {
                int id = table.GetIdAt(i);
                var textID = table.Get(id);

                if (!ProcessFilter(textID))
                    continue;

                DrawUniquePopup(textID);
                //textID
                GUILayout.BeginHorizontal();
                GUILayout.Label("ID");
                string newText = textID;
                textID = GUILayout.TextField(textID);
                if (newText != textID)
                    table.Set(id, newText);
                GUILayout.Label(id.ToString(), grayStyle);
                GUILayout.EndHorizontal();

                //dirty
                bool wasDirty = lang.GetDirty(id);
                bool newDirty = GUILayout.Toggle(wasDirty, "Dirty");
                if (wasDirty != newDirty) ;
                lang.SetDirty(id, newDirty);

                //text
                string text = lang.GetText(id);
                newText = text;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Text");
                newText = GUILayout.TextArea(text);
                if (newText != text)
                    lang.SetText(id, newText);
                GUILayout.EndHorizontal();
                
                //comment
                text = table.GetRemark(id);
                newText = text;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Comment");
                newText = GUILayout.TextArea(text);
                if (newText != text)
                    table.SetRemark(id, newText);
                GUILayout.EndHorizontal();

            }
        }

        void DrawMultiLangs()
        {
            var list = m_editor.locList;


        }

        void DrawUniquePopup(string textID)
        {
            var list = m_editor.locList;
            var table = list.GetTable();

            int nbText = table.Count();

            int nbFound = 0;
            for(int i = 0; i < nbText; i++)
            {
                int id = table.GetIdAt(i);
                if (table.Get(id) == textID)
                    nbFound++;
            }

            if(nbFound > 1)
                EditorGUILayout.HelpBox("This textID is not unique", MessageType.Error);
        }

        bool ProcessFilter(string name)
        {
            return true;
        }
    }

    public class LocEditorNewLangTab
    {
        LocEditor m_editor;
        string m_lang;
        string m_langID;
        bool m_langValid = false;
        bool m_langAlreadyExist = false;

        public LocEditorNewLangTab(LocEditor editor)
        {
            m_editor = editor;
        }

        [OnInspectorGUI]
        private void MyInspectorGUI()
        {
            GUILayout.Label("Display name");
            m_lang = GUILayout.TextField(m_lang);

            GUILayout.Space(5);
            GUILayout.Label("Language ID");
            if (!m_langValid)
                EditorGUILayout.HelpBox("This language id is not valid\nYou must follow the BCP47 standard.\nYou can found the supported names here\nhttps://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-lcid/a9eac961-e77d-41a6-90a5-ce1a8b0cdb9c", MessageType.Error);
            else if (m_langAlreadyExist)
                EditorGUILayout.HelpBox("This language already exist", MessageType.Error);
            else EditorGUILayout.HelpBox("Language " + GetDisplayLangID(m_langID), MessageType.Info);

            string newLangID = GUILayout.TextField(m_langID);
            if(newLangID != m_langID)
            {
                m_langID = newLangID;
                ValidateLangID();
            }

            if(GUILayout.Button("Create") && m_langValid)
            {
                var langs = m_editor.locList;
                langs.AddLang(m_langID, m_lang);
                m_editor.ChangeTab();
            }
        }

        void ValidateLangID()
        {
            m_langAlreadyExist = false;
            m_langValid = true;

            if(m_langID.Length <= 1)
            {
                m_langValid = false;
                return;
            }

            bool found = false;
            foreach (var ci in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
            {
                if(ci.Name == m_langID)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                m_langValid = false;
                return;
            }

            var loc = m_editor.locList;

            if (loc.GetLanguage(m_langID))
                m_langAlreadyExist = true;
        }

        string GetDisplayLangID(string id)
        {
            var info = CultureInfo.GetCultureInfo(id);
            if (info != null)
                return info.DisplayName;
            return "";
        }
    }
}


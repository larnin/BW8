using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

class QuestEditor : OdinMenuEditorWindow
{
    [MenuItem("Game/Quests")]
    public static void OpenWindow()
    {
        GetWindow<QuestEditor>().Show();
    }

    List<QuestEditorQuestTab> m_tabs = new List<QuestEditorQuestTab>();

    public void UpdateWindow()
    {
        ForceMenuTreeRebuild();
    }

    public void OpenQuest(int questIndex)
    {
        if (questIndex < 0 || questIndex >= m_tabs.Count)
            return;

        TrySelectMenuItemWithObject(m_tabs[questIndex]);
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        m_tabs.Clear();

        int nNbQuest = QuestList.GetQuestNb();

        for (int i = 0; i < nNbQuest; i++)
        {
            QuestObject quest = QuestList.GetQuestFromIndex(i);

            var tab = new QuestEditorQuestTab(this, quest.questID);
            m_tabs.Add(tab);

            tree.Add(quest.name, tab);
        }

        tree.Add("Add Quest", new QuestEditorNewQuestTab(this));

        return tree;
    }
}

class QuestEditorQuestTab
{
    QuestEditor m_editor;
    int m_ID;

    public QuestEditorQuestTab(QuestEditor editor, int ID)
    {
        m_editor = editor;
        m_ID = ID;
    }

    [OnInspectorGUI]
    private void OnInspectorGUI()
    {
        bool edited = false;

        var grayStyle = new GUIStyle(GUI.skin.button);
        grayStyle.normal.textColor = Color.gray;

        QuestObject quest = QuestList.GetQuest(m_ID);
        if(quest == null)
        {
            GUILayout.Label("Quest ID: " + m_ID);
            EditorGUILayout.HelpBox("Invalid quest id, this must not happen. Do you have renamed or removed some quest assets ?", MessageType.Error);
            return;
        }

        EditorGUI.BeginChangeCheck();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Quest: ", GUILayout.Width(50));
        quest.name = GUILayout.TextField(quest.name);
        GUILayout.Label(m_ID.ToString(), grayStyle, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        var names = QuestList.GetAllQuestsNames();
        names.Insert(0, "None");
        int index = QuestList.GetQuestIndex(quest.parentQuestID);
        index++;

        GUILayout.BeginHorizontal();
        int newIndex = EditorGUILayout.Popup("Previous Quest", index, names.ToArray());
        if(newIndex != index)
        {
            if (newIndex <= 0)
                quest.parentQuestID = QuestObject.invalidQuestID;
            else
            {
                var parentQuest = QuestList.GetQuestFromIndex(newIndex - 1);
                if (parentQuest == null)
                    quest.parentQuestID = QuestObject.invalidQuestID;
                else quest.parentQuestID = parentQuest.questID;
            }
        }
        if(newIndex > 0)
        {
            if(GUILayout.Button("Open", GUILayout.MaxWidth(100)))
            {
                m_editor.OpenQuest(index - 1);
            }
        }
        GUILayout.EndHorizontal();


        if(EditorGUI.EndChangeCheck() || edited)
        {
            EditorUtility.SetDirty(quest);
            AssetDatabase.SaveAssets();
        }
    }
}

class QuestEditorNewQuestTab
{
    QuestEditor m_editor;

    string m_name = "";

    public QuestEditorNewQuestTab(QuestEditor editor)
    {
        m_editor = editor;
    }

    [OnInspectorGUI]
    private void OnInspectorGUI()
    {
        GUILayout.Label("Quest name");
        m_name = GUILayout.TextField(m_name);

        bool validName = true;

        if (m_name.Length <= 0)
        {
            EditorGUILayout.HelpBox("Empty quest name", MessageType.Error);
            validName = false;
        }

        if (QuestList.HaveQuest(m_name))
            EditorGUILayout.HelpBox("An other quest already have this name", MessageType.Warning);

        if (GUILayout.Button("Create") && validName)
        {
            QuestList.AddQuest(m_name);
            m_editor.UpdateWindow();
            m_name = "";
        }
    }
}
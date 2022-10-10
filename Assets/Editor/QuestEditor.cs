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
    Dictionary<int, bool> m_foldoutTabs = new Dictionary<int, bool>();

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

    public bool GetFoldoutState(int questID, int objective)
    {
        int id = FoldoutID(questID, objective);

        bool state = true;

        if (!m_foldoutTabs.TryGetValue(id, out state))
            state = objective >= 0;

        return state;
    }

    public void SetFoldoutState(int questID, int objective, bool state)
    {
        int id = FoldoutID(questID, objective);

        if (!m_foldoutTabs.ContainsKey(id))
            m_foldoutTabs.Add(id, state);
        else m_foldoutTabs[id] = state;
    }

    int FoldoutID(int questID, int objective)
    {
        int hash = 27;
        hash = (13 * hash) + questID.GetHashCode();
        hash = (13 * hash) + objective.GetHashCode();
        return hash;
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

        bool state = m_editor.GetFoldoutState(m_ID, -1);
        state = EditorGUILayout.BeginFoldoutHeaderGroup(state, "Objectives", null, ShowObjectiveContextMenu);
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (state)
        {
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginVertical();
            int nbQuestObjective = quest.GetObjectiveNb();
            for(int i = 0; i < nbQuestObjective; i++)
            {
                DisplayOneObjective(i, quest.GetObjective(i));
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        m_editor.SetFoldoutState(m_ID, -1, state);

        if (EditorGUI.EndChangeCheck() || edited)
        {
            EditorUtility.SetDirty(quest);
            AssetDatabase.SaveAssets();
        }
    }

    void DisplayOneObjective(int objectiveIndex, QuestObjectiveObjectBase objective)
    {
        //https://docs.unity3d.com/ScriptReference/EditorGUILayout.BeginFoldoutHeaderGroup.html

        bool state = m_editor.GetFoldoutState(m_ID, objectiveIndex);
        state = EditorGUILayout.BeginFoldoutHeaderGroup(state, objective.GetObectiveName(), null, (Rect rect)=> { ShowOneObjectiveContextMenu(rect, objectiveIndex); });
        
        if(state)
            objective.OnInspectorGUI();

        EditorGUILayout.EndFoldoutHeaderGroup();
        m_editor.SetFoldoutState(m_ID, objectiveIndex, state);
    }

    void ShowObjectiveContextMenu(Rect position)
    {
        const string baseMenu = "New Objective";

        var allObjectiveTypes = AppDomain.CurrentDomain.GetAssemblies()
         .SelectMany(assembly => assembly.GetTypes())
         .Where(type => type.IsSubclassOf(typeof(QuestObjectiveObjectBase))).ToArray();

        var menu = new GenericMenu();
        foreach (var t in allObjectiveTypes)
        {
            const string baseString = "QuestObjectiveObject";
            string name = t.Name;
            if (name.StartsWith(baseString))
                name = name.Substring(baseString.Length);
            menu.AddItem(new GUIContent(baseMenu + "/" + name), false, () => { NewObjectiveClicked(t); });

        }
        menu.DropDown(position);
    }

    void NewObjectiveClicked(Type objectiveType)
    {
        var quest = QuestList.GetQuest(m_ID);
        if (quest == null)
            return;

        quest.AddObjectiveEditor(Activator.CreateInstance(objectiveType) as QuestObjectiveObjectBase);
    }

    void ShowOneObjectiveContextMenu(Rect position, int objective)
    {
        var quest = QuestList.GetQuest(m_ID);
        if (quest == null)
            return;

        int nbObjective = quest.GetObjectiveNb();

        var menu = new GenericMenu();
        if(objective > 0)
            menu.AddItem(new GUIContent("Move UP"), false, ()=>{ MoveObjective(objective, objective - 1); });
        if(objective < nbObjective - 1)
            menu.AddItem(new GUIContent("Move DOWN"), false, () => { MoveObjective(objective, objective + 1); });
        menu.AddItem(new GUIContent("Delete"), false, () => { RemoveObjective(objective); });
        menu.DropDown(position);
    }

    void MoveObjective(int objective, int newIndex)
    {
        var quest = QuestList.GetQuest(m_ID);
        if (quest == null)
            return;

        quest.MoveObjectiveEditor(objective, newIndex);

        bool state = m_editor.GetFoldoutState(m_ID, objective);
        bool otherState = m_editor.GetFoldoutState(m_ID, newIndex);

        m_editor.SetFoldoutState(m_ID, objective, otherState);
        m_editor.SetFoldoutState(m_ID, newIndex, state);
    }

    void RemoveObjective(int objective)
    {
        var quest = QuestList.GetQuest(m_ID);
        if (quest == null)
            return;

        quest.RemoveObjectiveEditor(objective);
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
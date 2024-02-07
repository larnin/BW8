using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BSMGraph : EditorWindow
{
    BSMGraphView m_graphView;
    BSMAttributesWindow m_attributesWindow;
    BSMErrorWindow m_errorWindow;

    Label m_nameLabel;

    string m_savePath;

    [MenuItem("Game/Behavior State Machine")]
    public static BSMGraph Open()
    {
        return GetWindow<BSMGraph>("Behavior State Machine");
    }

    public BSMAttributesWindow GetAttributesWindow()
    {
        return m_attributesWindow;
    }

    public BSMGraphView GetGraphView()
    {
        return m_graphView;
    }


    private void OnEnable()
    {
        AddGraphView();
        AddMenusWindows();

        AddStyles();

        hasUnsavedChanges = true;
    }

    private void AddGraphView()
    {
        m_graphView = new BSMGraphView(this);

        m_graphView.StretchToParentSize();

        VisualElement horizontal = BSMEditorUtility.CreateHorizontalLayout();
        horizontal.style.flexGrow = 2;

        VisualElement element = new VisualElement();
        element.style.minHeight = new StyleLength(new Length(50, LengthUnit.Percent));
        element.style.flexGrow = 2;
        element.Add(m_graphView);


        m_nameLabel = new Label();
        element.Add(m_nameLabel);
        UpdateLabel();

        horizontal.Add(element);
        AddSideMenus(horizontal);

        rootVisualElement.Add(horizontal);
    }
    private void AddStyles()
    {
        rootVisualElement.AddStyleSheets("BehaviorStateMachine/BSMVariables.uss");
    }

    void AddSideMenus(VisualElement parent)
    {
        VisualElement sideMenu = new VisualElement();
        sideMenu.style.width = 200;

        m_attributesWindow = new BSMAttributesWindow(this);
        m_attributesWindow.SetParent(sideMenu);

        parent.Add(sideMenu);
    }

    void AddMenusWindows()
    {
        VisualElement baseWindow = BSMEditorUtility.CreateHorizontalLayout();
        baseWindow.style.height = new StyleLength(new Length(90, LengthUnit.Pixel));

        VisualElement menuWindow = new VisualElement();
        menuWindow.style.width = 120;
        baseWindow.Add(menuWindow);

        menuWindow.Add(BSMEditorUtility.CreateButton("New", NewFile));
        menuWindow.Add(BSMEditorUtility.CreateButton("Load", Load));
        menuWindow.Add(BSMEditorUtility.CreateButton("Save", SaveChanges));
        menuWindow.Add(BSMEditorUtility.CreateButton("Save As", SaveAs));

        if (m_errorWindow == null)
            m_errorWindow = new BSMErrorWindow();

        VisualElement element = new VisualElement();
        m_errorWindow.SetParent(element);
        element.style.flexGrow = 2;

        baseWindow.Add(element);
        rootVisualElement.Add(baseWindow);
    }

    public void AddError(string error, string source)
    {
        if (m_errorWindow != null)
            m_errorWindow.AddError(error, source);
    }

    public void ClearErrors(string source = null)
    {
        if (m_errorWindow != null)
            m_errorWindow.ClearErrors(source);
    }

    void Save(string path)
    {
        BSMSaveData saveData = new BSMSaveData();

        m_graphView.Save(saveData);
        m_attributesWindow.Save(saveData);

        var doc = new JsonDocument();
        doc.SetRoot(saveData.Save());

        string data = Json.WriteToString(doc);

        SaveEx.SaveAsset(path, data);

        hasUnsavedChanges = true;
    }

    void Load(string path)
    {
        BSMSaveData saveData = new BSMSaveData();

        var data = SaveEx.LoadFile(path);
        if (data == null)
        {
            m_graphView.Load(saveData);
            m_attributesWindow.Load(saveData);
            return;
        }

        var doc = Json.ReadFromString(data);
        if (!doc.GetRoot().IsJsonObject())
        {
            m_graphView.Load(saveData);
            m_attributesWindow.Load(saveData);
            return;
        }
        saveData.Load(doc.GetRoot().JsonObject());

        m_graphView.Load(saveData);
        m_attributesWindow.Load(saveData);

        hasUnsavedChanges = true;
    }

    public override void SaveChanges()
    {
        base.SaveChanges();

        if (m_savePath == null || m_savePath.Length == 0)
            GetSavePath();

        if (m_savePath == null || m_savePath.Length == 0)
            return;

        Save(m_savePath);
    }

    void GetSavePath()
    {
        string savePath = SaveEx.GetSaveFilePath("Save Behavior", Application.dataPath, "json");
        if (savePath == null || savePath.Length == 0)
            return;

        m_savePath = savePath;
        UpdateLabel();
    }

    public void SaveAs()
    {
        GetSavePath();
        if (m_savePath == null || m_savePath.Length == 0)
            return;

        SaveChanges();
    }

    public void Load()
    {
        string loadPath = SaveEx.GetLoadFiltPath("Load Behavior", Application.dataPath, "json");
        if (loadPath == null || loadPath.Length == 0)
            return;

        m_savePath = loadPath;
        Load(m_savePath);
    }

    public void NewFile()
    {
        m_graphView.Load(new BSMSaveData());
        m_savePath = "";

        UpdateLabel();
    }

    void UpdateLabel()
    {
        if (m_nameLabel != null)
        {
            if (m_savePath == null)
                m_nameLabel.text = "";
            else
            {
                int index = m_savePath.IndexOf("Assets");
                if (index != -1)
                    m_nameLabel.text = m_savePath.Substring(index);
                else m_nameLabel.text = m_savePath;
            }
        }
    }
}

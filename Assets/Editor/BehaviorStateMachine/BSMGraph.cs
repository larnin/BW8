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
    BSMErrorWindow m_errorWindow;

    Label m_nameLabel;

    string m_savePath;

    [MenuItem("Game/Behavior State Machine")]
    public static BSMGraph Open()
    {
        return GetWindow<BSMGraph>("Behavior State Machine");
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
        VisualElement element = new VisualElement();
        element.style.minHeight = new StyleLength(new Length(85, LengthUnit.Percent));
        element.Add(m_graphView);

        m_nameLabel = new Label();
        element.Add(m_nameLabel);
        UpdateLabel();

        rootVisualElement.Add(element);
    }
    private void AddStyles()
    {
        rootVisualElement.AddStyleSheets("BehaviorStateMachine/BSMVariables.uss");
    }

    void AddMenusWindows()
    {
        VisualElement baseWindow = new VisualElement();
        baseWindow.style.flexDirection = FlexDirection.Row;
        baseWindow.style.justifyContent = Justify.SpaceBetween;
        baseWindow.style.height = new StyleLength(new Length(100, LengthUnit.Pixel));

        VisualElement menuWindow = new VisualElement();
        menuWindow.style.width = 120;
        baseWindow.Add(menuWindow);

        menuWindow.Add(BSMUtility.CreateButton("New", NewFile));
        menuWindow.Add(BSMUtility.CreateButton("Load", Load));
        menuWindow.Add(BSMUtility.CreateButton("Save", SaveChanges));
        menuWindow.Add(BSMUtility.CreateButton("Save As", SaveAs));

        if (m_errorWindow == null)
            m_errorWindow = new BSMErrorWindow();

        VisualElement element = new VisualElement();
        m_errorWindow.SetParent(element);

        baseWindow.Add(element);
        rootVisualElement.Add(baseWindow);
    }

    public void AddError(string error)
    {
        if (m_errorWindow != null)
            m_errorWindow.AddError(error);
    }

    public void ClearErrors()
    {
        if (m_errorWindow != null)
            m_errorWindow.ClearErrors();
    }

    void Save(string path)
    {
        var saveData = m_graphView.Save();

        var doc = new JsonDocument();
        doc.SetRoot(saveData.Save());

        string data = Json.WriteToString(doc);

        SaveEx.SaveAsset(path, data);

        hasUnsavedChanges = true;
    }

    void Load(string path)
    {
        var data = SaveEx.LoadFile(path);
        if (data == null)
        {
            m_graphView.Load(new BSMSaveData());
            return;
        }

        var doc = Json.ReadFromString(data);
        BSMSaveData saveData = new BSMSaveData();
        if (!doc.GetRoot().IsJsonObject())
        {
            m_graphView.Load(saveData);
            return;
        }
        saveData.Load(doc.GetRoot().JsonObject());

        m_graphView.Load(saveData);

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

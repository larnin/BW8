using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;

public class BSMGraph : EditorWindow
{
    BSMGraphView m_graphView;
    BSMErrorWindow m_errorWindow;

    [MenuItem("Game/Behavior State Machine")]
    public static BSMGraph Open()
    {
        return GetWindow<BSMGraph>("Behavior State Machine");
    }

    private void OnEnable()
    {
        AddGraphView();
        AddErrorWindows();

        AddStyles();
    }

    private void AddGraphView()
    {
        m_graphView = new BSMGraphView(this);

        m_graphView.StretchToParentSize();
        VisualElement element = new VisualElement();
        element.style.minHeight = new StyleLength(new Length(85, LengthUnit.Percent));
        element.Add(m_graphView);

        rootVisualElement.Add(element);
    }
    private void AddStyles()
    {
        rootVisualElement.AddStyleSheets("BehaviorStateMachine/BSMVariables.uss");
    }

    void AddErrorWindows()
    {
        if (m_errorWindow == null)
            m_errorWindow = new BSMErrorWindow();

        VisualElement element = new VisualElement();
        m_errorWindow.SetParent(element);

        element.style.maxHeight = new StyleLength(new Length(100, LengthUnit.Pixel)); 
        rootVisualElement.Add(element);
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
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;

public class BSMGraph : EditorWindow
{
    private BSMGraphView m_graphView;

    [MenuItem("Game/Behavior State Machine")]
    public static BSMGraph Open()
    {
        return GetWindow<BSMGraph>("Behavior State Machine");
    }

    private void OnEnable()
    {
        AddGraphView();

        AddStyles();
    }

    private void AddGraphView()
    {
        m_graphView = new BSMGraphView(this);

        m_graphView.StretchToParentSize();

        rootVisualElement.Add(m_graphView);
    }
    private void AddStyles()
    {
        rootVisualElement.AddStyleSheets("BehaviorStateMachine/BSMVariables.uss");
    }

    public void EnableSaving()
    {
        //todo
    }

    public void DisableSaving()
    {
        //todo
    }
}

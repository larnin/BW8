using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;

public class BehaviorStateMachineGraph : EditorWindow
{
    private BehaviorStateMachineGraphView m_graphView;

    [MenuItem("Game/Behavior State Machine")]
    public static BehaviorStateMachineGraph Open()
    {
        return GetWindow<BehaviorStateMachineGraph>("Behavior State Machine");
    }

    private void OnEnable()
    {
        AddGraphView();

        AddStyles();
    }

    private void AddGraphView()
    {
        m_graphView = new BehaviorStateMachineGraphView(this);

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

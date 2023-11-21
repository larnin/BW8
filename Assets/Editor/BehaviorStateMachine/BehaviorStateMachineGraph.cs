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


}

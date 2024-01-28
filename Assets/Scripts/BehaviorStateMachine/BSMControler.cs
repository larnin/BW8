using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMControler : MonoBehaviour
{
    [SerializeField] TextAsset m_behaviour;

    int m_startStateIndex;
    List<BSMControlerState> m_states = new List<BSMControlerState>();

    int m_currentStateIndex = -1;

    class BSMControlerTransition
    {
        public BSMConditionBase condition;
        public string nextStateID;
    }
    class BSMControlerState
    {
        public BSMStateBase state;

        public string name;
        public string ID;

        public List<BSMControlerTransition> transitions;
    }

    private void Awake()
    {
        Load();
    }

    private void OnDestroy()
    {
        foreach (var state in m_states)
            state.state.OnDestroy();
    }

    void Load()
    {
        ResetDatas();

        if (m_behaviour == null)
            return;

        string datas = SaveEx.LoadAsset(m_behaviour);

        var doc = Json.ReadFromString(datas);

        BSMSaveData saveData = new BSMSaveData();
        if (!doc.GetRoot().IsJsonObject())
            return;
        saveData.Load(doc.GetRoot().JsonObject());

        LoadStates(saveData);
        LoadStart(saveData);
        LoadTransitions(saveData);
        AfterLoad();
    }

    void LoadStates(BSMSaveData data)
    {
        foreach(var element in data.nodes)
        {
            if (element.nodeType != BSMNodeType.State)
                continue;

            BSMControlerState state = new BSMControlerState();
            state.name = element.name;
            state.ID = element.ID;
            state.state = element.data as BSMStateBase;
            if (state.state == null)
                continue;

            m_states.Add(state);
        }
    }

    void LoadStart(BSMSaveData data)
    {
        BSMSaveNode startNode = null;

        foreach(var element in data.nodes)
        {
            if (element.nodeType != BSMNodeType.Label)
                continue;

            if(element.name == "Start")
            {
                startNode = element;
                break;
            }    
        }

        if (startNode == null)
            return;

        if (startNode.outNodes.Count == 0)
            return;

        int startIndex = GetStateIndex(startNode.outNodes[0]);
    }

    void LoadTransitions(BSMSaveData data)
    {
        foreach(var state in m_states)
        {
            BSMSaveNode stateSave = GetSaveNode(data.nodes, state.ID);
            if (stateSave == null)
                continue;

            foreach(var output in stateSave.outNodes)
            {
                var transition = GetSaveNode(data.nodes, output);
                if (transform == null)
                    continue;

                if (transition.nodeType == BSMNodeType.Goto)
                {
                    transition = PropagateGoto(data.nodes, transition);
                    if (transition == null)
                        continue;
                }

                if (transition.nodeType != BSMNodeType.Condition)
                    continue;

                if (transition.outNodes.Count == 0)
                    continue;

                var nextNode = GetSaveNode(data.nodes, transition.outNodes[0]);
                
                if(nextNode.nodeType == BSMNodeType.Goto)
                {
                    nextNode = PropagateGoto(data.nodes, nextNode);
                    if (nextNode == null)
                        continue;
                }

                if (nextNode.nodeType != BSMNodeType.State)
                    continue;

                if (GetState(nextNode.ID) == null)
                    continue;

                BSMControlerTransition newTransition = new BSMControlerTransition();
                newTransition.condition = transition.data as BSMConditionBase;
                if (newTransition.condition == null)
                    continue;
                newTransition.nextStateID = nextNode.ID;

                state.transitions.Add(newTransition);
            }
        }
    }

    void AfterLoad()
    {
        foreach(var state in m_states)
        {
            state.state.SetControler(this);
            state.state.Init();
        }
    }

    BSMSaveNode GetSaveNode(List<BSMSaveNode> nodes, string ID)
    {
        foreach (var node in nodes)
        {
            if (node.ID == ID)
                return node;
        }

        return null;
    }

    BSMSaveNode PropagateGoto(List<BSMSaveNode> nodes, BSMSaveNode firstNode)
    {
        for (int i = 0; i < 10; i++)
        {
            if (firstNode.nodeType == BSMNodeType.Goto)
            {
                string target = firstNode.data as string;
                if (target == null)
                    return null;

                firstNode = GetSaveNode(nodes, target);
            }
            else if (firstNode.nodeType == BSMNodeType.Label)
            {
                if (firstNode.outNodes.Count == 0)
                    return null;

                firstNode = GetSaveNode(nodes, firstNode.outNodes[0]);
            }
            else return firstNode;
        }
        return null;
    }

    void ResetDatas()
    {
        foreach (var state in m_states)
            state.state.OnDestroy();
        m_states.Clear();

        m_currentStateIndex = -1;
        m_startStateIndex = -1;
    }

    int GetStateIndex(string ID)
    {
        for(int i = 0; i < m_states.Count; i++)
        {
            if (m_states[i].ID == ID)
                return i;
        }

        return -1;
    }

    BSMControlerState GetState(string ID)
    {
        int index = GetStateIndex(ID);
        if (index < 0)
            return null;
        return m_states[index];
    }

    private void Update()
    {
        UpdateCurrentState();

        if (m_currentStateIndex >= 0)
            m_states[m_currentStateIndex].state.Update();

        foreach (var state in m_states)
            state.state.UpdateAlways();
    }

    private void LateUpdate()
    {
        if (m_currentStateIndex >= 0)
            m_states[m_currentStateIndex].state.LateUpdate();
    }

    private void FixedUpdate()
    {
        if (m_currentStateIndex >= 0)
            m_states[m_currentStateIndex].state.FixedUpdate();
    }

    void UpdateCurrentState()
    {
        if(m_currentStateIndex < 0)
        {
            if(m_startStateIndex >= 0)
            {
                m_currentStateIndex = m_startStateIndex;
                m_states[m_currentStateIndex].state.BeginUpdate();
            }
            return;
        }

        int nextState = -1;

        foreach (var transition in m_states[m_currentStateIndex].transitions)
        {
            if (transition.condition.IsValid(m_states[m_currentStateIndex].state))
            {
                int index = GetStateIndex(transition.nextStateID);
                if (index >= 0)
                {
                    nextState = index;
                    break;
                }
            }
        }

        if(nextState >= 0)
        {
            m_states[m_currentStateIndex].state.EndUpdate();
            m_currentStateIndex = nextState;
            m_states[m_currentStateIndex].state.BeginUpdate();
        }
    }
}

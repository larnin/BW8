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
    BSMControlerAnyState m_anyState = new BSMControlerAnyState();
    List<BSMAttribute> m_attributes = new List<BSMAttribute>();

    int m_currentStateIndex = -1;

    SubscriberList m_subscriberList = new SubscriberList();

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

    class BSMControlerAnyState
    {
        public List<BSMControlerTransition> transitions;
    }

    private void Awake()
    {
        Load();

        m_subscriberList.Add(new Event<UpdateAggroTargetEvent>.LocalSubscriber(SetAggroTarget, gameObject));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();

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
        LoadAnyState(saveData);
        LoadTransitions(saveData);
        LoadAttributes(saveData);
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

    void LoadAnyState(BSMSaveData data)
    {
        BSMSaveNode anyStateNode = null;

        foreach (var element in data.nodes)
        {
            if (element.nodeType != BSMNodeType.Label)
                continue;

            if (element.name == "Any State")
            {
                anyStateNode = element;
                break;
            }
        }

        if (anyStateNode == null)
            return;

        foreach(var outNode in anyStateNode.outNodes)
        {
            var newTransition = LoadTransition(data, outNode);
            if (newTransition != null)
                m_anyState.transitions.Add(newTransition);
        }
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
                var newTransition = LoadTransition(data, output);
                if(newTransition != null)
                    state.transitions.Add(newTransition);
            }
        }
    }

    BSMControlerTransition LoadTransition(BSMSaveData data, string nodeID)
    {
        var transition = GetSaveNode(data.nodes, nodeID);
        if (transform == null)
            return null;

        if (transition.nodeType == BSMNodeType.Goto)
        {
            transition = PropagateGoto(data.nodes, transition);
            if (transition == null)
                return null;
        }

        if (transition.nodeType != BSMNodeType.Condition)
            return null;

        if (transition.outNodes.Count == 0)
            return null;

        var nextNode = GetSaveNode(data.nodes, transition.outNodes[0]);

        if (nextNode.nodeType == BSMNodeType.Goto)
        {
            nextNode = PropagateGoto(data.nodes, nextNode);
            if (nextNode == null)
                return null;
        }

        if (nextNode.nodeType != BSMNodeType.State)
            return null;

        if (GetState(nextNode.ID) == null)
            return null;

        BSMControlerTransition newTransition = new BSMControlerTransition();
        newTransition.condition = transition.data as BSMConditionBase;
        if (newTransition.condition == null)
            return null;
        newTransition.nextStateID = nextNode.ID;

        return newTransition;
    }

    void LoadAttributes(BSMSaveData data)
    {
        m_attributes = data.attributes;
    }

    void AfterLoad()
    {
        foreach(var state in m_states)
        {
            state.state.SetControler(this);
            state.state.Init();
            foreach(var transition in state.transitions)
            {
                transition.condition.SetState(state.state);
                transition.condition.Init();
            }
        }

        foreach (var transition in m_anyState.transitions)
            transition.condition.Init();
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
        {
            state.state.OnDestroy();
            foreach (var transition in state.transitions)
                transition.condition.OnDestroy();
        }
        m_states.Clear();
        m_anyState.transitions.Clear();

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
        {
            m_states[m_currentStateIndex].state.Update();
            foreach (var transition in m_states[m_currentStateIndex].transitions)
                transition.condition.Update();

            foreach (var transition in m_anyState.transitions)
                transition.condition.Update();
        }

        foreach (var state in m_states)
        {
            state.state.UpdateAlways();
            foreach (var transition in state.transitions)
                transition.condition.UpdateAlways();
        }

        foreach (var transition in m_anyState.transitions)
            transition.condition.UpdateAlways();
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
                StartUpdate(m_currentStateIndex);
            }
            return;
        }

        int nextState = -1;

        foreach (var transition in m_states[m_currentStateIndex].transitions)
        {
            if (transition.condition.IsValid())
            {
                int index = GetStateIndex(transition.nextStateID);
                if (index >= 0)
                {
                    nextState = index;
                    break;
                }
            }
        }

        if(nextState < 0)
        {
            foreach(var transition in m_anyState.transitions)
            {
                if(transition.condition.IsValid())
                {
                    int index = GetStateIndex(transition.nextStateID);
                    if (index >= 0)
                    {
                        nextState = index;
                        break;
                    }
                }
            }
        }

        if(nextState >= 0)
        {
            EndUpdate(m_startStateIndex);
            m_currentStateIndex = nextState;
            StartUpdate(m_startStateIndex);
        }
    }

    void StartUpdate(int stateIndex)
    {
        m_states[stateIndex].state.BeginUpdate();
        foreach (var transition in m_states[stateIndex].transitions)
            transition.condition.BeginUpdate();

        foreach(var transition in m_anyState.transitions)
        {
            transition.condition.SetState(m_states[stateIndex].state);
            transition.condition.BeginUpdate();
        }
    }

    void EndUpdate(int stateIndex)
    {
        m_states[stateIndex].state.EndUpdate();
        foreach (var transition in m_states[stateIndex].transitions)
            transition.condition.EndUpdate();

        foreach (var transition in m_anyState.transitions)
            transition.condition.EndUpdate();
    }

    public BSMAttribute GetAttribute(string ID)
    {
        foreach(var attribute in m_attributes)
        {
            if (attribute.ID == ID)
                return attribute;
        }

        return null;
    }

    public BSMAttribute GetAttributeFromName(string name)
    {
        foreach (var attribute in m_attributes)
        {
            if (attribute.name == name)
                return attribute;
        }

        return null;
    }

    public void SetIntAttribute(string name, int value)
    {
        var attribute = GetAttributeFromName(name);
        if (attribute == null)
            return;

        attribute.data.SetInt(value);
    }

    public int GetIntAttribute(string name, int defaultValue = 0)
    {
        var attribute = GetAttributeFromName(name);
        if (attribute == null)
            return defaultValue;

        return attribute.data.GetInt(defaultValue);
    }

    public void SetFloatAttribute(string name, float value)
    {
        var attribute = GetAttributeFromName(name);
        if (attribute == null)
            return;

        attribute.data.SetFloat(value);
    }

    public float GetFloatAttribute(string name, float defaultValue = 0)
    {
        var attribute = GetAttributeFromName(name);
        if (attribute == null)
            return defaultValue;

        return attribute.data.GetFloat(defaultValue);
    }

    public void SetStringAttribute(string name, string value)
    {
        var attribute = GetAttributeFromName(name);
        if (attribute == null)
            return;

        attribute.data.SetString(value);
    }

    public string GetStringAttribute(string name, string defaultValue = "")
    {
        var attribute = GetAttributeFromName(name);
        if (attribute == null)
            return defaultValue;

        return attribute.data.GetString(defaultValue);
    }

    public void SetGameObjectAttribute(string name, GameObject value)
    {
        var attribute = GetAttributeFromName(name);
        if (attribute == null)
            return;

        attribute.data.SetGameObject(value);
    }

    public GameObject GetGameObjectAttribute(string name, GameObject defaultValue = null)
    {
        var attribute = GetAttributeFromName(name);
        if (attribute == null)
            return defaultValue;

        return attribute.data.GetGameObject(defaultValue);
    }

    void SetAggroTarget(UpdateAggroTargetEvent e)
    {
        SetGameObjectAttribute("Aggro", e.target);
    }
}

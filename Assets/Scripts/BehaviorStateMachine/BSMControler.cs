using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMControler : SerializedMonoBehaviour
{
    [SerializeField] BSMScriptableObject m_behaviour;
    [SerializeField] List<BSMControlerAttribute> m_controllerAttributes = new List<BSMControlerAttribute>();

    int m_startStateIndex;
    List<BSMControlerState> m_states = new List<BSMControlerState>();
    BSMControlerAnyState m_anyState = new BSMControlerAnyState();
    List<BSMAttribute> m_attributes = new List<BSMAttribute>();

    int m_currentStateIndex = -1;

    SubscriberList m_subscriberList = new SubscriberList();

    public BSMScriptableObject behaviour { get { return m_behaviour; } set { m_behaviour = value; } }

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

        public List<BSMControlerTransition> transitions = new List<BSMControlerTransition>();
    }

    class BSMControlerAnyState
    {
        public List<BSMControlerTransition> transitions = new List<BSMControlerTransition>();
    }

    class BSMControlerAttribute
    {
        public string ID; 
        public object value;
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

    public void LoadFromEditor()
    {
        Load(false);
    }

    void Load(bool linkAttributes = true)
    {
        ResetDatas();

        if (m_behaviour == null)
            return;

        BSMSaveData saveData = m_behaviour.data;

        LoadStates(saveData);
        LoadStart(saveData);
        LoadAnyState(saveData);
        LoadTransitions(saveData);
        LoadAttributes(saveData, linkAttributes);
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

        m_startStateIndex = GetStateIndex(startNode.outNodes[0]);
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

    void LoadAttributes(BSMSaveData data, bool linkAttributes)
    {
        m_attributes = data.attributes;

        if(linkAttributes)
        {
            foreach(var a in m_controllerAttributes)
            {
                var attribute = GetAttribute(a.ID);
                if(attribute == null || attribute.automatic)
                    continue;

                attribute.data.data = a.value;
            }
        }
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

        m_attributes.Clear();
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

    public bool HaveControlerAttribute(string ID)
    {
        foreach (var a in m_controllerAttributes)
        {
            if (a.ID == ID)
                return true;
        }
        return false;
    }

    public object GetControlerAttribute(string ID)
    {
        foreach (var a in m_controllerAttributes)
        {
            if (a.ID == ID)
                return a.value;
        }
        return null;
    }

    public void SetControlerAttribute(string ID, object value)
    {
        bool found = false;
        foreach(var a in m_controllerAttributes)
        {
            if(a.ID == ID)
            {
                a.value = value;
                found = true;
                break;
            }
        }

        if(!found)
        {
            var attribute = new BSMControlerAttribute();
            attribute.ID = ID;
            attribute.value = value;
            m_controllerAttributes.Add(attribute);
        }
    }

    public int GetAttributeNB()
    {
        return m_attributes.Count;
    }

    public BSMAttribute GetAttributeFromIndex(int index)
    {
        if (index < 0 || index >= m_attributes.Count)
            return null;
        return m_attributes[index];
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

    void SetStructAttribute<T>(string name, T value, BSMAttributeType attributeType) where T : struct
    {
        var attribute = GetAttributeFromName(name);
        if (attribute == null)
            return;

        attribute.data.SetStruct(value, attributeType);
    }

    T GetStructAttribute<T>(string name, T defaultValue, BSMAttributeType attributeType) where T : struct
    {
        var attribute = GetAttributeFromName(name);
        if (attribute == null)
            return defaultValue;

        return attribute.data.GetStruct(defaultValue, attributeType);
    }

    void SetClassAttribute<T>(string name, T value, BSMAttributeType attributeType) where T : class
    {
        var attribute = GetAttributeFromName(name);
        if (attribute == null)
            return;

        attribute.data.SetClass(value, attributeType);
    }

    T GetClassAttribute<T>(string name, T defaultValue, BSMAttributeType attributeType) where T : class
    {
        var attribute = GetAttributeFromName(name);
        if (attribute == null)
            return defaultValue;

        return attribute.data.GetClass(defaultValue, attributeType);
    }

    public void SetIntAttribute(string name, int value)
    {
        SetStructAttribute(name, value, BSMAttributeType.attributeInt);
    }

    public int GetIntAttribute(string name, int defaultValue = 0)
    {
        return GetStructAttribute(name, defaultValue, BSMAttributeType.attributeInt);
    }

    public void SetFloatAttribute(string name, float value)
    {
        SetStructAttribute(name, value, BSMAttributeType.attributeFloat);
    }

    public float GetFloatAttribute(string name, float defaultValue = 0)
    {
        return GetStructAttribute(name, defaultValue, BSMAttributeType.attributeFloat);
    }

    public void SetStringAttribute(string name, string value)
    {
        SetClassAttribute(name, value, BSMAttributeType.attributeString);
    }

    public string GetStringAttribute(string name, string defaultValue = "")
    {
        return GetClassAttribute(name, defaultValue, BSMAttributeType.attributeString);
    }

    public void SetGameObjectAttribute(string name, GameObject value)
    {
        SetClassAttribute(name, value, BSMAttributeType.attributeGameObject);
    }

    public GameObject GetGameObjectAttribute(string name, GameObject defaultValue = null)
    {
        return GetClassAttribute(name, defaultValue, BSMAttributeType.attributeGameObject);
    }

    public void SetVector2Attribute(string name, Vector2 value)
    {
        SetStructAttribute(name, value, BSMAttributeType.attributeVector2);
    }

    public Vector2 GetVector2Attribute(string name) { return GetVector2Attribute(name, Vector2.zero); }
    public Vector2 GetVector2Attribute(string name, Vector2 defaultValue)
    {
        return GetStructAttribute(name, defaultValue, BSMAttributeType.attributeVector2);
    }

    public void SetVector3Attribute(string name, Vector3 value)
    {
        SetStructAttribute(name, value, BSMAttributeType.attributeVector3);
    }

    public Vector3 GetVector3Attribute(string name) { return GetVector3Attribute(name, Vector3.zero); }
    public Vector3 GetVector3Attribute(string name, Vector3 defaultValue)
    {
        return GetStructAttribute(name, defaultValue, BSMAttributeType.attributeVector3);
    }

    public void SetVector2IntAttribute(string name, Vector2Int value)
    {
        SetStructAttribute(name, value, BSMAttributeType.attributeVector2Int);
    }

    public Vector2Int GetVector2IntAttribute(string name) { return GetVector2IntAttribute(name, Vector2Int.zero); }
    public Vector2Int GetVector2IntAttribute(string name, Vector2Int defaultValue)
    {
        return GetStructAttribute(name, defaultValue, BSMAttributeType.attributeVector2Int);
    }

    public void SetVector3IntAttribute(string name, Vector3Int value)
    {
        SetStructAttribute(name, value, BSMAttributeType.attributeVector3Int);
    }

    public Vector3Int GetVector3IntAttribute(string name) { return GetVector3IntAttribute(name, Vector3Int.zero); }
    public Vector3Int GetVector3IntAttribute(string name, Vector3Int defaultValue)
    {
        return GetStructAttribute(name, defaultValue, BSMAttributeType.attributeVector3Int);
    }

    void SetAggroTarget(UpdateAggroTargetEvent e)
    {
        SetGameObjectAttribute("Aggro", e.target);
    }
}

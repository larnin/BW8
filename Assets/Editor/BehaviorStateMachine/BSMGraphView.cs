﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class BSMGraphView : GraphView
{
    BSMGraph m_editorWindow;

    List<BSMNode> m_nodes = new List<BSMNode>();
    BSMNodeLabel m_startNode;
    BSMNodeLabel m_anyStateNode;

    public BSMGraphView(BSMGraph editorWindow)
    {
        m_editorWindow = editorWindow;

        AddManipulators();
        AddGridBackground();
        AddStyles();

        OnElementsDeleted();
        OnGraphViewChanged();

        AddStartNode();
        AddAnyStateNode();
    }

    private void AddGridBackground()
    {
        GridBackground gridBackground = new GridBackground();

        gridBackground.StretchToParentSize();

        Insert(0, gridBackground);
    }

    private void AddStyles()
    {
        this.AddStyleSheets( 
            "BehaviorStateMachine/BSMGraphViewStyles.uss",
            "BehaviorStateMachine/BSMNodeStyles.uss"
        );
    }

    void OnElementsDeleted()
    {
        //todo
        deleteSelection = (operationName, askUser) =>
        {
            Type edgeType = typeof(Edge);
            
            List<BSMNode> nodesToDelete = new List<BSMNode>();
            List<Edge> edgesToDelete = new List<Edge>();

            foreach (GraphElement selectedElement in selection)
            {
                if (selectedElement is BSMNode node)
                {
                    if (node == m_startNode)
                        continue;

                    if (node == m_anyStateNode)
                        continue;

                    nodesToDelete.Add(node);

                    continue;
                }

                if (selectedElement.GetType() == edgeType)
                {
                    Edge edge = (Edge)selectedElement;

                    edgesToDelete.Add(edge);

                    continue;
                }
            }

            DeleteElements(edgesToDelete);

            foreach (BSMNode nodeToDelete in nodesToDelete)
            {
                RemoveNode(nodeToDelete, false);

                nodeToDelete.DisconnectAllPorts();

                RemoveElement(nodeToDelete);
            }

            ProcessErrors();
        };
    }

    void OnGraphViewChanged()
    {
        //todo
        graphViewChanged = (changes) =>
        {
            ProcessErrors(); //does not works for edges, need to delay this call

            return changes;
        };
    }

    public void AddNode(BSMNode node, bool checkError = true)
    {
        m_nodes.Add(node);
        if(checkError)
            ProcessErrors();
    }

    public void RemoveNode(BSMNode node, bool checkError = true)
    {
        m_nodes.Remove(node);
        if(checkError)
            ProcessErrors();
    }

    private void AddStartNode()
    {
        var node = CreateNode("Start", BSMNodeType.Label, new Vector2(10, 10), false, false);
        m_startNode = node as BSMNodeLabel;
        m_startNode.nodeType = BSMNodeLabelType.Start;
        node.Draw();
        AddNode(m_startNode);
        AddElement(node);
    }

    private void AddAnyStateNode()
    {
        var node = CreateNode("Any State", BSMNodeType.Label, new Vector2(10, 100), false, false);
        m_anyStateNode = node as BSMNodeLabel;
        m_anyStateNode.nodeType = BSMNodeLabelType.AnyState;
        node.Draw();
        AddNode(m_anyStateNode);
        AddElement(node);
    }

    private void AddManipulators()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        this.AddManipulator(CreateNodeContextualMenu("Add Label", BSMNodeType.Label));
        this.AddManipulator(CreateNodeContextualMenu("Add Goto", BSMNodeType.Goto));
        this.AddManipulator(CreateNodeContextualMenu("Add State", BSMNodeType.State));
        this.AddManipulator(CreateNodeContextualMenu("Add Condition", BSMNodeType.Condition)); 
    }

    private IManipulator CreateNodeContextualMenu(string actionTitle, BSMNodeType dialogueType)
    {
        string name = "New Node";
        if (dialogueType == BSMNodeType.State)
            name = "New State";
        else if (dialogueType == BSMNodeType.Condition)
            name = "New Condition";
        else if (dialogueType == BSMNodeType.Label)
            name = "New Label";
        else if (dialogueType == BSMNodeType.Goto)
            name = "New Goto";
        else return null;

        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(name, dialogueType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
        );

        return contextualMenuManipulator;
    }

    public BSMNode CreateNode(string nodeName, BSMNodeType dialogueType, Vector2 position, bool shouldDraw = true, bool addList = true)
    {
        Type nodeType = null;

        if (dialogueType == BSMNodeType.State)
            nodeType = typeof(BSMNodeState);
        else if (dialogueType == BSMNodeType.Condition)
            nodeType = typeof(BSMNodeCondition);
        else if (dialogueType == BSMNodeType.Label)
            nodeType = typeof(BSMNodeLabel);
        else if (dialogueType == BSMNodeType.Goto)
            nodeType = typeof(BSMNodeGoto);

        if (nodeType == null)
            return null;

        BSMNode node = (BSMNode)Activator.CreateInstance(nodeType);

        node.Initialize(nodeName, this, position);

        if (shouldDraw)
        {
            node.Draw();
        }

        if (addList)
            AddNode(node);

        return node;
    }

    public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
    {
        Vector2 worldMousePosition = mousePosition;

        if (isSearchWindow)
        {
            worldMousePosition = m_editorWindow.rootVisualElement.ChangeCoordinatesTo(m_editorWindow.rootVisualElement.parent, mousePosition - m_editorWindow.position.position);
        }

        Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

        return localMousePosition;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new List<Port>();

        ports.ForEach(port =>
        {
            if (startPort.node is BSMNodeLabel)
            {
                var nodeLabel = startPort.node as BSMNodeLabel;

                if(nodeLabel.nodeType == BSMNodeLabelType.AnyState&& !(port.node is BSMNodeCondition))
                    return;

                if (nodeLabel.nodeType != BSMNodeLabelType.AnyState && !(port.node is BSMNodeState))
                    return;
            }

            if (startPort.node is BSMNodeGoto && port.node is BSMNodeLabel)
                return;

            if (startPort.node is BSMNodeState && port.node is BSMNodeState)
                return;

            if (startPort.node is BSMNodeCondition && port.node is BSMNodeCondition)
                return;

            if (startPort == port)
                return;

            if (startPort.node == port.node)
                return;

            if (startPort.direction == port.direction)
                return;

            compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    public void ProcessErrors()
    {
        ClearErrors();

        foreach (var node in m_nodes)
            node.UpdateStyle(false);

        for(int i = 0; i < m_nodes.Count; i++)
        {
            int namesError = 1;

            var node = m_nodes[i];

            for(int j = i + 1; j< m_nodes.Count; j++)
            {
                if(m_nodes[j].NodeName == node.NodeName)
                {
                    if (namesError == 1)
                        node.UpdateStyle(true);
                    m_nodes[j].UpdateStyle(true);

                    namesError++;
                }
            }

            foreach(var edge in BSMEditorUtility.GetAllOutEdge(node))
            {
                SetEdgeStyle(edge, false);

                if (edge.input == null)
                    continue;
                if (edge.input.node == null)
                    continue;

                var nextNode = edge.input.node as BSMNode;
                if (node == null)
                    continue;

                var inType = BSMEditorUtility.GetType(node);
                var outType = BSMEditorUtility.GetType(nextNode);

                if (inType == BSMNodeType.Label && (node != m_startNode || node != m_anyStateNode))
                    continue;

                if(outType == BSMNodeType.Goto)
                {
                    var realNextNode = PropagateGoto(nextNode);
                    if (realNextNode == null)
                        continue;
                    outType = BSMEditorUtility.GetType(realNextNode);

                    if (outType == BSMNodeType.Label || outType == BSMNodeType.Goto)
                        continue;
                }

                if (node == m_startNode && outType != BSMNodeType.State)
                {
                    AddError("Start node must be connected to a state node");
                    SetEdgeStyle(edge, true);
                }
                else if(node == m_anyStateNode && outType != BSMNodeType.Condition)
                {
                    AddError("Any State node must be connected to a condition node");
                    SetEdgeStyle(edge, true);
                }
                else if (inType == outType)
                {
                    AddError("You can't connect 2 nodes of the same type together - Nodes " + node.NodeName + " & " + nextNode.NodeName);
                    SetEdgeStyle(edge, true);
                }
            }

            if (namesError > 1)
                AddError(namesError + " Nodes are named " + node.NodeName + ", names must be unique");
        }

        foreach (var node in GetALLGotoNodes())
            node.UpdateTarget();
    }

    BSMNode PropagateGoto(BSMNode node)
    {
        if (!(node is BSMNodeGoto))
            return null;

        var gotoNode = node as BSMNodeGoto;

        string lblID = gotoNode.GetLabelID();
        if (lblID == null)
            return null;

        BSMNode nextNode = null;

        var nodes = GetAllLabelNodes();
        foreach(var n in nodes)
        {
            if (n.ID == lblID)
                nextNode = n;
        }

        if (nextNode == null)
            return null;

        var edges = BSMEditorUtility.GetAllOutEdge(nextNode);
        foreach(var edge in edges)
        {
            if (edge.input == null)
                continue;
            if (edge.input.node == null)
                continue;

            return edge.input.node as BSMNode;
        }
        return null;
    }

    void SetEdgeStyle(Edge edge, bool error)
    {
        Color borderColor = new Color(.8f, .8f, .8f);

        if (error)
        {
            edge.style.borderLeftColor = BSMNode.errorBorderColor;
            edge.style.borderBottomColor = BSMNode.errorBorderColor;
            edge.style.borderRightColor = BSMNode.errorBorderColor;
            edge.style.borderTopColor = BSMNode.errorBorderColor;
        }
        else
        {
            edge.style.borderLeftColor = borderColor;
            edge.style.borderBottomColor = borderColor;
            edge.style.borderRightColor = borderColor;
            edge.style.borderTopColor = borderColor;
        }
    }

    public List<BSMNodeLabel> GetAllLabelNodes()
    {
        List<BSMNodeLabel> labels = new List<BSMNodeLabel>();
        foreach(var node in m_nodes)
        {
            if (node is BSMNodeLabel)
                labels.Add(node as BSMNodeLabel);
        }

        return labels;
    }

    public List<BSMNodeGoto> GetALLGotoNodes()
    {
        List<BSMNodeGoto> gotos = new List<BSMNodeGoto>();
        foreach (var node in m_nodes)
        {
            if (node is BSMNodeGoto)
                gotos.Add(node as BSMNodeGoto);
        }

        return gotos;
    }

    void AddError(string error)
    {
        if (m_editorWindow != null)
            m_editorWindow.AddError(error);
    }

    void ClearErrors()
    {
        if (m_editorWindow != null)
            m_editorWindow.ClearErrors();
    }

    void Clean()
    {
        foreach(var node in m_nodes)
            RemoveElement(node);

        var elements = edges.ToList();
        foreach(var element in elements)
        {
            if(element is Edge)
                RemoveElement(element as Edge);
        }

        m_nodes.Clear();
        m_startNode = null;
        m_anyStateNode = null;
    }

    public void Load(BSMSaveData data)
    {
        Clean();

        List<BSMNode> nodes = new List<BSMNode>();

        foreach(var dataNode in data.nodes)
        {
            var node = LoadNode(dataNode);
            nodes.Add(node);
        }

        foreach (var node in nodes)
        {
            node.Draw();
            AddElement(node);
            AddNode(node, false);
        }

        if(m_startNode == null)
            AddStartNode();

        if (m_anyStateNode == null)
            AddAnyStateNode();

        CreateConnexions(nodes, data.nodes);

        ProcessErrors();
    }

    BSMNode LoadNode(BSMSaveNode data)
    {
        BSMNode node = null;

        if (data.nodeType == BSMNodeType.Label)
        {
            var nodeLabel = new BSMNodeLabel();
            node = nodeLabel;
            if (data.name == "Start")
            {
                m_startNode = nodeLabel;
                m_startNode.nodeType = BSMNodeLabelType.Start;
            }
            else if(data.name == "Any State")
            {
                m_anyStateNode = nodeLabel;
                m_anyStateNode.nodeType = BSMNodeLabelType.AnyState;
            }
        }
        else if (data.nodeType == BSMNodeType.Goto)
        {
            var nodeGoto = new BSMNodeGoto();
            nodeGoto.SetLabelID(data.data as string);
            node = nodeGoto;
        }
        else if (data.nodeType == BSMNodeType.Condition)
        {
            var nodeConditon = new BSMNodeCondition();
            nodeConditon.SetCondition(data.data as BSMConditionBase);
            node = nodeConditon;
        }
        else if (data.nodeType == BSMNodeType.State)
        {
            var nodeState = new BSMNodeState();
            nodeState.SetState(data.data as BSMStateBase);
            node = nodeState;
        }
        
        node.ID = data.ID;
        node.NodeName = data.name;
        node.SetPosition(data.position);
        node.Initialize(data.name, this, data.position.position, false);

        return node;
    }

    void CreateConnexions(List<BSMNode> nodes, List<BSMSaveNode> datas)
    {
        List<Edge> edges = new List<Edge>();

        foreach(var data in datas)
        {
            var node = GetFromID(nodes, data.ID);
            if (node == null)
                continue;
            foreach (var connexion in data.outNodes)
            {
                var outNode = GetFromID(nodes, connexion);
                if (outNode == null)
                    continue;

                var edge = BSMEditorUtility.ConnectNodes(node, outNode);
                if (edge != null)
                    edges.Add(edge);
            }
        }

        foreach (var edge in edges)
            AddElement(edge);
    }

    static BSMNode GetFromID(List<BSMNode> nodes, string id)
    {
        foreach(var node in nodes)
        {
            if (node.ID == id)
                return node;
        }

        return null;
    }

    public void  Save(BSMSaveData data)
    {
        foreach (var node in m_nodes)
                data.nodes.Add(SaveNode(node));
    }

    BSMSaveNode SaveNode(BSMNode node)
    {
        BSMSaveNode data = new BSMSaveNode();

        data.ID = node.ID;
        data.name = node.NodeName;
        data.position = node.GetPosition();

        foreach(Port port in node.outputContainer.Children())
        {
            if (port == null)
                continue;

            foreach(var connexion in port.connections)
            {
                if (connexion.input == null)
                    continue;
                if (connexion.input.node == null)
                    continue;

                var nextNode = connexion.input.node as BSMNode;
                if (nextNode == null)
                    continue;
                data.outNodes.Add(nextNode.ID);
            }
        }

        data.nodeType = BSMEditorUtility.GetType(node);

        if (data.nodeType == BSMNodeType.Condition)
        {
            var nodeCondition = node as BSMNodeCondition;
            if(nodeCondition != null)
                data.data = nodeCondition.GetCondition();
        }
        else if (data.nodeType == BSMNodeType.State)
        {
            var nodeState = node as BSMNodeState;
            if(nodeState != null)
                data.data = nodeState.GetState();
        }
        else if(data.nodeType == BSMNodeType.Goto)
        {
            var nodeGoto = node as BSMNodeGoto;
            if (nodeGoto != null)
                data.data = nodeGoto.GetLabelID();
        }

        return data;
    }
}
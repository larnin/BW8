using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

class BSMNodeError
{
    public Color Color { get; private set; }
    public List<BSMNode> Nodes = new List<BSMNode>();

    public BSMNodeError()
    {
        Color = new Color32(
               (byte)UnityEngine.Random.Range(65, 256),
               (byte)UnityEngine.Random.Range(50, 176),
               (byte)UnityEngine.Random.Range(50, 176),
               255
           );
    }
}

public class BSMGraphView : GraphView
{
    BSMGraph m_editorWindow; 

    SerializableDictionary<string, BSMNodeError> m_ungroupedNodes = new SerializableDictionary<string, BSMNodeError>();
    BSMStartNode m_startNode;

    private int nameErrorsAmount = 0;

    public int NameErrorsAmount
    {
        get
        {
            return nameErrorsAmount;
        }

        set
        {
            nameErrorsAmount = value;

            if (nameErrorsAmount == 0)
            {
                m_editorWindow.EnableSaving();
            }

            if (nameErrorsAmount == 1)
            {
                m_editorWindow.DisableSaving();
            }
        }
    }
    public BSMGraphView(BSMGraph editorWindow)
    {
        m_editorWindow = editorWindow;

        AddManipulators();
        AddGridBackground();
        AddStyles();

        OnElementsDeleted();
        OnGraphViewChanged();

        AddStartNode();
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
                    if (node is BSMStartNode)
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
                RemoveUngroupedNode(nodeToDelete);

                nodeToDelete.DisconnectAllPorts();

                RemoveElement(nodeToDelete);
            }
        };
    }

    void OnGraphViewChanged()
    {
        //todo
        graphViewChanged = (changes) =>
        {
            if (changes.edgesToCreate != null)
            {
                foreach (Edge edge in changes.edgesToCreate)
                {
                    BSMNode nextNode = (BSMNode)edge.input.node;

                    //DSChoiceSaveData choiceData = (DSChoiceSaveData)edge.output.userData;

                    //choiceData.NodeID = nextNode.ID;
                }
            }

            if (changes.elementsToRemove != null)
            {
                Type edgeType = typeof(Edge);

                foreach (GraphElement element in changes.elementsToRemove)
                {
                    if (element.GetType() != edgeType)
                    {
                        continue;
                    }

                    Edge edge = (Edge)element;

                    //DSChoiceSaveData choiceData = (DSChoiceSaveData)edge.output.userData;

                    //choiceData.NodeID = "";
                }
            }

            return changes;
        };
    }

    public void AddUngroupedNode(BSMNode node)
    {
        string nodeName = node.NodeName.ToLower();

        if (!m_ungroupedNodes.ContainsKey(nodeName))
        {
            BSMNodeError nodeError = new BSMNodeError();
            nodeError.Nodes.Add(node);
            m_ungroupedNodes.Add(nodeName, nodeError);

            return;
        }

        Color errorColor = m_ungroupedNodes[nodeName].Color;
        List<BSMNode> list = m_ungroupedNodes[nodeName].Nodes;
        list.Add(node);

        node.SetErrorStyle(errorColor);
        if (list.Count == 2)
        {
            list[0].SetErrorStyle(errorColor);
            NameErrorsAmount++;
        }
    }

    public void RemoveUngroupedNode(BSMNode node)
    {
        string nodeName = node.NodeName.ToLower();

        List<BSMNode> list = m_ungroupedNodes[nodeName].Nodes;

        list.Remove(node);
        
        if (list.Count == 1)
        {
            NameErrorsAmount--;
            list[0].ResetStyle();

            return;
        }

        if (list.Count == 0)
            m_ungroupedNodes.Remove(nodeName);
    }

    private void AddStartNode()
    {
        var node = CreateNode("Start", BSMNodeType.Start, Vector2.zero, true, false);
        m_startNode = node as BSMStartNode;
        AddElement(node);
    }

    private void AddManipulators()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

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
        else if (dialogueType == BSMNodeType.Start)
            return null;

        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(name, dialogueType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
        );

        return contextualMenuManipulator;
    }

    public BSMNode CreateNode(string nodeName, BSMNodeType dialogueType, Vector2 position, bool shouldDraw = true, bool addList = true)
    {
        Type nodeType = null;

        if (dialogueType == BSMNodeType.State)
            nodeType = typeof(BSMStateNode);
        else if (dialogueType == BSMNodeType.Condition)
            nodeType = typeof(BSMConditionNode);
        else if (dialogueType == BSMNodeType.Start)
            nodeType = typeof(BSMStartNode);

        if (nodeType == null)
            return null;

        BSMNode node = (BSMNode)Activator.CreateInstance(nodeType);

        node.Initialize(nodeName, this, position);

        if (shouldDraw)
        {
            node.Draw();
        }

        if(addList)
            AddUngroupedNode(node);

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
            if (startPort.node is BSMStartNode && !(port.node is BSMStateNode))
                return;

            if (startPort.node is BSMStateNode && port.node is BSMStateNode)
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
}
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

public class BehaviorStateMachineGraphView : GraphView
{
    BehaviorStateMachineGraph m_editorWindow;

    SerializableDictionary<string, BSMNodeError> ungroupedNodes = new SerializableDictionary<string, BSMNodeError>();

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
    public BehaviorStateMachineGraphView(BehaviorStateMachineGraph editorWindow)
    {
        m_editorWindow = editorWindow;

        AddManipulators();
        AddGridBackground();
        AddStyles();

        OnElementsDeleted();
        OnGraphViewChanged();
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
            //if (changes.edgesToCreate != null)
            //{
            //    foreach (Edge edge in changes.edgesToCreate)
            //    {
            //        DSNode nextNode = (DSNode)edge.input.node;

            //        DSChoiceSaveData choiceData = (DSChoiceSaveData)edge.output.userData;

            //        choiceData.NodeID = nextNode.ID;
            //    }
            //}

            //if (changes.elementsToRemove != null)
            //{
            //    Type edgeType = typeof(Edge);

            //    foreach (GraphElement element in changes.elementsToRemove)
            //    {
            //        if (element.GetType() != edgeType)
            //        {
            //            continue;
            //        }

            //        Edge edge = (Edge)element;

            //        DSChoiceSaveData choiceData = (DSChoiceSaveData)edge.output.userData;

            //        choiceData.NodeID = "";
            //    }
            //}

            return changes;
        };
    }

    public void AddUngroupedNode(BSMNode node)
    {
        string nodeName = node.NodeName.ToLower();

        if (!ungroupedNodes.ContainsKey(nodeName))
        {
            BSMNodeError nodeError = new BSMNodeError();
            nodeError.Nodes.Add(node);
            ungroupedNodes.Add(nodeName, nodeError);

            return;
        }

        Color errorColor = ungroupedNodes[nodeName].Color;
        List<BSMNode> list = ungroupedNodes[nodeName].Nodes;
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

        List<BSMNode> list = ungroupedNodes[nodeName].Nodes;

        list.Remove(node);
        
        if (list.Count == 1)
        {
            NameErrorsAmount--;
            list[0].ResetStyle();

            return;
        }

        if (list.Count == 0)
            ungroupedNodes.Remove(nodeName);
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

        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(name, dialogueType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
        );

        return contextualMenuManipulator;
    }

    public BSMNode CreateNode(string nodeName, BSMNodeType dialogueType, Vector2 position, bool shouldDraw = true)
    {
        Type nodeType = null;

        if (dialogueType == BSMNodeType.State)
            nodeType = typeof(BSMStateNode);
        else if (dialogueType == BSMNodeType.Condition)
            nodeType = typeof(BSMConditionNode);

        if (nodeType == null)
            return null;

        BSMNode node = (BSMNode)Activator.CreateInstance(nodeType);

        node.Initialize(nodeName, this, position);

        if (shouldDraw)
        {
            node.Draw();
        }

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
}
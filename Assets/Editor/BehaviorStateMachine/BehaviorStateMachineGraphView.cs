using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class BehaviorStateMachineGraphView : GraphView
{
    BehaviorStateMachineGraph m_editorWindow;
    public BehaviorStateMachineGraphView(BehaviorStateMachineGraph editorWindow)
    {
        m_editorWindow = editorWindow;

        AddGridBackground();
        AddStyles();

        OnElementsDeleted();
        OnGroupElementsAdded();
        OnGroupElementsRemoved();
        OnGroupRenamed();
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
            "DialogueSystem/DSGraphViewStyles.uss",
            "DialogueSystem/DSNodeStyles.uss"
        );
    }

    void OnElementsDeleted()
    {
        //todo
        deleteSelection = (operationName, askUser) =>
        {
            //Type groupType = typeof(DSGroup);
            //Type edgeType = typeof(Edge);

            //List<DSGroup> groupsToDelete = new List<DSGroup>();
            //List<DSNode> nodesToDelete = new List<DSNode>();
            //List<Edge> edgesToDelete = new List<Edge>();

            //foreach (GraphElement selectedElement in selection)
            //{
            //    if (selectedElement is DSNode node)
            //    {
            //        nodesToDelete.Add(node);

            //        continue;
            //    }

            //    if (selectedElement.GetType() == edgeType)
            //    {
            //        Edge edge = (Edge)selectedElement;

            //        edgesToDelete.Add(edge);

            //        continue;
            //    }

            //    if (selectedElement.GetType() != groupType)
            //    {
            //        continue;
            //    }

            //    DSGroup group = (DSGroup)selectedElement;

            //    groupsToDelete.Add(group);
            //}

            //foreach (DSGroup groupToDelete in groupsToDelete)
            //{
            //    List<DSNode> groupNodes = new List<DSNode>();

            //    foreach (GraphElement groupElement in groupToDelete.containedElements)
            //    {
            //        if (!(groupElement is DSNode))
            //        {
            //            continue;
            //        }

            //        DSNode groupNode = (DSNode)groupElement;

            //        groupNodes.Add(groupNode);
            //    }

            //    groupToDelete.RemoveElements(groupNodes);

            //    RemoveGroup(groupToDelete);

            //    RemoveElement(groupToDelete);
            //}

            //DeleteElements(edgesToDelete);

            //foreach (DSNode nodeToDelete in nodesToDelete)
            //{
            //    if (nodeToDelete.Group != null)
            //    {
            //        nodeToDelete.Group.RemoveElement(nodeToDelete);
            //    }

            //    RemoveUngroupedNode(nodeToDelete);

            //    nodeToDelete.DisconnectAllPorts();

            //    RemoveElement(nodeToDelete);
            //}
        };
    }

    void OnGroupElementsAdded()
    {
        //todo
        elementsAddedToGroup = (group, elements) =>
        {
            //foreach (GraphElement element in elements)
            //{
            //    if (!(element is DSNode))
            //    {
            //        continue;
            //    }

            //    DSGroup dsGroup = (DSGroup)group;
            //    DSNode node = (DSNode)element;

            //    RemoveUngroupedNode(node);
            //    AddGroupedNode(node, dsGroup);
            //}
        };
    }

    void OnGroupElementsRemoved()
    {
        //todo
        elementsRemovedFromGroup = (group, elements) =>
        {
            //foreach (GraphElement element in elements)
            //{
            //    if (!(element is DSNode))
            //    {
            //        continue;
            //    }

            //    DSGroup dsGroup = (DSGroup)group;
            //    DSNode node = (DSNode)element;

            //    RemoveGroupedNode(node, dsGroup);
            //    AddUngroupedNode(node);
            //}
        };
    }

    void OnGroupRenamed()
    {
        //todo
        groupTitleChanged = (group, newTitle) =>
        {
            //DSGroup dsGroup = (DSGroup)group;

            //dsGroup.title = newTitle.RemoveWhitespaces().RemoveSpecialCharacters();

            //if (string.IsNullOrEmpty(dsGroup.title))
            //{
            //    if (!string.IsNullOrEmpty(dsGroup.OldTitle))
            //    {
            //        ++NameErrorsAmount;
            //    }
            //}
            //else
            //{
            //    if (string.IsNullOrEmpty(dsGroup.OldTitle))
            //    {
            //        --NameErrorsAmount;
            //    }
            //}

            //RemoveGroup(dsGroup);

            //dsGroup.OldTitle = dsGroup.title;

            //AddGroup(dsGroup);
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
}
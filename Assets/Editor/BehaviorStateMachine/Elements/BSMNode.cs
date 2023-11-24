using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BSMNode : Node
{
    public string ID { get; set; }
    public string NodeName { get; set; }

    protected BehaviorStateMachineGraphView m_graphView;

    static Color defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("Disconnect Input Ports", actionEvent => DisconnectInputPorts());
        evt.menu.AppendAction("Disconnect Output Ports", actionEvent => DisconnectOutputPorts());

        base.BuildContextualMenu(evt);
    }

    public virtual void Initialize(string nodeName, BehaviorStateMachineGraphView view, Vector2 position)
    {
        ID = Guid.NewGuid().ToString();
        NodeName = nodeName;

        SetPosition(new Rect(position, Vector2.zero));

        m_graphView = view;

        mainContainer.AddToClassList("bsm-node__main-container");
        extensionContainer.AddToClassList("bsm-node__extension-container");
    }

    public virtual void Draw()
    {
        /* TITLE CONTAINER */

        TextField dialogueNameTextField = BSMUtility.CreateTextField(NodeName, null, callback =>
        {
            TextField target = (TextField)callback.target;

            target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

            if (string.IsNullOrEmpty(target.value))
            {
                if (!string.IsNullOrEmpty(NodeName))
                {
                    m_graphView.NameErrorsAmount++;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(NodeName))
                {
                    m_graphView.NameErrorsAmount--;
                }
            }

            m_graphView.RemoveUngroupedNode(this);

            NodeName = target.value;

            m_graphView.AddUngroupedNode(this);
        });

        dialogueNameTextField.AddClasses(
            "bsm-node__text-field",
            "bsm-node__text-field__hidden",
            "bsm-node__filename-text-field"
        );

        titleContainer.Insert(0, dialogueNameTextField);

        /* INPUT CONTAINER */

        Port inputPort = this.CreatePort("In", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

        inputContainer.Add(inputPort);

        /* OUTPUT CONTAINER */

        Port outputPort = this.CreatePort("Out", Orientation.Horizontal, Direction.Output, Port.Capacity.Multi);

        outputContainer.Add(outputPort);
    }

    public void DisconnectAllPorts()
    {
        DisconnectInputPorts();
        DisconnectOutputPorts();
    }

    private void DisconnectInputPorts()
    {
        DisconnectPorts(inputContainer);
    }

    private void DisconnectOutputPorts()
    {
        DisconnectPorts(outputContainer);
    }

    private void DisconnectPorts(VisualElement container)
    {
        foreach (Port port in container.Children())
        {
            if (!port.connected)
            {
                continue;
            }

            m_graphView.DeleteElements(port.connections);
        }
    }

    public void SetErrorStyle(Color color)
    {
        mainContainer.style.backgroundColor = color;
    }

    public void ResetStyle()
    {
        mainContainer.style.backgroundColor = defaultBackgroundColor;
    }
}

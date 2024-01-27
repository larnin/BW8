using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BSMNodeLabel : BSMNode
{
    bool m_startNode = false;
    public bool isStartNode { get { return m_startNode; } set { m_startNode = value; } }

    public override void Draw()
    {
        /* TITLE CONTAINER */

        if (isStartNode)
        {
            Label labelName = new Label(NodeName);
            labelName.style.paddingBottom = 8;
            labelName.style.paddingLeft = 8;
            labelName.style.paddingRight = 8;
            labelName.style.paddingTop = 8;

            titleContainer.Insert(0, labelName);
        }
        else
        {
            string lbl = "LABEL_";

            string localName = NodeName;
            if (localName.StartsWith(lbl))
                localName.Remove(0, lbl.Length);

            TextField dialogueNameTextField = BSMEditorUtility.CreateTextField(localName, null, callback =>
            {
                TextField target = (TextField)callback.target;

                target.value = callback.newValue.RemoveSpecialCharacters();

                NodeName = lbl + target.value;

                m_graphView.ProcessErrors();
            });

            dialogueNameTextField.AddClasses(
                "bsm-node__text-field",
                "bsm-node__text-field__hidden",
                "bsm-node__filename-text-field"
            );

            titleContainer.Insert(0, dialogueNameTextField);
        }

        /* OUTPUT CONTAINER */

        Port outputPort = this.CreatePort("Out", Orientation.Horizontal, Direction.Output, Port.Capacity.Multi);

        outputContainer.Add(outputPort);

        RefreshExpandedState();
    }

    public override void UpdateStyle(bool error)
    {
        base.UpdateStyle(error);

        if (error)
        {
            mainContainer.style.backgroundColor = errorBackgroundColor;
            mainContainer.style.borderBottomColor = errorBorderColor;
            mainContainer.style.borderLeftColor = errorBorderColor;
            mainContainer.style.borderRightColor = errorBorderColor;
            mainContainer.style.borderTopColor = errorBorderColor;
        }
        else
        {
            Color backgroundColor;
            Color borderColor;
            if (m_startNode)
            {
                backgroundColor = new Color(0.4f, 0.2f, 0);
                borderColor = new Color(1.0f, 0.5f, 0);
            }
            else
            {
                backgroundColor = new Color(0.2f, 0.2f, 0.2f);
                borderColor = new Color(0.6f, 0.6f, 0.6f);
            }

            mainContainer.style.backgroundColor = backgroundColor;
            mainContainer.style.borderBottomColor = borderColor;
            mainContainer.style.borderLeftColor = borderColor;
            mainContainer.style.borderRightColor = borderColor;
            mainContainer.style.borderTopColor = borderColor;
        }

        if(m_startNode)
        {
            float radius = 8;

            mainContainer.style.borderBottomLeftRadius = radius;
            mainContainer.style.borderBottomRightRadius = radius;
            mainContainer.style.borderTopLeftRadius = radius;
            mainContainer.style.borderTopRightRadius = radius;
        }
        else 
        {
            float smallRadius = 2;
            float largeRadius = 15;

            mainContainer.style.borderBottomLeftRadius = smallRadius;
            mainContainer.style.borderBottomRightRadius = largeRadius;
            mainContainer.style.borderTopLeftRadius = smallRadius;
            mainContainer.style.borderTopRightRadius = largeRadius;
        }
    }
}

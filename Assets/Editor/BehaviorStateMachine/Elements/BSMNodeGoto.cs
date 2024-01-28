using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BSMNodeGoto : BSMNode, BSMDropdownCallback
{
    public static string lbl = "GOTO_";

    Button m_labelButton;
    string m_labelID;

    public override void Draw()
    {
        /* TITLE CONTAINER */

        string localName = NodeName;
        if (localName.StartsWith(lbl))
            localName = localName.Remove(0, lbl.Length);

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

        dialogueNameTextField.style.minWidth = 100;

        titleContainer.Insert(0, dialogueNameTextField);

        /* OUTPUT CONTAINER */

        Port outputPort = this.CreatePort("In", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

        inputContainer.Add(outputPort);

        DrawGoto();

        RefreshExpandedState();
    }

    void DrawGoto()
    {
        VisualElement block = new VisualElement();
        block.style.flexDirection = FlexDirection.Row;
        block.style.justifyContent = Justify.SpaceBetween;
        var label = new Label("Target");
        label.style.paddingBottom = 4;
        label.style.paddingLeft = 4;
        label.style.paddingRight = 4;
        label.style.paddingTop = 4;
        block.Add(label);

        m_labelButton = BSMUtility.CreateButton(GetButtonText(), CreatePopup);

        block.Add(m_labelButton);

        mainContainer.Add(block);
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
            Color backgroundColor = new Color(0.2f, 0.2f, 0.2f); ;
            Color borderColor = new Color(0.6f, 0.6f, 0.6f); ;

            mainContainer.style.backgroundColor = backgroundColor;
            mainContainer.style.borderBottomColor = borderColor;
            mainContainer.style.borderLeftColor = borderColor;
            mainContainer.style.borderRightColor = borderColor;
            mainContainer.style.borderTopColor = borderColor;
        }

        float smallRadius = 2;
        float largeRadius = 15;

        mainContainer.style.borderBottomLeftRadius = largeRadius;
        mainContainer.style.borderBottomRightRadius = smallRadius;
        mainContainer.style.borderTopLeftRadius = largeRadius;
        mainContainer.style.borderTopRightRadius = smallRadius;
    }

    BSMNodeLabel GetLabel(string ID)
    {
        var nodes = m_graphView.GetAllLabelNodes();
        foreach(var node in nodes)
        {
            if (node.ID == ID)
                return node;
        }

        return null;
    }

    void CreatePopup()
    {
        if (m_labelButton == null)
            return;

        var pos = m_labelButton.LocalToWorld(new Vector2(0, 0));
        pos.y -= 100;
        var rect = new Rect(pos, new Vector2(200, 100));

        List<String> names = new List<string>();
        var nodes = m_graphView.GetAllLabelNodes();

        foreach(var node in nodes)
        {
            string name = node.NodeName;
            if (name.StartsWith(BSMNodeLabel.lbl))
                name = name.Remove(0, BSMNodeLabel.lbl.Length);

            names.Add(name);
        }

        UnityEditor.PopupWindow.Show(rect, new BSMSearchDropdownPopup(names, this));
    }

    public void SendResult(int result)
    {
        var nodes = m_graphView.GetAllLabelNodes();
        if (result < 0 || result >= nodes.Count)
            m_labelID = "";
        else m_labelID = nodes[result].ID;

        UpdateButtonText();
    }

    string GetButtonText()
    {
        var node = m_labelID == null ? null : GetLabel(m_labelID);
        string nodeName = node == null ? "Not set" : node.NodeName;

        if (nodeName.StartsWith(BSMNodeLabel.lbl))
            nodeName = nodeName.Remove(0, BSMNodeLabel.lbl.Length);

        return nodeName;
    }

    void UpdateButtonText()
    {
        if(m_labelButton != null)
            m_labelButton.text = GetButtonText();
    }

    public void UpdateTarget()
    {
        UpdateButtonText();
    }

    public string GetLabelID()
    {
        return m_labelID;
    }

    public void SetLabelID(string id)
    {
        m_labelID = id;
        UpdateButtonText();
    }
}

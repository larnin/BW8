using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;

public class BSMConditionNode : BSMNode
{
    BSMConditionBase m_condition = null;

    public override void Draw() 
    {
        base.Draw();

        if (m_condition == null)
            DrawSetCondition();
        else DrawCondition();
        
        RefreshExpandedState();
    }

    void DrawSetCondition()
    {
        Button addChoiceButton = BSMUtility.CreateButton("Set condition", CreatePopup);
        extensionContainer.Add(addChoiceButton);
    }

    void DrawCondition()
    {

    }

    public void SetCondition(BSMConditionBase condition)
    {
        m_condition = condition;
    }

    void CreatePopup()
    {
        var pos = GetPosition();

        UnityEditor.PopupWindow.Show(pos, new BSMConditionNodePopup(this));
    }
}

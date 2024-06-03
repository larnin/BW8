using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public abstract class BSMActionViewBase : BSMAttributeHolderView
{
    public BSMActionViewBase(BSMNode node) : base(node) { }

    public abstract BSMActionBase GetAction();

    public static BSMActionViewBase Create(BSMNode node, BSMActionBase action)
    {
        return new BSMActionViewDefault(node, action);
    }

    protected override VisualElement GetActionsElement()
    {
        return null;
    }
}

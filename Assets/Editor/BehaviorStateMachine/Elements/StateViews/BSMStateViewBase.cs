using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BSMStateViewBase : BSMAttributeHolderView
{
    public BSMStateViewBase(BSMNode node) : base(node) { }

    public abstract BSMStateBase GetState();

    public static BSMStateViewBase Create(BSMNode node, BSMStateBase state)
    {
        if (state is BSMStateIdle)
            return new BSMStateViewIdle(node, state as BSMStateIdle);

        if (state is BSMStateFollowEntity)
            return new BSMStateViewFollowEntity(node, state as BSMStateFollowEntity);

        Debug.LogError("No state view for type " + state.GetType().ToString());
        return null;
    }
}

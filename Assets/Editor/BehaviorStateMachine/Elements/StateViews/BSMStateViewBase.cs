using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BSMStateViewBase
{
    public abstract VisualElement GetElement();
    public abstract BSMStateBase GetState();

    public static BSMStateViewBase Create(BSMStateBase state)
    {
        if (state is BSMStateIdle)
            return new BSMStateViewIdle(state as BSMStateIdle);

        Debug.LogError("No state view for type " + state.GetType().ToString());
        return null;
    }
}

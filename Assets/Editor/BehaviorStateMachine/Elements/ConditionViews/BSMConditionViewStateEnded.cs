using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class BSMConditionViewStateEnded : BSMConditionViewBase
{
    BSMConditionStateEnded m_condition;

    public BSMConditionViewStateEnded(BSMConditionStateEnded condition)
    {
        m_condition = condition;
    }

    public override VisualElement GetElement()
    {
        return new HelpBox("Transition when the previous state is ended", HelpBoxMessageType.Info);
    }
}

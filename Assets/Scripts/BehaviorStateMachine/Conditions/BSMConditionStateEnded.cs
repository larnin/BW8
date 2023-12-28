using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class BSMConditionStateEnded : BSMConditionBase
{
    public override VisualElement GetElement()
    {
        return new HelpBox("Transition when the previous state is ended", HelpBoxMessageType.Info);
    }
}

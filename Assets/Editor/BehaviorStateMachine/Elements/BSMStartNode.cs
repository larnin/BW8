using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class BSMStartNode : BSMNode
{
    public override void Draw() 
    {
        /* TITLE CONTAINER */

        Label labelName = new Label(NodeName);

        labelName.AddClasses(
            "bsm-node__text-field",
            "bsm-node__text-field__hidden",
            "bsm-node__filename-text-field"
        );

        titleContainer.Insert(0, labelName);

        /* OUTPUT CONTAINER */

        Port outputPort = this.CreatePort("Out", Orientation.Horizontal, Direction.Output, Port.Capacity.Multi);

        outputContainer.Add(outputPort);

        RefreshExpandedState();
    }
}

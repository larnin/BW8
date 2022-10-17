using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class QuestFailConditionObjectBase
{
    public abstract string GetFailConditionName();

    public abstract QuestFailConditionBase MakeFailCondition();

    public void InspectorGUI()
    {
        OnInspectorGUI();
    }

    protected virtual void OnInspectorGUI() { }
}

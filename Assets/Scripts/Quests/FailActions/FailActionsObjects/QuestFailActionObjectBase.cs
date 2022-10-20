using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class QuestFailActionObjectBase
{
    public abstract string GetFailActionName();

    public abstract QuestFailActionBase MakeFailAction();

    public void InspectorGUI()
    {
        OnInspectorGUI();
    }

    protected virtual void OnInspectorGUI() { }
}

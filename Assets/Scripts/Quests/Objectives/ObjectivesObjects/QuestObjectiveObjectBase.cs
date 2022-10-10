using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class QuestObjectiveObjectBase
{
    public abstract string GetObectiveName();

    public abstract QuestObjectiveBase MakeObjective();

    public void OnInspectorGUI()
    {
        OnSpecificInspectorGUI();
    }

    protected abstract void OnSpecificInspectorGUI();
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum QuestCompletionState
{
    Running,
    Completed,
    Failed,
}

public abstract class QuestObjectiveBase
{
    public abstract void OnStart();
    
    public abstract void OnCompletion();
    
    public abstract void OnFail();

    public abstract QuestCompletionState Update();
}

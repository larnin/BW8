using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class QuestFailConditionBase
{
    public virtual void Start() { }

    public virtual void End(QuestCompletionState state) { }

    public abstract QuestCompletionState Update();
}
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
    List<QuestFailConditionBase> m_failConditions = new List<QuestFailConditionBase>();

    public QuestObjectiveBase(QuestObjectiveObjectBase questObject)
    {
        MakeFailConditions(questObject);
    }

    public void Start()
    {
        foreach (var f in m_failConditions)
            f.Start();

        OnStart();
    }

    protected virtual void OnStart() { }

    public void End(QuestCompletionState state)
    {
        foreach (var f in m_failConditions)
            f.End(state);

        OnEnd(state);
    }

    protected virtual void OnEnd(QuestCompletionState state) { }

    public QuestCompletionState Update()
    {
        foreach(var f in m_failConditions)
        {
            var state = f.Update();
            if (state == QuestCompletionState.Failed)
                return state;
        }

        return OnUpdate();
    }

    protected abstract QuestCompletionState OnUpdate();

    void MakeFailConditions(QuestObjectiveObjectBase questObject)
    {
        if (questObject.m_failConditions == null || questObject.m_failConditions.Count == 0)
            return;

        foreach (var fail in questObject.m_failConditions)
            m_failConditions.Add(fail.MakeFailCondition());
    }
}

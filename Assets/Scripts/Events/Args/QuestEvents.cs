using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StartQuestObjectiveEvent
{
    public int questID;
    public int objectiveIndex;

    public StartQuestObjectiveEvent(int _questID, int _objectiveIndex)
    {
        questID = _questID;
        objectiveIndex = _objectiveIndex;
    }
}

public class StopQuestObjectiveEvent
{
    public int questID;
    public int objectiveIndex;

    public StopQuestObjectiveEvent(int _questID, int _objectiveIndex)
    {
        questID = _questID;
        objectiveIndex = _objectiveIndex;
    }
}

public class CompleteQuestObjectiveEvent
{
    public int questID;
    public int objectiveIndex;

    public CompleteQuestObjectiveEvent(int _questID, int _objectiveIndex)
    {
        questID = _questID;
        objectiveIndex = _objectiveIndex;
    }
}

public class ResetQuestObjectiveEvent
{
    public int questID;
    public int objectiveIndex;

    public ResetQuestObjectiveEvent(int _questID, int _objectiveIndex)
    {
        questID = _questID;
        objectiveIndex = _objectiveIndex;
    }
}

public class IsQuestCompletedEvent
{
    public int questID;
    public bool completed;

    public IsQuestCompletedEvent(int _questID)
    {
        questID = _questID;
        completed = false;
    }
}

public class IsQuestObjectiveCompletedEvent
{
    public int questID;
    public int objectiveIndex;
    public QuestSystem.ObjectiveState state;

    public IsQuestObjectiveCompletedEvent(int _questID, int _objective)
    {
        questID = _questID;
        objectiveIndex = _objective;
        state = QuestSystem.ObjectiveState.NotStarted;
    }
}

public class IsQuestActiveEvent
{
    public int questID;
    public bool active;

    public IsQuestActiveEvent(int _questID)
    {
        questID = _questID;
        active = false;
    }
}

public class RegisterQuestEntityEvent
{
    public QuestEntity entity;
    public string id;

    public RegisterQuestEntityEvent(QuestEntity _entity, string _id)
    {
        entity = _entity;
        id = _id;
    }
}

public class UnregisterQuestEntityEvent
{
    public QuestEntity entity;
    public string id;

    public UnregisterQuestEntityEvent(QuestEntity _entity, string _id)
    {
        entity = _entity;
        id = _id;
    }
}

public class GetFirstQuestEntityEvent
{
    public string id;
    public QuestEntity entity;

    public GetFirstQuestEntityEvent(string _id)
    {
        id = _id;
    }
}

public class GetAllQuestEntityEvent
{
    public string id;
    public List<QuestEntity> entities;

    public GetAllQuestEntityEvent(string _id)
    {
        id = _id;
        entities = new List<QuestEntity>();
    }
}

public class GetQuestEntityIDEvent
{
    public string id;
}

public class QuestObjectiveCompletedEvent
{
    public int questID;
    public int objectiveIndex;

    public QuestObjectiveCompletedEvent(int _questID, int _objectiveIndex)
    {
        questID = _questID;
        objectiveIndex = _objectiveIndex;
    }
}

public class QuestObjectiveFailedEvent
{
    public int questID;
    public int objectiveIndex;

    public QuestObjectiveFailedEvent(int _questID, int _objectiveIndex)
    {
        questID = _questID;
        objectiveIndex = _objectiveIndex;
    }
}

public class QuestStartTalkEvent
{
    public QuestEntity entity;

    public QuestStartTalkEvent(QuestEntity _entity)
    {
        entity = _entity;
    }
}

public class QuestKillEntityEvent
{
    public QuestEntity entity;
    
    public QuestKillEntityEvent(QuestEntity _entity)
    {
        entity = _entity;
    }
}
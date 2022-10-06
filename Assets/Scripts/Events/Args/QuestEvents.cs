using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
    public bool completed;

    public IsQuestObjectiveCompletedEvent(int _questID, int _objective)
    {
        questID = _questID;
        objectiveIndex = _objective;
        completed = false;
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



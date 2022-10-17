using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    class QuestData
    {
        public int questID;
        public int objectiveIndex;
        public QuestObjectiveBase objective;
    }

    List<int> m_completedQuests = new List<int>();
    List<QuestData> m_activeQuests = new List<QuestData>();

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<IsQuestActiveEvent>.Subscriber(IsQuestActive));
        m_subscriberList.Add(new Event<IsQuestCompletedEvent>.Subscriber(IsQuestCompleted));
        m_subscriberList.Add(new Event<IsQuestObjectiveCompletedEvent>.Subscriber(IsQuestObjectiveCompleted));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    private void Start()
    {
        int nbQuest = QuestList.GetQuestNb();
        for(int i = 0; i < nbQuest; i++)
        {
            var quest = QuestList.GetQuestFromIndex(i);
            if(quest.parentQuestID == QuestObject.invalidQuestID)
                StartQuestObjective(quest.questID, 0);
        }
    }

    private void Update()
    {
        List<QuestData> nextQuests = new List<QuestData>();
        List<QuestData> completedQuests = new List<QuestData>();
        List<QuestData> failedQuests = new List<QuestData>();

        foreach(var quest in m_activeQuests)
        {
            var result = quest.objective.Update();

            if (result == QuestCompletionState.Running)
                nextQuests.Add(quest);
            else if (result == QuestCompletionState.Completed)
                completedQuests.Add(quest);
            else if (result == QuestCompletionState.Failed)
                failedQuests.Add(quest);
        }

        m_activeQuests = nextQuests;

        foreach (var quest in completedQuests)
            OnQuestObjectiveCompleted(quest.questID, quest.objectiveIndex, quest.objective);
        foreach (var quest in failedQuests)
            OnQuestObjectiveFailed(quest.questID, quest.objectiveIndex, quest.objective);
    }

    void IsQuestCompleted(IsQuestCompletedEvent e)
    {
        e.completed = false;

        foreach (var q in m_completedQuests)
        {
            if (e.questID == q)
            {
                e.completed = true;
                return;
            }
        }
    }

    void IsQuestObjectiveCompleted(IsQuestObjectiveCompletedEvent e)
    {
        e.completed = false;

        foreach (var q in m_completedQuests)
        {
            if (e.questID == q)
            {
                e.completed = true;
                return;
            }
        }

        foreach(var q in m_activeQuests)
        {
            if(e.questID == q.questID)
            {
                if (e.objectiveIndex < q.objectiveIndex)
                    e.completed = true;
                else e.completed = false;

                return;
            }
        }
    }

    void IsQuestActive(IsQuestActiveEvent e)
    {
        e.active = false;
        foreach (var q in m_activeQuests)
        {
            if (e.questID == q.questID)
            {
                e.active = true;
                return;
            }
        }
    }

    QuestData StartQuestObjective(int questID, int objectiveIndex)
    {
        QuestData data = new QuestData();
        data.questID = questID;
        data.objectiveIndex = objectiveIndex;

        var quest = QuestList.GetQuest(questID);
        if(quest == null)
        {
            Debug.LogError("Start Quest - Unable to find quest ID " + questID);
            return data;
        }

        if(objectiveIndex < 0 || objectiveIndex >= quest.GetObjectiveNb())
        {
            Debug.LogError("Start Quest - Unable to start objective " + objectiveIndex + " on quest " + quest.questName + " ID " + questID);
            return data;
        }

        data.objective = quest.GetObjective(objectiveIndex).MakeObjective();

        data.objective.Start();

        return data;
    }

    void OnQuestObjectiveCompleted(int questID, int questObjectiveIndex, QuestObjectiveBase questObjective)
    {
        questObjective.End(QuestCompletionState.Completed);

        var quest = QuestList.GetQuest(questID);
        if (quest == null)
            return;

        if(questObjectiveIndex == quest.GetObjectiveNb() - 1)
        {
            StartNextQuest(questID);
            return;
        }

        StartQuestObjective(questID, questObjectiveIndex + 1);

        Event<QuestObjectiveCompletedEvent>.Broadcast(new QuestObjectiveCompletedEvent(questID, questObjectiveIndex));
    }

    void StartNextQuest(int questID)
    {
        m_completedQuests.Add(questID);

        int nbQuest = QuestList.GetQuestNb();
        for (int i = 0; i < nbQuest; i++)
        {
            var quest = QuestList.GetQuestFromIndex(i);

            if (quest.parentQuestID == questID)
                StartQuestObjective(quest.questID, 0);
        }
    }

    void OnQuestObjectiveFailed(int questID, int questObjectiveIndex, QuestObjectiveBase questObjective)
    {
        questObjective.End(QuestCompletionState.Failed);

        //todo

        Event<QuestObjectiveFailedEvent>.Broadcast(new QuestObjectiveFailedEvent(questID, questObjectiveIndex));
    }
}
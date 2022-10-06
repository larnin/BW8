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
            {
                var data = new QuestData();
                data.questID = quest.questID;
                data.objectiveIndex = 0;
                
                m_activeQuests.Add(data);
            }
        }
    }

    private void Update()
    {
        
    }

    void UpdateQuest(QuestData data)
    {

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
            Debug.LogError("Unable to start objective index " + objectiveIndex + " on quest ID " + questID);
            return data;
        }



        return data;
    }
}
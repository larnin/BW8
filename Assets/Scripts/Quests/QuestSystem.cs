using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    public enum ObjectiveState
    {
        NotStarted,
        Started,
        Completed,
    }

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
        m_subscriberList.Add(new Event<StartQuestObjectiveEvent>.Subscriber(StartQuestObjective));
        m_subscriberList.Add(new Event<StopQuestObjectiveEvent>.Subscriber(StopQuestObjective));
        m_subscriberList.Add(new Event<CompleteQuestObjectiveEvent>.Subscriber(CompleteQuestObjective));
        m_subscriberList.Add(new Event<ResetQuestObjectiveEvent>.Subscriber(ResetQuestObjective));
        m_subscriberList.Add(new Event<DrawDebugWindowEvent>.Subscriber(DebugDraw));
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
        e.state = ObjectiveState.NotStarted;

        foreach (var q in m_completedQuests)
        {
            if (e.questID == q)
            {
                e.state = ObjectiveState.Completed;
                return;
            }
        }

        foreach(var q in m_activeQuests)
        {
            if(e.questID == q.questID)
            {
                if (e.objectiveIndex == q.objectiveIndex)
                    e.state = ObjectiveState.Started;
                else if (e.questID < q.questID)
                    e.state = ObjectiveState.Completed;
                else e.state = ObjectiveState.NotStarted;
                return;
            }
        }
    }

    void StartQuestObjective(StartQuestObjectiveEvent e)
    {
        m_completedQuests.Remove(e.questID);
        m_activeQuests.RemoveAll(x => (x.questID == e.questID));

        StartQuestObjective(e.questID, e.objectiveIndex);
    }

    void StopQuestObjective(StopQuestObjectiveEvent e)
    {
        m_activeQuests.RemoveAll(x => (x.questID == e.questID && x.objectiveIndex == e.objectiveIndex));
    }

    void CompleteQuestObjective(CompleteQuestObjectiveEvent e)
    {
        if (m_activeQuests.Find(x => (x.questID == e.questID && x.objectiveIndex == e.objectiveIndex)) == null)
            StartQuestObjective(new StartQuestObjectiveEvent(e.questID, e.objectiveIndex));

        var quest = m_activeQuests.Find(x => (x.questID == e.questID && x.objectiveIndex == e.objectiveIndex));
        if(quest != null)
            OnQuestObjectiveCompleted(quest.questID, quest.objectiveIndex, quest.objective);
    }

    void ResetQuestObjective(ResetQuestObjectiveEvent e)
    {
        m_completedQuests.Remove(e.questID);
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

    void StartQuestObjective(int questID, int objectiveIndex)
    {
        QuestData data = new QuestData();
        data.questID = questID;
        data.objectiveIndex = objectiveIndex;

        var quest = QuestList.GetQuest(questID);
        if(quest == null)
        {
            DebugLogs.LogError("Start Quest - Unable to find quest ID " + questID);
            return;
        }

        if(objectiveIndex < 0 || objectiveIndex >= quest.GetObjectiveNb())
        {
            DebugLogs.LogError("Start Quest - Unable to start objective " + objectiveIndex + " on quest " + quest.questName + " ID " + questID);
            return;
        }

        data.objective = quest.GetObjective(objectiveIndex).MakeObjective();

        data.objective.Start();

        m_activeQuests.Add(data);
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

        m_activeQuests.RemoveAll(x => (x.questID == questID && x.objectiveIndex == questObjectiveIndex));
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

        Event<QuestObjectiveFailedEvent>.Broadcast(new QuestObjectiveFailedEvent(questID, questObjectiveIndex));

        m_activeQuests.RemoveAll(x => (x.questID == questID && x.objectiveIndex == questObjectiveIndex));
    }


    class DebugInfos
    {
        public Vector2 scrollPos;
        List<bool> folded = new List<bool>();
        int foldIndex = 0;

        GUIStyle ButtonStyle()
        {
            var style = new GUIStyle(GUI.skin.label);

            style.hover.textColor = new Color(1.0f, 1.0f, 1.0f);
            style.normal.textColor = new Color(0.8f, 0.8f, 0.8f);

            return style;
        }

        public void Start()
        {
            foldIndex = 0;
        }

        public bool BeginFolded(string name)
        {
            bool folded = GetFolded(foldIndex);

            string text = (folded ? "➤" : "▼") + " " + name;

            if (GUILayout.Button(text, ButtonStyle()))
                folded = !folded;

            SetFolded(foldIndex, folded);

            if(!folded)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical();
            }

            foldIndex++;

            return folded;
        }

        public void EndFolded()
        {
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }


        void SetFolded(int index, bool _folded)
        {
            while (folded.Count <= index)
                folded.Add(false);
            folded[index] = _folded;
        }

        bool GetFolded(int index)
        {
            if (index >= folded.Count)
                return false;
            return folded[index];
        }
    }

    DebugInfos m_debugInfos = null;
    
    void DebugDraw(DrawDebugWindowEvent e)
    {
        if (e.type != DebugWindowType.Window_Quest)
            return;

        if (m_debugInfos == null)
            m_debugInfos = new DebugInfos();

        m_debugInfos.Start();

        m_debugInfos.scrollPos = GUILayout.BeginScrollView(m_debugInfos.scrollPos);

        int nbQuest = QuestList.GetQuestNb();
        for (int i = 0; i < nbQuest; i++)
        {
            var quest = QuestList.GetQuestFromIndex(i);
            DrawOneQuest(quest);
        }

        GUILayout.EndScrollView();
    }

    void DrawOneQuest(QuestObject quest)
    {
        IsQuestActiveEvent questActive = new IsQuestActiveEvent(quest.questID);
        Event<IsQuestActiveEvent>.Broadcast(questActive);

        IsQuestCompletedEvent questCompleted = new IsQuestCompletedEvent(quest.questID);
        Event<IsQuestCompletedEvent>.Broadcast(questCompleted);

        string name = quest.questName;
        if (questActive.active)
            name += " (Active)";
        else if (questCompleted.completed)
            name += " (Completed)";

        GUILayout.BeginVertical(GUI.skin.box);
        if (!m_debugInfos.BeginFolded(name))
        {
            int nbObjectives = quest.GetObjectiveNb();
            for(int i = 0; i < nbObjectives; i++)
            {
                IsQuestObjectiveCompletedEvent objectiveCompleted = new IsQuestObjectiveCompletedEvent(quest.questID, i);
                Event<IsQuestObjectiveCompletedEvent>.Broadcast(objectiveCompleted);

                var state = objectiveCompleted.state;
                var objective = quest.GetObjective(i);

                GUILayout.BeginHorizontal();

                Color oldColor = GUI.color;
                if (state == ObjectiveState.NotStarted)
                    GUI.color = Color.white;
                else if (state == ObjectiveState.Started)
                    GUI.color = Color.cyan;
                else GUI.color = Color.green;

                GUILayout.Label(objective.GetObjectiveName());

                GUI.color = oldColor;

                if(questActive.active)
                {
                    if(objectiveCompleted.state == ObjectiveState.Started)
                    {
                        if(GUILayout.Button("Stop", GUILayout.MaxWidth(70)))
                            Event<StopQuestObjectiveEvent>.Broadcast(new StopQuestObjectiveEvent(quest.questID, i));
                    }
                }
                else if(GUILayout.Button("Start", GUILayout.MaxWidth(70)))
                    Event<StartQuestObjectiveEvent>.Broadcast(new StartQuestObjectiveEvent(quest.questID, i));

                if (!questCompleted.completed)
                {
                    if (GUILayout.Button("Complete", GUILayout.MaxWidth(70)))
                        Event<CompleteQuestObjectiveEvent>.Broadcast(new CompleteQuestObjectiveEvent(quest.questID, i));
                }
                else if (GUILayout.Button("Reset", GUILayout.MaxWidth(70)))
                    Event<ResetQuestObjectiveEvent>.Broadcast(new ResetQuestObjectiveEvent(quest.questID, i));


                GUILayout.EndHorizontal();
            }

            m_debugInfos.EndFolded();
        }
        GUILayout.EndVertical();
    }
}
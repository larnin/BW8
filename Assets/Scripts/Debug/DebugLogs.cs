using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DebugLogs : MonoBehaviour
{
    const int maxLines = 500;

    static DebugLogs m_instance = null;
    static DebugLogs instance { get { return m_instance; } }

    List<string> m_logs = new List<string>();
    SubscriberList m_subscriberList = new SubscriberList();

    void Awake()
    {
        if (m_instance != null)
            Destroy(gameObject);
        m_instance = this;
        DontDestroyOnLoad(gameObject);

        m_subscriberList.Add(new Event<DrawDebugWindowEvent>.Subscriber(DebugDraw));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void AddLog(string str)
    {
        m_logs.Add(str);
        while (m_logs.Count > maxLines)
            m_logs.RemoveAt(0);
    }

    void DebugDraw(DrawDebugWindowEvent e)
    {
        if (e.type != DebugWindowType.Window_Log)
            return;

        for(int i = 0; i < m_logs.Count; i++)
        {
            GUILayout.Label(m_logs[i]);
        }
    }

    public static void Log(string str)
    {
        if(instance != null)
            instance.AddLog(str);
        Debug.Log(str);
    }

    public static void LogWarning(string str)
    {
        if (instance != null)
            instance.AddLog("WARNING: " + str);
        Debug.LogWarning(str);
    }

    public static void LogError(string str)
    {
        if (instance != null)
            instance.AddLog("ERROR: " + str);
        Debug.LogError(str);
    }
}

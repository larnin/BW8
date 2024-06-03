using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMActionNamed
{
    public string name;
    public List<BSMActionBase> actions = new List<BSMActionBase>();
}

public abstract class BSMStateBase : BSMAttributeHolder
{
    static string BeginName = "BeforeState";
    static string EndName = "AfterState";

    [SerializeField] protected List<BSMActionNamed> m_actions = new List<BSMActionNamed>();

    protected BSMControler m_controler;

    public BSMStateBase()
    {
        AddActionName(BeginName);
        AddActionName(EndName);
    }

    public static string GetName(Type type)
    {
        const string startString = "BSMState";

        string name = type.Name;
        if (name.StartsWith(startString))
            name = name.Substring(startString.Length);

        return name;
    }

    public void SetControler(BSMControler controler) 
    { 
        m_controler = controler;

        if (m_actions == null)
            return;

        foreach(var a in m_actions)
        {
            if (a.actions == null)
                continue;
            foreach (var action in a.actions)
                action.SetControler(m_controler);
        }
    }

    public override BSMControler GetControler() { return m_controler; }

    protected void AddActionName(string name)
    {
        if (m_actions == null)
            m_actions = new List<BSMActionNamed>();
        foreach (var a in m_actions)
        {
            if (a.name == name)
                return;
        }

        var action = new BSMActionNamed();
        action.name = name;
        m_actions.Add(action);
    }

    public void AddAction(string name, BSMActionBase action)
    {
        if (m_actions == null)
            return;
        foreach (var a in m_actions)
        {
            if (a.name == name)
            {
                if (a.actions == null)
                    a.actions = new List<BSMActionBase>();
                a.actions.Add(action);
                action.SetControler(m_controler);
                break;
            }
        }
    }

    public void RemoveAction(string name, int index)
    {
        if (m_actions == null)
            return;
        foreach (var a in m_actions)
        {
            if (a.name == name && a.actions != null)
            {
                if (index >= 0 && index < a.actions.Count)
                    a.actions.RemoveAt(index);
            }
        }
    }

    public List<BSMActionNamed> GetActions()
    {
        return m_actions;
    }

    public void TriggerActions(string name)
    {
        if (m_actions == null)
            return;
        foreach (var a in m_actions)
        {
            if (a.name == name && a.actions != null)
            {
                foreach (var action in a.actions)
                    action.Exec();
            }
        }
    }    
    
    public void InitActions()
    {
        if (m_actions == null)
            return;
        foreach (var a in m_actions)
        {
            if (a.actions == null)
                continue;
            foreach (var action in a.actions)
                action.Init();
        }
    }

    public void OnDestroyActions()
    {
        if (m_actions == null)
            return;
        foreach (var a in m_actions)
        {
            if (a.actions == null)
                continue;
            foreach (var action in a.actions)
                action.OnDestroy();
        }
    }

    public virtual void Init() { }

    public virtual void Update() { }

    public virtual void LateUpdate() { }

    public virtual void FixedUpdate() { }

    public virtual void UpdateAlways() { }

    public void BeginUpdate() 
    {
        TriggerActions(BeginName);
        OnBeginUpdate();
    }
    public virtual void OnBeginUpdate() { }

    public void EndUpdate() 
    {
        OnEndUpdate();
        TriggerActions(EndName);
    }
    public virtual void OnEndUpdate() { }

    public virtual void OnDestroy() { }
}

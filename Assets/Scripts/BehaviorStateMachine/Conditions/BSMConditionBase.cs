using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BSMConditionBase : BSMAttributeHolder
{
    [SerializeField] List<BSMActionBase> m_actions = new List<BSMActionBase>();

    protected BSMStateBase m_state;

    public BSMConditionBase()
    {
        if (m_actions == null)
            m_actions = new List<BSMActionBase>();
    }

    public abstract bool IsValid();

    public virtual void Init() { }

    public virtual void Update() { }

    public virtual void UpdateAlways() { }

    public virtual void BeginUpdate() { }

    public virtual void EndUpdate() { }

    public virtual void OnDestroy() { }

    public virtual void OnStateChange() { }

    public void OnValidation() 
    {
        if(m_actions != null)
        {
            foreach (var a in m_actions)
                a.Exec();
        }
    }

    public void SetState(BSMStateBase state) 
    { 
        m_state = state;
        OnStateChange();

        if (m_state != null && m_actions != null)
        {
            var controler = m_state.GetControler();
            foreach (var a in m_actions)
                a.SetControler(controler);
        }
    }

    public BSMStateBase GetState() { return m_state; }

    public override BSMControler GetControler()
    {
        if (m_state == null)
            return null;
        return m_state.GetControler();
    }

    public static string GetName(Type type)
    {
        const string startString = "BSMCondition";

        string name = type.Name;
        if (name.StartsWith(startString))
            name = name.Substring(startString.Length);

        return name;
    }

    public void AddAction(BSMActionBase action)
    {
        if (m_actions == null)
            return;
        if (m_state != null)
            action.SetControler(m_state.GetControler());
        m_actions.Add(action);
    }

    public void RemoveAction(int index)
    {
        if (m_actions == null)
            return;
        if (index >= 0 && index < m_actions.Count)
            m_actions.RemoveAt(index);
    }

    public List<BSMActionBase> GetActions()
    {
        return m_actions;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BSMConditionBase : BSMAttributeHolder
{
    protected BSMStateBase m_state;

    public abstract bool IsValid();

    public virtual void Init() { }

    public virtual void Update() { }

    public virtual void UpdateAlways() { }

    public virtual void BeginUpdate() { }

    public virtual void EndUpdate() { }

    public virtual void OnDestroy() { }

    public virtual void OnStateChange() { }

    public void SetState(BSMStateBase state) 
    { 
        m_state = state;
        OnStateChange();
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
}

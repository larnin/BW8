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

    public abstract void Load(JsonObject obj);
    public abstract void Save(JsonObject obj);

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

    public static BSMConditionBase LoadCondition(JsonObject obj)
    {
        var typeObj = obj.GetElement("Type");
        if (typeObj == null || !typeObj.IsJsonString())
            return null;

        string typeName = typeObj.String();
        try
        {
            Type t = Type.GetType(typeName);

            var instance = Activator.CreateInstance(t) as BSMConditionBase;
            if (instance == null)
                return null;

            instance.Load(obj);
            instance.LoadAttributes(obj);
            return instance;
        }
        catch(Exception)
        {
            return null;
        }
    }

    public static JsonObject SaveCondition(BSMConditionBase condition)
    {
        var type = condition.GetType();
        string typeName = type.FullName;

        JsonObject obj = new JsonObject();
        obj.AddElement("Type", typeName);

        condition.Save(obj);
        condition.SaveAttributes(obj);

        return obj;
    }
}

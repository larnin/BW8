using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BSMStateBase : BSMAttributeHolder
{
    protected BSMControler m_controler;

    public abstract void Load(JsonObject obj);
    public abstract void Save(JsonObject obj);

    public static string GetName(Type type)
    {
        const string startString = "BSMState";

        string name = type.Name;
        if (name.StartsWith(startString))
            name = name.Substring(startString.Length);

        return name;
    }

    public static BSMStateBase LoadState(JsonObject obj)
    {
        var typeObj = obj.GetElement("Type");
        if (typeObj == null || !typeObj.IsJsonString())
            return null;

        string typeName = typeObj.String();
        try
        {
            Type t = Type.GetType(typeName);

            var instance = Activator.CreateInstance(t) as BSMStateBase;
            if (instance == null)
                return null;

            instance.Load(obj);
            instance.LoadAttributes(obj);
            return instance;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static JsonObject SaveState(BSMStateBase state)
    {
        var type = state.GetType();
        string typeName = type.FullName;

        JsonObject obj = new JsonObject();
        obj.AddElement("Type", typeName);

        state.Save(obj);
        state.SaveAttributes(obj);

        return obj;
    }

    public void SetControler(BSMControler controler) { m_controler = controler; }

    public override BSMControler GetControler() { return m_controler; }

    public virtual void Init() { }

    public virtual void Update() { }

    public virtual void LateUpdate() { }

    public virtual void FixedUpdate() { }

    public virtual void UpdateAlways() { }

    public virtual void BeginUpdate() { }

    public virtual void EndUpdate() { }

    public virtual void OnDestroy() { }
}

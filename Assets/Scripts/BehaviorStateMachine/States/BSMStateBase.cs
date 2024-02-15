using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BSMStateBase : BSMAttributeHolder
{
    protected BSMControler m_controler;

    public static string GetName(Type type)
    {
        const string startString = "BSMState";

        string name = type.Name;
        if (name.StartsWith(startString))
            name = name.Substring(startString.Length);

        return name;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class BSMActionBase : BSMAttributeHolder
{
    protected BSMControler m_controler;

    public abstract void Exec();

    public void SetControler(BSMControler controler) { m_controler = controler; }

    public override BSMControler GetControler() { return m_controler; }

    public static string GetName(Type type)
    {
        const string startString = "BSMAction";

        string name = type.Name;
        if (name.StartsWith(startString))
            name = name.Substring(startString.Length);

        return name;
    }

    public virtual void Init() { }

    public virtual void OnDestroy() { }
}

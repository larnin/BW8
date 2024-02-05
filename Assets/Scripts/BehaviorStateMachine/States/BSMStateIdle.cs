using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMStateIdle : BSMStateBase
{
    [SerializeField] string m_animation = "Idle";
    [SerializeField] bool m_isOrientable = true;

    Rigidbody2D m_rigdbody;

    public string animation { get { return m_animation; } set { m_animation = value; } }
    public bool isOrientable { get { return m_isOrientable; } set { m_isOrientable = value; } }

    public override void Load(JsonObject obj)
    {
        var animElt = obj.GetElement("Anim");
        if (animElt != null && animElt.IsJsonString())
            m_animation = animElt.String();

        var orientElt = obj.GetElement("Orientable");
        if (orientElt != null && orientElt.IsJsonNumber())
            m_isOrientable = orientElt.Int() != 0;
    }

    public override void Save(JsonObject obj)
    {
        obj.AddElement("Anim", m_animation);
        obj.AddElement("Orientable", m_isOrientable ? 1 : 0);
    }

    public override void Init()
    {
        m_rigdbody = m_controler.GetComponent<Rigidbody2D>();
    }

    public override void BeginUpdate()
    {
        Event<StopMoveEvent>.Broadcast(new StopMoveEvent(), m_controler.gameObject);
    }

    public override void FixedUpdate()
    {

    }

}

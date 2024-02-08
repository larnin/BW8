using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMStateIdle : BSMStateBase
{
    public override void Load(JsonObject obj) { }

    public override void Save(JsonObject obj) { }

    public override void BeginUpdate()
    {
        Event<StopMoveEvent>.Broadcast(new StopMoveEvent(), m_controler.gameObject);
    }
}

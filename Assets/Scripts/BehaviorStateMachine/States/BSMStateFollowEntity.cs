using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMStateFollowEntity : BSMStateBase
{
    static string targetName = "Target";
    static string idleTimeName = "MaxIdleTime";

    bool m_moving = true;
    float m_idleTime = 0;

    public BSMStateFollowEntity()
    {
        AddAttribute(targetName, new BSMAttributeObject((GameObject)null));
        AddAttribute(idleTimeName, new BSMAttributeObject(-1.0f));
    }

    public override void Load(JsonObject obj) { }

    public override void Save(JsonObject obj) { }

    public override void BeginUpdate()
    {
        m_idleTime = 0;

        var target = GetGameObjectAttribute(targetName);
        if (target != null)
        {
            m_moving = true;
            Event<StartFollowEvent>.Broadcast(new StartFollowEvent(target), m_controler.gameObject);
        }
        else
        {
            m_moving = false;
            Event<StopMoveEvent>.Broadcast(new StopMoveEvent(), m_controler.gameObject);
            Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
        }
    }

    public override void EndUpdate()
    {
        Event<StopMoveEvent>.Broadcast(new StopMoveEvent(), m_controler.gameObject);
    }

    public override void Update()
    {
        if(m_moving)
        {
            var target = GetGameObjectAttribute(targetName);
            if(target == null)
            {
                Event<StopMoveEvent>.Broadcast(new StopMoveEvent(), m_controler.gameObject);
                Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
                m_idleTime = 0;
                m_moving = false;
            }
            else
            {
                IsMovingEvent movingData = new IsMovingEvent();
                Event<IsMovingEvent>.Broadcast(movingData, m_controler.gameObject);

                if (!movingData.isMoving)
                {
                    Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
                    m_idleTime = 0;
                    m_moving = false;
                }
            }
        }
        else
        {
            float idleMaxTime = GetFloatAttribute(idleTimeName, -1);

            m_idleTime += Time.deltaTime;

            if(idleMaxTime >= 0 && m_idleTime > idleMaxTime)
            {
                var target = GetGameObjectAttribute(targetName);
                
                if(target != null)
                {
                    m_moving = true;
                    Event<StartFollowEvent>.Broadcast(new StartFollowEvent(target), m_controler.gameObject);
                }
                else
                {
                    Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
                    m_idleTime = 0;
                    m_moving = false;
                }
            }
        }
    }
}

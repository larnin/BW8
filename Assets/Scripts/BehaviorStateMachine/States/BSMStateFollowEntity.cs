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

    static string endJumpActionName = "EndJump";

    bool m_moving = true;
    float m_idleTime = 0;

    SubscriberList m_subscriberList = new SubscriberList();

    public BSMStateFollowEntity()
    {
        AddAttribute(targetName, BSMAttributeObject.CreateUnityObject((GameObject)null));
        AddAttribute(idleTimeName, new BSMAttributeObject(-1.0f));

        AddActionName(endJumpActionName);
    }

    public override void Init()
    {
        if (m_subscriberList == null)
            m_subscriberList = new SubscriberList();

        m_subscriberList.Add(new Event<MoveEndJumpEvent>.LocalSubscriber(OnEndJump, GetControler().gameObject));
    }

    public override void OnBeginUpdate()
    {
        m_idleTime = 0;

        var target = GetUnityObjectAttribute<GameObject>(targetName);
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

        m_subscriberList.Subscribe();
    }

    public override void OnEndUpdate()
    {
        Event<StopMoveEvent>.Broadcast(new StopMoveEvent(), m_controler.gameObject);

        m_subscriberList.Unsubscribe();
    }

    public override void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    public override void Update()
    {
        if(m_moving)
        {
            var target = GetUnityObjectAttribute<GameObject>(targetName);
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
                var target = GetUnityObjectAttribute<GameObject>(targetName);
                
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

    void OnEndJump(MoveEndJumpEvent e)
    {
        TriggerActions(endJumpActionName);
    }
}

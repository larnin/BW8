using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMStateMoveToPosition : BSMStateBase
{
    static string posName = "Position";

    static string endJumpActionName = "EndJump";

    SubscriberList m_subscriberList = new SubscriberList();

    public BSMStateMoveToPosition()
    {
        AddAttribute(posName, new BSMAttributeObject(Vector3.zero));

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
        var target = GetVector3Attribute(posName, Vector3.zero);

        Event<StartMoveEvent>.Broadcast(new StartMoveEvent(target), m_controler.gameObject);

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
        IsMovingEvent movingData = new IsMovingEvent();
        Event<IsMovingEvent>.Broadcast(movingData, m_controler.gameObject);

        if(!movingData.isMoving)
            Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
    }

    void OnEndJump(MoveEndJumpEvent e)
    {
        TriggerActions(endJumpActionName);
    }
}

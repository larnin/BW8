using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMStateFollowPath : BSMStateBase
{
    static string pathName = "Path";
    static string validationDistanceName = "ValidationDistance";
    static string deviationName = "Deviation";
    static string nextDistanceName = "NextDistance";

    static string endJumpActionName = "EndJump";

    Vector2 m_nextPoint;
    float m_idleTime;

    SubscriberList m_subscriberList = new SubscriberList();

    public BSMStateFollowPath()
    {
        AddAttribute(pathName, new BSMAttributeObject(""));
        AddAttribute(validationDistanceName, new BSMAttributeObject(1.0f));
        AddAttribute(deviationName, new BSMAttributeObject(0.0f));
        AddAttribute(nextDistanceName, new BSMAttributeObject(1.0f));

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
        m_nextPoint = new Vector2(float.MaxValue, float.MaxValue);

        GetNextPoint();

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
        IsMovingEvent moving = new IsMovingEvent();
        Event<IsMovingEvent>.Broadcast(moving, m_controler.gameObject);
        if (!moving.isMoving)
        {
            if(m_idleTime <= 0)
                GetNextPoint();
        }
        else
        {
            float pointValidationDistance = GetFloatAttribute(validationDistanceName, 1);

            Vector2 pos = m_controler.transform.position;

            float dist = (pos - m_nextPoint).magnitude;
            if (dist < pointValidationDistance)
                GetNextPoint();

            Vector3 next = new Vector3(m_nextPoint.x, m_nextPoint.y, m_controler.transform.position.z);
        }

        m_idleTime -= Time.deltaTime;
    }

    Path GetPath()
    {
        string name = GetStringAttribute(pathName, "");

        return Path.GetPath(name);
    }

    void GetNextPoint()
    {
        Vector2 oldPoint = m_nextPoint;

        Path p = GetPath();
        if (p == null)
        {
            m_nextPoint = m_controler.transform.position;
            m_idleTime = 1.0f;
            return;
        }

        float nextDistance = GetFloatAttribute(nextDistanceName, 1);
        float deviation = GetFloatAttribute(deviationName, 0);

        Vector2 nextPos = p.GetNextTarget(m_controler.transform.position, nextDistance, deviation);

        if ((oldPoint - nextPos).sqrMagnitude < 0.01f)
        {
            Event<StopMoveEvent>.Broadcast(new StopMoveEvent(), m_controler.gameObject);
            Event<BSMStateEndedEvent>.Broadcast(new BSMStateEndedEvent(), m_controler.gameObject);
            m_idleTime = 1.0f;
        }
        else Event<StartMoveEvent>.Broadcast(new StartMoveEvent(new Vector3(nextPos.x, nextPos.y, m_controler.transform.position.z)), m_controler.gameObject);

        m_nextPoint = nextPos;
    }

    void OnEndJump(MoveEndJumpEvent e)
    {
        TriggerActions(endJumpActionName);
    }
}

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    [SerializeField] string m_idleAnim = "Idle";
    [SerializeField] bool m_idleUseDirections = true;
    [SerializeField] bool m_seeDownWhenIdle = false;
    [SerializeField] [ShowIf("m_seeDownWhenIdle")] float m_seeDownAfterDelay = 0;
    [SerializeField] string m_moveAnim = "Move";
    [SerializeField] bool m_moveUseDirection = true;
    [SerializeField] float m_moveSpeed = 1;
    [SerializeField] float m_turnSpeed = 10;
    [SerializeField] float m_targetStopMoveDistance = 1;
    [SerializeField] float m_targetStartMoveDistance = 2;

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<StartMoveEvent>.LocalSubscriber(StartMove, gameObject));
        m_subscriberList.Add(new Event<StartFollowEvent>.LocalSubscriber(StartFollow, gameObject));
        m_subscriberList.Add(new Event<StopMoveEvent>.LocalSubscriber(StopMove, gameObject));
        m_subscriberList.Add(new Event<IsMovingEvent>.LocalSubscriber(IsMoving, gameObject));

        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    private void Update()
    {
        
    }

    void StartMove(StartMoveEvent e)
    {

    }

    void StartFollow(StartFollowEvent e)
    {

    }

    void StopMove(StopMoveEvent e)
    {

    }

    void IsMoving(IsMovingEvent e)
    {

    }
}
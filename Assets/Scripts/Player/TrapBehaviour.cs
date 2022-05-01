using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TrapBehaviour : MonoBehaviour
{
    [SerializeField] LayerMask m_triggerMask;
    [SerializeField] bool m_triggerOnContact = false;
    [HideIf("m_triggerOnContact")]
    [SerializeField] float m_delay = 5;
    [SerializeField] float m_upDelay = 2;
    [SerializeField] float m_upDuration = 1;
    [SerializeField] int m_damage = 1;
    [SerializeField] float m_knockback = 1;

    enum State
    {
        waiting,
        beforeUp,
        Up,
    }

    float m_time = 0;
    State m_state = State.waiting;
    List<GameObject> m_hitObj = new List<GameObject>();

    private void Update()
    {
        m_time += Time.deltaTime;

        switch(m_state)
        {
            case State.waiting:
                if (!m_triggerOnContact && m_time > m_delay)
                    ChangeState(State.beforeUp);
                break;
            case State.beforeUp:
                if (m_time > m_upDelay)
                    ChangeState(State.Up);
                break;
            case State.Up:
                if (m_time > m_upDuration)
                    ChangeState(State.waiting);
                break;
        }
    }

    void ChangeState(State newState)
    {
        const string waitingAnim = "Waiting";
        const string pendingAnim = "Pending";
        const string goUpAnim = "GoUp";
        const string upAnim = "Up";
        const string goDownAnim = "GoDown";

        switch(newState)
        {
            case State.waiting:
                m_hitObj.Clear();
                Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(goDownAnim, false), gameObject, true);
                Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(waitingAnim, true, true), gameObject, true);
                break;
            case State.beforeUp:
                Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(pendingAnim, true), gameObject, true);
                break;
            case State.Up:
                Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(goUpAnim, false), gameObject, true);
                Event<PlayAnimationEvent>.Broadcast(new PlayAnimationEvent(upAnim, true, true), gameObject, true);
                break;
        }

        m_state = newState;

        m_time = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((m_triggerMask & (1 << collision.gameObject.layer)) == 0)
            return;

        if (m_state == State.Up)
            Damage(collision.gameObject);

        if (m_state == State.waiting && m_triggerOnContact)
            ChangeState(State.beforeUp);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (m_state != State.Up)
            return;

        if ((m_triggerMask & (1 << collision.gameObject.layer)) == 0)
            return;

        Damage(collision.gameObject);
    }

    void Damage(GameObject obj)
    {
        if (m_hitObj.Contains(obj))
            return;

        m_hitObj.Add(obj);
        Event<HitEvent>.Broadcast(new HitEvent(m_damage, gameObject, m_knockback), obj);
    }
}

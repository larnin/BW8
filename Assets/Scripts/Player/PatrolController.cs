using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PatrolController : MonoBehaviour
{
    [SerializeField] string m_pathName;
    [SerializeField] float m_pointValidationDistance = 0.5f;
    [SerializeField] float m_deviation = 0.2f;
    [SerializeField] float m_nextDistance = 2;
    
    Vector2 m_nextPoint;

    private void OnEnable()
    {
        GetNextPoint();
    }

    void GetNextPoint()
    {
        Path p = Path.GetPath(m_pathName);
        if(p == null)
        {
            m_nextPoint = transform.position;
            return;
        }

        Vector2 nextPos = p.GetNextTarget(transform.position, m_nextDistance, m_deviation);
        
        Event<StartMoveEvent>.Broadcast(new StartMoveEvent(new Vector3(nextPos.x, nextPos.y, transform.position.z)), gameObject);
        m_nextPoint = nextPos;
    }

    private void Update()
    {
        IsMovingEvent moving = new IsMovingEvent();
        Event<IsMovingEvent>.Broadcast(moving, gameObject);
        if (!moving.isMoving)
            GetNextPoint();
        else
        {
            Vector2 pos = transform.position;

            float dist = (pos - m_nextPoint).magnitude;
            if (dist < m_pointValidationDistance)
                GetNextPoint();
            
            Vector3 next = new Vector3(m_nextPoint.x, m_nextPoint.y, transform.position.z);
        }
    }
}
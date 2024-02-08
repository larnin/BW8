using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

enum Team
{
    Player,
    Ennemy,
}

public class Aggro : MonoBehaviour
{
    [SerializeField] Team m_currentTeam = Team.Ennemy;

    [SerializeField] float m_radius360 = 2;
    [SerializeField] float m_radiusSee = 3;
    [SerializeField] float m_seeAngle = 90;

    [SerializeField] float m_detectionTime = 1;
    [SerializeField] float m_forgetTime = 5;

    class OneTarget
    {
        public GameObject target;
        public float distance;
        public float angle;
        public float duration;
        public bool see;
        public bool newSee;

        public OneTarget(GameObject _target)
        {
            target = _target;
            distance = float.MaxValue;
            angle = 0;
            duration = 0;
            see = false;
            newSee = true;
        }
    }

    List<OneTarget> m_targets = new List<OneTarget>();
    int m_currentTarget = -1;

    private void Update()
    {
        PopulateList();
        UpdateTimersAndVisibility();
        ForgetOldTargets();
        UpdateCurrentTarget();
    }

    void PopulateList()
    {
        var commonData = World.common;
        LayerMask mask = m_currentTeam == Team.Player ? commonData.ennemyLayer : commonData.playerLayer;
        float biggestCircle = m_radius360 > m_radiusSee ? m_radius360 : m_radiusSee;

        var colliders = Physics2D.OverlapCircleAll(transform.position, biggestCircle, mask);

        foreach(var target in m_targets)
            target.newSee = false;

        foreach (var col in colliders)
        {
            var obj = col.gameObject;

            bool found = false;
            foreach (var target in m_targets)
            {
                if (target.target == obj)
                {
                    found = true;
                    target.newSee = true;
                }
            }

            if(!found)
            {
                var newTarget = new OneTarget(obj);
                m_targets.Add(newTarget);
            }
        }
    }

    void UpdateTimersAndVisibility()
    {
        Vector3 pos = transform.position;
        Vector2 see = GetSeeDirection();

        foreach(var target in m_targets)
        {
            if (target.target == null)
                continue;

            Vector3 targetPos = target.target.transform.position;
            target.distance = (targetPos - pos).magnitude;

            if (Mathf.Abs(see.x) < 0.1f && Mathf.Abs(see.y) < 0.1f)
                target.angle = 0;
            else
            {
                Vector2 dir = (targetPos - pos) / target.distance;

                target.angle = Vector2.Angle(see, dir) * 2;
            }

            if (target.angle > m_seeAngle && target.distance > m_radius360)
                target.newSee = false;
            if (target.distance > m_radius360 && target.distance > m_radiusSee)
                target.newSee = false;

            if(target.see != target.newSee)
            {
                if (target.newSee)
                    target.duration = m_detectionTime;
                else if (target.duration >= m_detectionTime)
                    target.duration = 0;
                else target.duration = m_forgetTime;
            }
            target.duration += Time.deltaTime;
            target.see = target.newSee;
            target.newSee = false;
        }
    }

    void ForgetOldTargets()
    {
        for(int i = 0; i < m_targets.Count; i++)
        {
            var target = m_targets[i];
            bool forget = false;

            if (target.target == null)
                forget = true;
            else if (!target.see && target.duration >= m_forgetTime)
                forget = true;
            else
            {
                GetLifeEvent e = new GetLifeEvent(1, 1);
                Event<GetLifeEvent>.Broadcast(e, target.target);
                if (e.life <= 0)
                    forget = true;
            }

            if (forget)
            {
                m_targets.RemoveAt(i);
                i--;
            }
        }
    }

    void UpdateCurrentTarget()
    {
        int newTarget = -1;

        for(int i = 0; i < m_targets.Count; i++)
        {
            var target = m_targets[i];
            if (target.see && target.duration < m_detectionTime)
                continue;

            if(newTarget >= 0)
            {
                if (target.distance > m_targets[newTarget].distance)
                    continue;
            }

            newTarget = i;
        }

        if(newTarget != m_currentTarget)
        {
            GameObject target = null;
            if (newTarget >= 0)
                target = m_targets[newTarget].target;

            Event<UpdateAggroTargetEvent>.Broadcast(new UpdateAggroTargetEvent(target), gameObject);

            m_currentTarget = newTarget;
        }
    }

    Vector2 GetSeeDirection()
    {
        return Vector2.zero;
    }
}

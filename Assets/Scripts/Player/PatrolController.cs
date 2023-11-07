using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PatrolController : MonoBehaviour
{
    [SerializeField] string m_pathName;
    [SerializeField] float m_pointValidationDistance = 0.2f;

    float m_currentIndex;

    Vector2 m_nextPoint;
    float m_nextPointIndex;

    private void OnEnable()
    {
        GetCurrentIndex();
        GetNextPoint();
    }

    void GetCurrentIndex()
    {
        Vector3 pos = transform.position;
        Path p = Path.GetPath(m_pathName);
        if (p == null)
            return;

        m_currentIndex = p.GetProgress(pos);
    }

    void GetNextPoint()
    {

    }

    private void Update()
    {
        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] [HideInInspector] bool m_bInit = false;
    [SerializeField] List<Vector2> m_points;

    private void OnValidate()
    {
        if(!m_bInit)
        {
            InitPoints();
        }
    }

    void InitPoints()
    {
        if (m_points == null)
            m_points = new List<Vector2>();
        m_points.Clear();

        m_points.Add(new Vector2(-0.5f, -0.5f));
        m_points.Add(new Vector2(-0.5f, 0.5f));
        m_points.Add(new Vector2(0.5f, -0.5f));
        m_points.Add(new Vector2(0.5f, 0.5f));
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class PathPoint
{
    public Vector2 position;
    public float width;

    public PathPoint(Vector2 pos, float w)
    {
        position = pos;
        width = w;
    }
}

public class Path : MonoBehaviour
{
    [SerializeField] [HideInInspector] bool m_init = false;
    [SerializeField] bool m_loop = true;
    [SerializeField] List<PathPoint> m_points;

    private void OnValidate()
    {
        if(!m_init)
        {
            InitPoints();
            m_init = true;
        }
    }

    void InitPoints()
    {
        if (m_points == null)
            m_points = new List<PathPoint>();
        m_points.Clear();

        m_points.Add(new PathPoint(new Vector2(-0.5f, -0.5f), 0.1f));
        m_points.Add(new PathPoint(new Vector2(-0.5f, 0.5f), 0.1f));
        m_points.Add(new PathPoint(new Vector2(0.5f, 0.5f), 0.1f));
        m_points.Add(new PathPoint(new Vector2(0.5f, -0.5f), 0.1f));
    }

    public int GetPointNb() { return m_points.Count; }

    public void AddPoint(Vector3 pos, float width = 0.1f)
    {
        Vector2 localPos = ToLocal(pos);

        m_points.Add(new PathPoint(localPos, width));
    }

    public void RemovePoint(int index)
    {
        if (index < 0 || index >= m_points.Count)
            return;

        m_points.RemoveAt(index);
    }

    public Vector3 GetPointPos(int index)
    {
        if (index < 0 || index >= m_points.Count)
            return transform.position;

        return ToGlobal(m_points[index].position);
    }

    public void SetPointPos(int index, Vector3 pos)
    {
        if (index < 0 || index >= m_points.Count)
            return;

        m_points[index].position = ToLocal(pos);
    }

    public float GetPointWidth(int index)
    {
        if (index < 0 || index >= m_points.Count)
            return 0;

        return m_points[index].width;
    }

    public void SetPointWidth(int index, float width)
    {
        if (index < 0 || index >= m_points.Count)
            return;

        m_points[index].width = width;
    }

    public Vector3 GetPointOrthoDir(int index)
    {
        if (index < 0 || index >= m_points.Count)
            return Vector3.up;

        if (m_points.Count <= 1)
            return Vector3.up;

        int lastPointIndex = index == 0 ? m_points.Count - 1 : index - 1;
        int nextPointIndex = index == m_points.Count - 1 ? 0 : index + 1;

        if(!m_loop)
        {
            if (index == 0)
                lastPointIndex = 1;
            if (index == m_points.Count - 1)
                nextPointIndex = index - 1;
        }

        Vector2 pos = m_points[index].position;
        Vector2 lastPos = m_points[lastPointIndex].position;
        Vector2 nextPos = m_points[nextPointIndex].position;

        if(lastPointIndex == nextPointIndex)
        {
            if (index == 0)
            {
                Vector2 offset = pos - nextPos;
                lastPos = pos + offset;
            }
            else
            {
                Vector2 offset = pos - lastPos;
                nextPos = pos + offset;
            }
        }

        Vector2 dir = (lastPos - pos).normalized + (pos - nextPos).normalized;

        Vector3 orthoDir = new Vector3(-dir.y, dir.x, 0);
        float norm = orthoDir.magnitude;
        if (norm > 0.001f)
            orthoDir /= norm;

        return orthoDir;
    }

    public bool IsLoop() { return m_loop; }

    Vector2 ToLocal(Vector3 pos)
    {
        pos -= transform.position;
        return new Vector2(pos.x, pos.y);
    }

    Vector3 ToGlobal(Vector2 pos)
    {
        Vector3 globalPos = transform.position;
        globalPos.x += pos.x;
        globalPos.y += pos.y;

        return globalPos;
    }
}

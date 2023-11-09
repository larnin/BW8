using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using NRand;

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
    [SerializeField] string m_name;

    static List<Path> m_paths = new List<Path>();

    public static Path GetPath(string name)
    {
        foreach (var p in m_paths)
        {
            if (p == null)
                continue;

            if (p.GetName() == name)
                return p;
        }

        return null;
    }

    public static List<Path> GetPaths(string name)
    {
        List<Path> paths = new List<Path>();
        foreach(var p in m_paths)
        {
            if (p == null)
                continue;

            if (p.GetName() == name)
                paths.Add(p);
        }

        return paths;
    }

    private void Awake()
    {
        m_paths.Add(this);
    }

    private void OnDestroy()
    {
        m_paths.Remove(this);
    }

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

    public void AddPoint(Vector2 pos, float width = 0.1f)
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

    public Vector2 GetPointPos(int index)
    {
        if (index < 0 || index >= m_points.Count)
            return transform.position;

        return ToGlobal(m_points[index].position);
    }

    public void SetPointPos(int index, Vector2 pos)
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

    public Vector2 GetPointOrthoDir(int index)
    {
        if (index < 0 || index >= m_points.Count)
            return Vector2.up;

        if (m_points.Count <= 1)
            return Vector2.up;

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

        Vector2 orthoDir = new Vector2(-dir.y, dir.x);
        float norm = orthoDir.magnitude;
        if (norm > 0.001f)
            orthoDir /= norm;

        return orthoDir;
    }

    public bool IsLoop() { return m_loop; }

    Vector2 ToLocal(Vector2 pos)
    {
        pos -= new Vector2(transform.position.x, transform.position.y);
        return pos;
    }

    Vector2 ToGlobal(Vector2 pos)
    {
        Vector2 globalPos = transform.position;
        globalPos.x += pos.x;
        globalPos.y += pos.y;

        return globalPos;
    }

    public string GetName() { return m_name; }

    public int GetNextPointIndex(Vector2 pos, bool forward)
    {
        float progress = GetProgress(pos);

        if (forward)
            return Mathf.FloorToInt(progress);
        return Mathf.CeilToInt(progress);
    }
    
    // return the normalised index along the path 
    // [0-1] = between the first and the second point
    public float GetProgress(Vector2 pos)
    {
        int nbSegments = m_loop ? m_points.Count : m_points.Count - 1;
        //first check is on pathArea
        for(int i = 0; i < nbSegments; i++)
        {
            Vector2 A, B, C, D;
            if (!GetSegmentParameters(i, out A, out B, out C, out D))
                continue;

            if (!IsOnQuadrilateralShape(pos, A, B, C, D))
                continue;

            float offset = GetProjectionOnQuadrilateralShape(pos, A, B, C, D);
            if (offset >= 0 && offset <= 1)
                return offset + i;
        }

        float bestDistance = float.MaxValue;
        float bestIndex = 0;

        for (int i = 0; i < nbSegments; i++)
        {
            int j = i == m_points.Count - 1 ? 0 : i + 1;

            Vector2 A = GetPointPos(i);
            Vector2 B = GetPointPos(j);

            float f = Mathf.Clamp01(GetProjectionOnSegment(pos, A, B));
            Vector2 p = A * f + B * (1 - f);

            float dist = (p - pos).sqrMagnitude;
            if(dist < bestDistance)
            {
                bestDistance = dist;
                bestIndex = f + i;
            }    
        }

        return bestIndex;
    }

    public bool IsOnShape(Vector2 pos)
    {
        int nbSegments = m_loop ? m_points.Count : m_points.Count - 1;
        for (int i = 0; i < nbSegments; i++)
        {
            Vector2 A, B, C, D;
            if (!GetSegmentParameters(i, out A, out B, out C, out D))
                continue;

            if (IsOnQuadrilateralShape(pos, A, B, C, D))
                return true;
        }

        return false;
    }

    bool GetSegmentParameters(int index, out Vector2 A, out Vector2 B, out Vector2 C, out Vector2 D, bool forceGet = false)
    {
        int i = index;

        int j = i == m_points.Count - 1 ? 0 : i + 1;

        float wI = m_points[i].width;
        float wJ = m_points[j].width;

        if (wI < 0.01f && wJ < 0.01f && !forceGet)
        {
            A = Vector2.zero;
            B = Vector2.zero;
            C = Vector2.zero;
            D = Vector2.zero;
            return false;
        }

        Vector2 dirI = GetPointOrthoDir(i);
        Vector2 dirJ = GetPointOrthoDir(j);

        Vector2 posI = GetPointPos(i);
        Vector2 posJ = GetPointPos(j);

        A = posJ + wJ * dirJ;
        D = posJ - wJ * dirJ;
        B = posI + wI * dirI;
        C = posI - wI * dirI;

        return true;
    }

    static float GetProjectionOnSegment(Vector2 pos, Vector2 p1, Vector2 p2)
    {
        var projectPoint = GetProjectionPointOnLine(pos, p1, p2);

        float pointDist = (projectPoint - p1).magnitude;
        float totalDist = (p1 - p2).magnitude;

        float dot = Vector2.Dot(projectPoint - p1, p2 - p1);

        if (dot > 0.5f)
            return pointDist / totalDist;
        return -pointDist / totalDist;
    }

    static Vector2 GetProjectionPointOnLine(Vector2 pos, Vector2 p1, Vector2 p2)
    {
        Vector2 p1p2 = p2 - p1;
        Vector2 p1Pos = pos - p1;

        float norm = p1p2.magnitude;

        float dist = Vector2.Dot(p1Pos, p1p2) / norm;

        return p1 + p1p2 / norm * dist;
    }

    static bool IsOnQuadrilateralShape(Vector2 P, Vector2 A, Vector2 B, Vector2 C,Vector2 D)
    {
        return PointOnTriangle(P, A, B, C) || PointOnTriangle(P, A, C, D);
    }

    static bool PointOnTriangle(Vector2 p, Vector2 A, Vector2 B, Vector2 C)
    {
        //https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle

        var s = (A.x - C.x) * (p.y - C.y) - (A.y - C.y) * (p.x - C.x);
        var t = (B.x - A.x) * (p.y - A.y) - (B.y - A.y) * (p.x - A.x);

        if ((s < 0) != (t < 0) && s != 0 && t != 0)
            return false;

        var d = (C.x - B.x) * (p.y - B.y) - (C.y - B.y) * (p.x - B.x);
        return d == 0 || (d < 0) == (s + t <= 0);
    }

    static float GetProjectionOnQuadrilateralShape(Vector2 P, Vector2 A, Vector2 B, Vector2 C, Vector2 D)
    {
        //https://cdn.discordapp.com/attachments/434331479607083008/1169618852217376921/image.png?ex=65560f70&is=65439a70&hm=53591c0bd318495392f3efe365f6602a5ca22f6a44d8cec8ccdaab0d64c73cbf&

        float tMin = 0;
        float tMax = 1;

        const float resolution = 1 / 16.0f;

        float len1 = (A - B).magnitude;
        float len2 = (C - D).magnitude;
        float len = Mathf.Max(len1, len2);
        float nbSegment = len / resolution;
        int nbLoop = Mathf.CeilToInt(Mathf.Log(nbSegment) / Mathf.Log(2));
        if (nbLoop < 4)
            nbLoop = 4;

        for(int i = 0; i < nbLoop; i++)
        {
            float t = (tMax - tMin) / 2 + tMin;

            Vector2 P3 = D * t + C * (1 - t);
            Vector2 P4 = A * t + B * (1 - t);
            
            if (Utility.IsRight(P, P3, P4))
                tMax = t;
            else tMin = t;
        }

        float tEnd = (tMax - tMin) / 2 + tMin;
        return tEnd;
    }

    public Vector2 GetNextTarget(Vector2 pos, float distance, float deviation)
    {
        float current = GetProgress(pos);

        int nextIndex = Mathf.CeilToInt(current);

        if (nextIndex - current < 0.01f && !m_loop && nextIndex == m_points.Count)
            return pos;

        if (nextIndex >= m_points.Count)
            nextIndex = 0;

        int lastIndex = Mathf.FloorToInt(current);

        Vector2 lastPos = GetPointPos(lastIndex);
        Vector2 nextPos = GetPointPos(nextIndex);

        float part = current - lastIndex;

        Vector2 A, B, C, D;
        GetSegmentParameters(lastIndex, out A, out B, out C, out D, true);
        Vector2 p4 = A * part + B * (1 - part);
        Vector2 p3 = D * part + C * (1 - part);
        float pPercent = 0.5f;
        float p3p4 = (p3 - p4).magnitude;
        if(p3p4 > 0.01f)
            pPercent = (p3 - pos).magnitude / p3p4;
        pPercent = Mathf.Clamp01(pPercent);

        float dist = (lastPos - nextPos).magnitude;

        float distToNext = dist * (1 - part);

        if(distance > distToNext)
        {
            distance -= distToNext;
            if (distance < 0.01f)
                return CalculateDeviation(nextIndex, pPercent, deviation);

            int afterIndex = nextIndex + 1;
            if(afterIndex >= m_paths.Count)
            {
                if (!m_loop)
                    return CalculateDeviation(nextIndex, pPercent, deviation);
            }
            
            Vector2 evaluatedPosWithDeviation = CalculateDeviation(nextIndex + 0.01f, pPercent, 0);
            return GetNextTarget(evaluatedPosWithDeviation, distance, deviation);
        }

        float nextComputedIndex = current + distance / dist;
        return CalculateDeviation(nextComputedIndex, pPercent, deviation);
    }

    Vector2 CalculateDeviation(float index, float currentPercent, float deviation)
    {
        int lastIndex = Mathf.FloorToInt(index);
        float part = index - lastIndex;

        Vector2 A, B, C, D;
        GetSegmentParameters(lastIndex, out A, out B, out C, out D, true);
        Vector2 p4 = A * part + B * (1 - part);
        Vector2 p3 = D * part + C * (1 - part);

        float dist = (p4 - p3).magnitude;

        float min = currentPercent * dist - deviation;
        if (min < 0)
            min = 0;
        float max = currentPercent * dist + deviation;
        if (max > dist)
            max = dist;

        float randValue = new UniformFloatDistribution(0, 1).Next(new StaticRandomGenerator<MT19937>());
        float percent = (max * randValue + min * (1 - randValue)) / dist;

        return p3 * (percent) + p4 * (1 - percent);
    }
}

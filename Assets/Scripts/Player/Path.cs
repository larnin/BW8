using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public string GetName() { return m_name; }

    public int GetNextPointIndex(Vector3 pos, bool forward)
    {
        float progress = GetProgress(pos);

        if (forward)
            return Mathf.FloorToInt(progress);
        return Mathf.CeilToInt(progress);
    }
    
    // return the normalised index along the path 
    // [0-1] = between the first and the second point
    public float GetProgress(Vector3 pos)
    {
        int nbSegments = m_loop ? m_points.Count : m_points.Count - 1;
        //first check is on pathArea
        for(int i = 0; i < nbSegments; i++)
        {
            Vector3 A, B, C, D;
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

            Vector3 A = GetPointPos(i);
            Vector3 B = GetPointPos(j);

            float f = Mathf.Clamp01(GetProjectionOnSegment(pos, A, B));
            Vector3 p = A * f + B * (1 - f);

            float dist = (p - pos).sqrMagnitude;
            if(dist < bestDistance)
            {
                bestDistance = dist;
                bestIndex = f + i;
            }    
        }

        return bestIndex;
    }

    public bool IsOnShape(Vector3 pos)
    {
        int nbSegments = m_loop ? m_points.Count : m_points.Count - 1;
        for (int i = 0; i < nbSegments; i++)
        {
            Vector3 A, B, C, D;
            if (!GetSegmentParameters(i, out A, out B, out C, out D))
                continue;

            if (IsOnQuadrilateralShape(pos, A, B, C, D))
                return true;
        }

        return false;
    }

    bool GetSegmentParameters(int index, out Vector3 A, out Vector3 B, out Vector3 C, out Vector3 D, bool forceGet = false)
    {
        int i = index;

        int j = i == m_points.Count - 1 ? 0 : i + 1;

        float wI = m_points[i].width;
        float wJ = m_points[j].width;

        if (wI < 0.01f && wJ < 0.01f && !forceGet)
        {
            A = Vector3.zero;
            B = Vector3.zero;
            C = Vector3.zero;
            D = Vector3.zero;
            return false;
        }

        Vector3 dirI = GetPointOrthoDir(i);
        Vector3 dirJ = GetPointOrthoDir(j);

        Vector3 posI = GetPointPos(i);
        Vector3 posJ = GetPointPos(j);

        A = posJ + wJ * dirJ;
        D = posJ - wJ * dirJ;
        B = posI + wI * dirI;
        C = posI - wI * dirI;

        return true;
    }

    static float GetProjectionOnSegment(Vector3 pos, Vector3 p1, Vector3 p2)
    {
        var projectPoint = GetProjectionPointOnLine(pos, p1, p2);

        float pointDist = (projectPoint - p1).magnitude;
        float totalDist = (p1 - p2).magnitude;

        float dot = Vector3.Dot(projectPoint - p1, p2 - p1);

        if (dot > 0.5f)
            return pointDist / totalDist;
        return -pointDist / totalDist;
    }

    static Vector3 GetProjectionPointOnLine(Vector3 pos, Vector3 p1, Vector3 p2)
    {
        return Vector3.Project(pos - p1, p2 - p1) + p1;
    }

    static bool IsOnQuadrilateralShape(Vector3 P, Vector3 A, Vector3 B, Vector3 C,Vector3 D)
    {
        return PointOnTriangle(P, A, B, C) || PointOnTriangle(P, A, C, D);
    }

    static bool PointOnTriangle(Vector3 p, Vector3 A, Vector3 B, Vector3 C)
    {
        // https://gamedev.stackexchange.com/questions/28781/easy-way-to-project-point-onto-triangle-or-plane
        Vector3 U = B - A;
        Vector3 V = C - A;
        Vector3 N = Vector3.Cross(U, V);

        Vector3 W = p - A;

        float gamma = Vector3.Dot(Vector3.Cross(U, W), N) / Vector3.Dot(N, N);
        float beta = Vector3.Dot(Vector3.Cross(W, V), N) / Vector3.Dot(N, N);
        float alpha = 1 - gamma - beta;

        return ((0 <= alpha) && (alpha <= 1) && (0 <= beta) && (beta <= 1) && (0 <= gamma) && (gamma <= 1));
    }

    static float GetProjectionOnQuadrilateralShape(Vector3 P, Vector3 A, Vector3 B, Vector3 C, Vector3 D)
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

            Vector3 P3 = D * t + C * (1 - t);
            Vector3 P4 = A * t + B * (1 - t);
            
            if (Utility.IsRight(P, P3, P4))
                tMax = t;
            else tMin = t;
        }

        float tEnd = (tMax - tMin) / 2 + tMin;
        return tEnd;
    }

    private void OnGUI()
    {
        var cam = Camera.main;
        if (cam == null)
            return;

        var mousePos = Mouse.current.position.ReadValue();

        var ray = cam.ScreenPointToRay(mousePos);

        var p = new Plane(transform.forward, transform.position);

        float enter = 0;
        if(p.Raycast(ray, out enter))
        {
            Vector3 point = ray.GetPoint(enter);

            float index = GetProgress(point);

            GUI.color = Color.blue;
            Vector2 screenPos = new Vector2(mousePos.x + 10, Screen.height - mousePos.y);
            GUI.Label(new Rect(screenPos, new Vector2(100, 20)), index.ToString());
        }
    }

    private void Update()
    {
        int nbPoint = IsLoop() ? GetPointNb() : GetPointNb() - 1;

        for (int i = 0; i < nbPoint; i++)
        {
            int nextIndex = i < GetPointNb() - 1 ? i + 1 : 0;

            Vector3 pos = GetPointPos(i);
            Vector3 nextPos = GetPointPos(nextIndex);

            Vector3 orthoDir = GetPointOrthoDir(i);
            Vector3 nextOrthoDir = GetPointOrthoDir(nextIndex);

            float width = GetPointWidth(i);
            float nextWidth = GetPointWidth(nextIndex);

            Debug.DrawLine(pos, nextPos, Color.yellow);

            Vector3 offPos = pos + orthoDir * width;
            Vector3 nextOffPos = nextPos + nextOrthoDir * nextWidth;
            Debug.DrawLine(offPos, nextOffPos, Color.green);

            Vector3 offPos2 = pos - orthoDir * width;
            Vector3 nextOffPos2 = nextPos - nextOrthoDir * nextWidth;
            Debug.DrawLine(offPos2, nextOffPos2, Color.green);

            Debug.DrawLine(offPos, offPos2, Color.green);
            Debug.DrawLine(nextOffPos, nextOffPos2, Color.green);
        }
    }
}

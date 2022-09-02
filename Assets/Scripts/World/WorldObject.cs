using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "World", menuName = "Game/New world")]
public class WorldObject : SerializedScriptableObject
{
    [OnValueChanged("UpdateChunkMatrix")]
    [SerializeField] Vector2Int m_chunkNb = new Vector2Int(2, 2);
    public Vector2Int chunkNb { get { return m_chunkNb; } }

    [TableMatrix(SquareCells = true)]
    [SerializeField] GameObject[,] m_chunkMatrix;

    [SerializeField] Vector2 m_chunkSize = new Vector2(1, 1);

    [SerializeField] bool m_horizontalLoop = false;
    [SerializeField] bool m_verticalLoop = false;

    public Vector2 chunkSize { get { return m_chunkSize; } }
    public bool horizontalLoop { get { return m_horizontalLoop; } }
    public bool verticalLoop { get { return m_verticalLoop; } }

    [OnInspectorInit]
    private void CreateData()
    {
        UpdateChunkMatrix();
    }

    void UpdateChunkMatrix()
    {
        if(m_chunkMatrix == null)
        {
            m_chunkMatrix = new GameObject[m_chunkNb.x, m_chunkNb.y];
            return;
        }

        int x = m_chunkMatrix.GetLength(0);
        int y = m_chunkMatrix.GetLength(1);
        if (x == m_chunkNb.x && y == m_chunkNb.y)
            return;

        var newMatrix = new GameObject[m_chunkNb.x, m_chunkNb.y];

        int minX = Mathf.Min(x, m_chunkNb.x);
        int minY = Mathf.Min(y, m_chunkNb.y);

        for(int i  = 0; i < minX; i++)
            for(int j = 0; j < minY; j++)
                newMatrix[i, j] = m_chunkMatrix[i, j];

        m_chunkMatrix = newMatrix;
    }

    public GameObject GetChunk(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= chunkNb.x)
            return null;
        if (pos.y < 0 || pos.y >= chunkNb.y)
            return null;

        return m_chunkMatrix[pos.x, pos.y];
    }

    public GameObject GetUnclampedChunk(Vector2Int pos)
    {
        var realPos = ClampPos(pos);
        return GetChunk(realPos);
    }

    Vector2Int ClampPos(Vector2Int pos)
    {
        Vector2Int outPos = Vector2Int.zero;

        if (m_horizontalLoop)
        {
            if (pos.x < 0)
                outPos.x = (pos.x % m_chunkNb.x + m_chunkNb.x) % m_chunkNb.x;
            else outPos.x = pos.x % m_chunkNb.x;
        }
        else outPos.x = pos.x;
        if (m_verticalLoop)
        {
            if (pos.y < 0)
                outPos.y = (pos.y % m_chunkNb.y + m_chunkNb.y) % m_chunkNb.y;
            else outPos.y = pos.y % m_chunkNb.y;
        }
        else outPos.y = pos.y;

        return outPos;
    }

    public bool GetSpawnPos(string name, out Vector2 outPos)
    {
        for(int i = 0; i < m_chunkNb.x; i++)
        {
            for(int j = 0; j < m_chunkNb.y; j++)
            {
                var obj = m_chunkMatrix[i, j];
                if (obj == null)
                    continue;

                var origin = obj.transform.position;

                var points = obj.GetComponentsInChildren<SpawnPoint>();
                foreach(var point in points)
                {
                    if(point.spawnName == name)
                    {
                        outPos = point.transform.position - origin;
                        return true;
                    }
                }
            }
        }

        outPos = Vector2.zero;
        return false;
    }
}
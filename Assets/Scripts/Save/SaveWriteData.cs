using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SaveWriteData
{
    const int chunkSize = 256;

    Byte[] m_data;
    int m_dataIndex = 0;
    int m_dataSize = 0;

    public int GetDataSize()
    {
        return m_dataIndex;
    }

    public Byte[] GetData()
    {
        return m_data;
    }

    public void Write(Byte b)
    {
        IncreaseSize(1);
        m_data[m_dataIndex] = b;
        m_dataIndex++;
    }

    public void Write(Byte[] bytes)
    {
        IncreaseSize(bytes.Length);
        for (int i = 0; i < bytes.Length; i++)
            m_data[m_dataIndex + i] = bytes[i];
        m_dataIndex += bytes.Length;
    }

    public void Write(string str)
    {
        Write(Encoding.ASCII.GetBytes(str));
    }

    public void Write(int value)
    {
        Write(BitConverter.GetBytes(value));
    }

    public void Write(float value)
    {
        Write(BitConverter.GetBytes(value));
    }

    void IncreaseSize(int add)
    {
        Realloc(m_dataIndex + add);
    }

    void Realloc(int wantedSize)
    {
        if (wantedSize < m_dataSize)
            return;

        int allocSize = (wantedSize / chunkSize) + 1;
        allocSize *= chunkSize;
        Byte[] data = new Byte[allocSize];

        for (int i = 0; i < m_dataIndex; i++)
            data[i] = m_data[i];

        m_data = data;
        m_dataSize = allocSize;
    }

    /// <summary>
    /// Does not reset the memory, only remove the written data
    /// </summary>
    public void Clear()
    {
        m_dataIndex = 0;
    }

    public void Reset()
    {
        m_data = null;
        m_dataIndex = 0;
        m_dataSize = 0;
    }
}
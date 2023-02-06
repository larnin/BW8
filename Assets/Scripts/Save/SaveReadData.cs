using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SaveReadData
{
    Byte[] m_data;
    int m_dataIndex = 0;
    int m_dataSize = 0;

    public void SetData(Byte[] data, int dataSize)
    {
        m_data = data;
        m_dataSize = dataSize;
        m_dataIndex = 0;
    }

    public void SetIndex(int dataIndex)
    {
        m_dataIndex = dataIndex;
        if (m_dataIndex > m_dataSize)
            m_dataIndex = m_dataSize;
    }

    public int GetIndex()
    {
        return m_dataIndex;
    }

    public int GetSize()
    {
        return m_dataSize;
    }

    public Byte ReadByte()
    {
        var bytes = ReadBytes(1);
        return bytes[0];
    }

    public Byte[] ReadBytes(int size)
    {
        var bytes = new Byte[size];

        if (m_dataIndex + size > m_dataSize)
        {
            DebugLogs.LogError("Read too many bytes !");
            return bytes;
        }

        for (int i = 0; i < size; i++)
            bytes[i] = m_data[m_dataIndex + i];

        m_dataIndex += size;

        return bytes;
    }

    public int ReadInt()
    {
        var bytes = ReadBytes(sizeof(int));
        return BitConverter.ToInt32(bytes, 0);
    }

    public float ReadFloat()
    {
        var bytes = ReadBytes(sizeof(float));
        return BitConverter.ToSingle(bytes, 0);
    }

    public string ReadString(int size)
    {
        var bytes = ReadBytes(size);
        return Encoding.ASCII.GetString(bytes);
    }
}
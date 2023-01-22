using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum SaveValueType : byte
{
    // the values must not change or the save file will be corrupted
    SaveEmpty = 0,
    SaveString = 1,
    SaveInt = 2,
    SaveFloat = 3,
    SaveVector2 = 4,
    SaveVector2Int = 5,
    SaveVector3 = 6,
    SaveVector3Int = 7,
}

public class SaveValue
{
    SaveValueType m_type = SaveValueType.SaveFloat;
    object m_value = null;

    public void Set(string value)
    {
        m_value = value;
        m_type = SaveValueType.SaveString;
    }

    public string GetString(string def = "")
    {
        if (m_type != SaveValueType.SaveString)
            return def;
        return m_value as string;
    }

    public void Set(float value)
    {
        m_value = value;
        m_type = SaveValueType.SaveFloat;
    }

    public float GetFloat(float def = 0.0f)
    {
        if (m_type != SaveValueType.SaveFloat || m_value == null)
            return def;
        return Convert.ToSingle(m_value);
    }

    public void Set(int value)
    {
        m_value = value;
        m_type = SaveValueType.SaveInt;
    }

    public int GetInt(int def = 0)
    {
        if (m_type != SaveValueType.SaveInt || m_value == null)
            return def;
        return Convert.ToInt32(m_value);
    }

    public void Set(Vector2 value)
    {
        m_value = value;
        m_type = SaveValueType.SaveVector2;
    }

    public Vector2 GetVector2() { return GetVector2(Vector2.zero); }
    public Vector2 GetVector2(Vector2 def)
    {
        if (m_type != SaveValueType.SaveVector2 || m_value == null)
            return def;
        return (m_value as Vector2?).Value;
    }

    public void Set(Vector2Int value)
    {
        m_value = value;
        m_type = SaveValueType.SaveVector2Int;
    }

    public Vector2Int GetVector2Int() { return GetVector2Int(Vector2Int.zero); }
    public Vector2Int GetVector2Int(Vector2Int def)
    {
        if (m_type != SaveValueType.SaveVector2Int || m_value == null)
            return def;
        return (m_value as Vector2Int?).Value;
    }

    public void Set(Vector3 value)
    {
        m_value = value;
        m_type = SaveValueType.SaveVector3;
    }

    public Vector3 GetVector3() { return GetVector3(Vector3.zero); }
    public Vector3 GetVector3(Vector3 def)
    {
        if (m_type != SaveValueType.SaveVector3 || m_value == null)
            return def;
        return (m_value as Vector3?).Value;
    }

    public void Set(Vector3Int value)
    {
        m_value = value;
        m_type = SaveValueType.SaveVector3Int;
    }

    public Vector3Int GetVector3Int() { return GetVector3Int(Vector3Int.zero); }
    public Vector3Int GetVector3Int(Vector3Int def)
    {
        if (m_type != SaveValueType.SaveVector3Int || m_value == null)
            return def;
        return (m_value as Vector3Int?).Value;
    }

    public SaveValueType GetValueType()
    {
        return m_type;
    }

    public void Load(SaveReadData data)
    {
        m_type = (SaveValueType)data.ReadByte();

        switch (m_type)
        {
            case SaveValueType.SaveString:
                int strSize = data.ReadInt();
                m_value = data.ReadString(strSize);
                break;
            case SaveValueType.SaveFloat:
                m_value = data.ReadFloat();
                break;
            case SaveValueType.SaveInt:
                m_value = data.ReadInt();
                break;
            case SaveValueType.SaveVector2:
                Vector2 value2 = Vector2.zero;
                value2.x = data.ReadFloat();
                value2.y = data.ReadFloat();
                m_value = value2;
                break;
            case SaveValueType.SaveVector2Int:
                Vector2 value2i = Vector2.zero;
                value2i.x = data.ReadInt();
                value2i.y = data.ReadInt();
                m_value = value2i;
                break;
            case SaveValueType.SaveVector3:
                Vector3 value3 = Vector3.zero;
                value3.x = data.ReadFloat();
                value3.y = data.ReadFloat();
                value3.z = data.ReadFloat();
                m_value = value3;
                break;
            case SaveValueType.SaveVector3Int:
                Vector3 value3i = Vector3.zero;
                value3i.x = data.ReadInt();
                value3i.y = data.ReadInt();
                value3i.z = data.ReadInt();
                m_value = value3i;
                break;
            default:
                Debug.LogError("Unknow save value type");
                break;
        }
    }

    public void Save(SaveWriteData data)
    {
        data.Write((Byte)m_type);

        switch (m_type)
        {
            case SaveValueType.SaveString:
                var str = GetString();
                data.Write(str.Length);
                data.Write(str);
                break;
            case SaveValueType.SaveFloat:
                data.Write(GetFloat());
                break;
            case SaveValueType.SaveInt:
                data.Write(GetInt());
                break;
            case SaveValueType.SaveVector2:
                var value2 = GetVector2();
                data.Write(value2.x);
                data.Write(value2.y);
                break;
            case SaveValueType.SaveVector2Int:
                var value2i = GetVector2Int();
                data.Write(value2i.x);
                data.Write(value2i.y);
                break;
            case SaveValueType.SaveVector3:
                var value3 = GetVector3();
                data.Write(value3.x);
                data.Write(value3.y);
                data.Write(value3.z);
                break;
            case SaveValueType.SaveVector3Int:
                var value3i = GetVector3Int();
                data.Write(value3i.x);
                data.Write(value3i.y);
                data.Write(value3i.z);
                break;
            default:
                Debug.LogError("Unknow save value type");
                break;
        }
    }
}
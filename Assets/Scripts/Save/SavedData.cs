using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SavedData
{
    Dictionary<string, SaveValue> m_data = new Dictionary<string, SaveValue>();

    public void Clear()
    {
        m_data.Clear();
    }

    SaveValue GetOrInsertValue(string key)
    {
        if (!m_data.ContainsKey(key))
            m_data[key] = new SaveValue();
        return m_data[key];
    }

    SaveValue GetOrNullValue(string key)
    {
        if (!m_data.ContainsKey(key))
            return null;
        return m_data[key];
    }

    public bool Remove(string key)
    {
        return m_data.Remove(key);
    }

    public bool HaveKey(string key)
    {
        return m_data.ContainsKey(key);
    }

    public SaveValueType GetValueType(string key)
    {
        if (!m_data.ContainsKey(key))
            return SaveValueType.SaveEmpty;
        return m_data[key].GetValueType();
    }

    public string[] GetKeys()
    {
        return m_data.Keys.ToArray();
    }

    public int GetNbEntry()
    {
        return m_data.Count;
    }

    public void Set(string key, string value) { GetOrInsertValue(key).Set(value); }
    public string GetString(string key, string def = "") { return GetOrNullValue(key)?.GetString(def) ?? def; }

    public void Set(string key, float value) { GetOrInsertValue(key).Set(value); }
    public float GetFloat(string key, float def = 0) { return GetOrNullValue(key)?.GetFloat(def) ?? def; }

    public void Set(string key, int value) { GetOrInsertValue(key).Set(value); }
    public int GetInt(string key, int def = 0) { return GetOrNullValue(key)?.GetInt(def) ?? def; }

    public void Set(string key, Vector2 value) { GetOrInsertValue(key).Set(value); }
    public Vector2 GetVector2(string key) { return GetVector2(key, Vector2.zero); }
    public Vector2 GetVector2(string key, Vector2 def) { return GetOrNullValue(key)?.GetVector2(def) ?? def; }

    public void Set(string key, Vector2Int value) { GetOrInsertValue(key).Set(value); }
    public Vector2Int GetVector2Int(string key) { return GetVector2Int(key, Vector2Int.zero); }
    public Vector2Int GetVector2Int(string key, Vector2Int def) { return GetOrNullValue(key)?.GetVector2Int(def) ?? def; }

    public void Set(string key, Vector3 value) { GetOrInsertValue(key).Set(value); }
    public Vector3 GetVector3(string key) { return GetVector3(key, Vector3.zero); }
    public Vector3 GetVector3(string key, Vector2 def) { return GetOrNullValue(key)?.GetVector3(def) ?? def; }

    public void Set(string key, Vector3Int value) { GetOrInsertValue(key).Set(value); }
    public Vector3Int GetVector3Int(string key) { return GetVector3Int(key, Vector3Int.zero); }
    public Vector3Int GetVector3Int(string key, Vector3Int def) { return GetOrNullValue(key)?.GetVector3Int(def) ?? def; }

    public void Load(SaveReadData data)
    {
        int nbData = data.ReadInt();
        for (int i = 0; i < nbData; i++)
            LoadOneData(data);
    }

    void LoadOneData(SaveReadData data)
    {
        int strLen = data.ReadInt();
        string str = data.ReadString(strLen);
        SaveValue value = new SaveValue();
        value.Load(data);
        m_data.Add(str, value);
    }

    public void Save(SaveWriteData data)
    {
        int nbData = m_data.Count;
        data.Write(nbData);
        foreach(var d in m_data)
        {
            data.Write(d.Key.Length);
            data.Write(d.Key);
            d.Value.Save(data);
        }
    }
}

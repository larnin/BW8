using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SaveSystem
{
    const string savePath = "/Save/";
    const string headerName = "Header";
    const string saveName = "Save";
    const string extensionName = ".save";
    const int nbSlots = 3;

    SavedData[] m_slotsHeaders;

    int m_currentSlot = -1;
    Dictionary<string, SavedData> m_slotDataGroups = new Dictionary<string, SavedData>();

    static SaveSystem m_save;
    public static SaveSystem instance
    {
        get
        {
            if (m_save == null)
                m_save = new SaveSystem();
            return m_save;
        }
    }

    public SaveSystem()
    {
        LoadHeaders();
    }

    void LoadHeaders()
    {
        if (m_slotsHeaders == null || m_slotsHeaders.Length != nbSlots)
            m_slotsHeaders = new SavedData[nbSlots];

        for (int i = 0; i < nbSlots; i++)
            LoadHeader(i);
    }

    void LoadHeader(int slot)
    {
        if (slot < 0 || slot >= nbSlots)
            return;

        string path = GetHeaderPath(slot);
        Byte[] bytes = LoadFile(path);
        if(bytes == null || bytes.Length == 0)
        {
            m_slotsHeaders[slot] = null;
            return;
        }

        SaveReadData data = new SaveReadData();
        data.SetData(bytes, bytes.Length);

        m_slotsHeaders[slot] = new SavedData();
        m_slotsHeaders[slot].Load(data);
    }

    void SaveHeader(int slot)
    {
        if (slot < 0 || slot >= nbSlots)
            return;

        string headerPath = GetHeaderPath(slot);
        if (m_slotsHeaders[slot] == null || m_slotsHeaders[slot].GetNbEntry() == 0)
        {
            DeleteFile(headerPath);
            return;
        }

        SaveWriteData data = new SaveWriteData();
        m_slotsHeaders[slot].Save(data);

        SaveFile(headerPath, data.GetData());
    }

    public void LoadSlot(int slot)
    {
        if (slot < 0 || slot >= nbSlots)
            return;

        string path = GetSlotPath(slot);
        Byte[] bytes = LoadFile(path);
        if(bytes == null || bytes.Length == 0)
        {
            m_slotDataGroups.Clear();
            return;
        }

        SaveReadData data = new SaveReadData();
        data.SetData(bytes, bytes.Length);

        m_slotDataGroups.Clear();

        int nbGroups = data.ReadInt();
        for(int i = 0; i < nbGroups; i++)
        {
            int nbChar = data.ReadInt();
            string groupName = data.ReadString(nbChar);

            SavedData groupData = new SavedData();
            groupData.Load(data);
            if(groupData.GetNbEntry() > 0)
                m_slotDataGroups.Add(groupName, groupData);
        }

        m_currentSlot = slot;
    }

    void SaveCurrentSlot()
    {
        if (m_currentSlot < 0 || m_currentSlot >= nbSlots)
            return;

        SaveWriteData data = new SaveWriteData();

        data.Write(m_slotDataGroups.Count);
        foreach(var group in m_slotDataGroups)
        {
            data.Write(group.Key.Length);
            data.Write(group.Key);
            group.Value.Save(data);
        }

        string savePath = GetSlotPath(m_currentSlot);
        SaveFile(savePath, data.GetData());
    }

    public void Save()
    {
        for (int i = 0; i < nbSlots; i++)
            SaveHeader(i);

        SaveCurrentSlot();
    }

    public int GetSlotNb()
    {
        return nbSlots;
    }

    public SavedData GetSlotHeader(int slot)
    {
        if (slot < 0 || slot >= nbSlots)
            return null;

        if (m_slotsHeaders[slot] == null)
            m_slotsHeaders[slot] = new SavedData();
        return m_slotsHeaders[slot];
    }

    public void NewSlot(int slot)
    {
        if (slot < 0 || slot >= nbSlots)
            return;

        RemoveSlot(slot);

        m_currentSlot = slot;
    }

    public void RemoveSlot(int slot)
    {
        if (slot < 0 || slot >= nbSlots)
            return;

        m_slotsHeaders[slot] = null;

        if(m_currentSlot == slot)
        {
            m_currentSlot = -1;
            m_slotDataGroups.Clear();
        }
    }

    public SavedData GetGroup(string name)
    {
        if (m_currentSlot < 0)
            return null;

        if (!m_slotDataGroups.ContainsKey(name))
            m_slotDataGroups.Add(name, new SavedData());
        return m_slotDataGroups[name];
    }

    void SaveFile(string path, Byte[] data)
    {
        try
        {
            string directory = path.Substring(0, path.LastIndexOf('/'));
            Directory.CreateDirectory(directory);

            FileStream file = File.Create(path);

            file.Write(data, 0, data.Length);

            file.Close();
        }
        catch(Exception e)
        {
            Debug.LogError("Unable to save file " + path);
            Debug.LogError(e.Message);
        }
    }

    Byte[] LoadFile(string path)
    {
        try
        {
            if (!File.Exists(path))
                return null;

            return File.ReadAllBytes(path);
        }
        catch(Exception e)
        {
            Debug.LogError("Unable to read file " + path);
            Debug.LogError(e.Message);
        }

        return null;
    }

    void DeleteFile(string path)
    {
        try
        {
            if (!File.Exists(path))
                return;
            File.Delete(path);
        }
        catch(Exception e)
        {
            Debug.LogError("Unable to remove file " + path);
            Debug.LogError(e.Message);
        }
    }

    string GetSlotPath(int slot)
    {
        return Application.persistentDataPath + savePath + saveName + slot.ToString() + extensionName;
    }   
    
    string GetHeaderPath(int slot)
    {
        return Application.persistentDataPath + savePath + headerName + slot.ToString() + extensionName;
    }
}

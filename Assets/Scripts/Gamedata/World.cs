using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class World
{
    static string s_path = "World";
    static string s_lootTypeName = "LootType";
    static string s_swordName = "Sword";
    static string s_vacuumName = "Vacuum";
    static string s_commonName = "Common";
    static string s_projectilesName = "Projectiles";

    static CommonData m_commonData;
    public static CommonData common
    {
        get
        {
            if (m_commonData == null)
                Load();
            return m_commonData;
        }
    }

    static LootType m_lootType;
    public static LootType lootType 
    { 
        get 
        {
            if (m_lootType == null)
                Load();
            return m_lootType;
        } 
    }

    static SwordData m_sword;
    public static SwordData sword
    {
        get
        {
            if (m_sword == null)
                Load();
            return m_sword;
        }
    }

    static VacuumData m_vacuum;
    public static VacuumData vacuum
    {
        get
        {
            if (m_vacuum == null)
                Load();
            return m_vacuum;
        }
    }

    static ProjectileList m_projectiles;
    public static ProjectileList projectiles
    {
        get
        {
            if (m_projectiles == null)
                Load();
            return m_projectiles;
        }
    }

#if UNITY_EDITOR
    [MenuItem("Game/Create Gamedata World")]
    public static void MakeWorld()
    {
        m_lootType = Create<LootType>(s_lootTypeName) ?? m_lootType;
        m_sword = Create<SwordData>(s_swordName) ?? m_sword;
        m_vacuum = Create<VacuumData>(s_vacuumName) ?? m_vacuum;
        m_commonData = Create<CommonData>(s_commonName) ?? m_commonData;
        m_projectiles = Create<ProjectileList>(s_projectilesName) ?? m_projectiles;

        AssetDatabase.SaveAssets();
    }

    static T Create<T>(string name) where T : ScriptableObject
    {
        var elements = Resources.LoadAll<ScriptableObject>(s_path);
        foreach (var e in elements)
        {
            if (e.name == name)
            {
                Debug.LogError("An item with the name " + name + " already exist in Ressources/" + s_path);
                return null;
            }
        }

        T asset = ScriptableObjectEx.CreateAsset<T>(s_path, name);
        EditorUtility.SetDirty(asset);

        return asset;
    }
#endif

    static void Load()
    {
        m_lootType = LoadOneInstance<LootType>(s_lootTypeName);
        m_sword = LoadOneInstance<SwordData>(s_swordName);
        m_vacuum = LoadOneInstance<VacuumData>(s_vacuumName);
        m_commonData = LoadOneInstance<CommonData>(s_commonName);
        m_projectiles = LoadOneInstance<ProjectileList>(s_projectilesName);
    }

    static T LoadOneInstance<T>(string name) where T : ScriptableObject
    {
        T asset = null;

        var elements = Resources.LoadAll<ScriptableObject>(s_path);
        foreach (var e in elements)
        {
            if (e.name == name)
            {
                asset = e as T;
                break;
            }
        }

        if (asset == null)
            DebugLogs.LogError("The " + name + " asset does not exist in the Ressources/" + s_path + " folder");

        return asset;
    }
}

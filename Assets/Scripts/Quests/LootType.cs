
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class LootType : ScriptableObject
{
    static string s_path = "Loots";
    static string s_name = "LootType";
    
    [Serializable]
    public class OneLoot
    {
        public GameObject prefab;
        public int stack;
    }

    [Serializable]
    public class OneLootType
    {
        public ItemType type;
        public List<OneLoot> loots;
    }

    [Serializable]
    public class LootParams
    {
        public float m_radius = 0.3f;
        public LayerMask m_layerLootHit;
        public float m_minMoveSpeed = 1;
        public float m_maxMoveSpeed = 2;
        public float m_brakePower = 5;
        public float m_pickupSpeed = 10;
    }

    static LootType m_instance = null;

    [SerializeField] List<OneLootType> m_loots;
    [SerializeField] LootParams m_lootsParams;


#if UNITY_EDITOR
    [MenuItem("Game/Create LootType")]
    public static void MakeLootType()
    {
        var elements = Resources.LoadAll<ScriptableObject>(s_path);
        foreach(var e in elements)
        {
            if (e.name == s_name)
            {
                Debug.LogError("An item with the name " + s_name + " already exist in Ressources/" + s_path);
                return;
            }
        }

        LootType loot = ScriptableObjectEx.CreateAsset<LootType>(s_path, s_name);
        EditorUtility.SetDirty(loot);

        AssetDatabase.SaveAssets();
    }
#endif

    static void Load()
    {
        if (m_instance != null)
            return;

        var elements = Resources.LoadAll<ScriptableObject>(s_path);
        foreach(var e in elements)
        {
            if(e.name == s_name)
            {

                break;
            }
        }

        if(m_instance == null)
            Debug.LogError("The LootType asset does not exist in the Ressources/" + s_path + " folder");
    }

    static OneLootType Get(ItemType item)
    {
        foreach (var l in m_instance.m_loots)
        {
            if (l.type == item)
                return l;
        }
        return null;
    }

    static OneLoot GetBiggest(OneLootType loots, int stack)
    {
        if (stack <= 0)
            return null;

        OneLoot biggest = null;
        foreach(var l in loots.loots)
        {
            if (l.stack > stack)
                continue;

            if (biggest == null || l.stack > biggest.stack)
                biggest = l;
        }

        return biggest;
    }

    public static List<GameObject> MakeLoots(ItemType item, int stack)
    {
        var items = new List<GameObject>();

        Load();
        if (m_instance == null)
            return items;

        OneLootType oneLootType = Get(item);
        if (oneLootType == null)
            return items;

        List<OneLoot> loots = new List<OneLoot>();
        while(true)
        {
            var loot = GetBiggest(oneLootType, stack);
            if (loot == null)
                break;
            stack -= loot.stack;
            loots.Add(loot);
        }

        foreach(var l in loots)
            items.Add(GameObject.Instantiate(l.prefab));

        return items;
    }

    public static LootParams GetParams()
    {
        Load();
        if (m_instance == null)
            return new LootParams();
        return m_instance.m_lootsParams;
    }
}
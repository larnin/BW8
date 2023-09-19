
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
        public int maxStack;
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

    [SerializeField] List<OneLootType> m_loots;
    [SerializeField] LootParams m_lootsParams;

    OneLootType Get(ItemType item)
    {
        foreach (var l in m_loots)
        {
            if (l.type == item)
                return l;
        }
        return null;
    }

    OneLoot GetBiggest(OneLootType loots, int stack)
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

    public List<GameObject> MakeLoots(ItemType item, int stack)
    {
        var items = new List<GameObject>();

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

    public int GetMaxStack(ItemType item)
    {
        OneLootType oneLootType = Get(item);
        if (oneLootType == null)
            return 1;
        return oneLootType.maxStack;
    }

    public LootParams GetParams()
    {
        return m_lootsParams;
    }
}
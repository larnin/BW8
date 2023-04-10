using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NRand;

[CreateAssetMenu(fileName = "LootList", menuName = "Game/LootList", order = 1)]
public class LootList : ScriptableObject
{

    [Serializable]
    public class OneLoot
    {
        public ItemType item;
        public int minStack = 1;
        public int maxStack = 1;
        public float weight = 1;
    }

    [Serializable]
    public class OneLootGroup
    {
        //public float weight = 1; //no current use
        public List<OneLoot> loots = new List<OneLoot>();
    }

    [SerializeField] List<OneLootGroup> m_lootGroup = new List<OneLootGroup>();
    [SerializeField] List<OneLoot> m_loots = new List<OneLoot>();
    [SerializeField] int m_maxLoot = -1;

    public List<GameObject> GenerateLoots()
    {
        var rand = new StaticRandomGenerator<MT19937>();

        int lootTypeCount = Enum.GetNames(typeof(ItemType)).Length;
        List<int> lootCount = new List<int>();
        for (int i = 0; i < lootTypeCount; i++)
            lootCount.Add(0);

        foreach(var g in m_lootGroup)
        {
            if (g.loots.Count == 0)
                continue;
            List<float> weights = new List<float>();
            foreach (var l in g.loots)
                weights.Add(l.weight);
            int index = new DiscreteDistribution(weights).Next(rand);
            int stack = new UniformIntDistribution(g.loots[index].minStack, g.loots[index].maxStack + 1).Next(rand);
            lootCount[(int)g.loots[index].item] += stack;
        }
        foreach(var l in m_loots)
        {
            if(l.weight < 100)
            {
                if (new UniformFloatDistribution(0, 100).Next(rand) > l.weight)
                    continue;
            }
            int stack = new UniformIntDistribution(l.minStack, l.maxStack + 1).Next(rand);
            lootCount[(int)l.item] += stack;
        }

        int totalLoot = 0;
        foreach (var l in lootCount)
            totalLoot += l;

        if(m_maxLoot > 0 && totalLoot > m_maxLoot)
        {
            List<ItemType> items = new List<ItemType>();
            for(int i = 0; i < lootCount.Count; i++)
            {
                for (int j = 0; j < lootCount[i]; j++)
                    items.Add((ItemType)i);
            }

            items.Shuffle(rand);

            for (int i = 0; i < lootCount.Count; i++)
                lootCount[i] = 0;
            for (int i = 0; i < m_maxLoot; i++)
                lootCount[(int)items[i]]++;
        }

        List<GameObject> loots = new List<GameObject>();
        for(int i = 0; i < lootCount.Count; i++)
        {
            if (lootCount[i] <= 0)
                continue;

            var objects = World.lootType.MakeLoots((ItemType)i, lootCount[i]);
            foreach (var o in objects)
                loots.Add(o);
        }

        return loots;
    }
}


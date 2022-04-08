using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NRand;

public class NpcGroup : MonoBehaviour
{
    [Serializable]
    class GroupData
    {
        public GameObject npc = null;
        public int count = 1;
        public float radius = 0;
    }

    [SerializeField] List<GroupData> m_groups;
    [SerializeField] LayerMask m_ground = new LayerMask();

    private void Start()
    {
        var rand = new StaticRandomGenerator<MT19937>();

        foreach(var g in m_groups)
        {
            if (g.npc == null || g.count <= 0)
                continue;

            var d = new UniformVector2CircleDistribution(g.radius);

            for(int i = 0; i < g.count; i++)
            {
                Vector3 pos = d.Next(rand);
                pos += transform.position;

                var obj = Instantiate(g.npc);
                obj.transform.position = pos;

                Event<AddEntityEvent>.Broadcast(new AddEntityEvent(obj));
            }
        }
    }
}

using NLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NRand;

public class LootItem : Interactable
{
    enum State
    {
        Init,
        Dropped,
        Idle,
        Looted,
        Picked,
    }

    [SerializeField] bool m_autoLoot = false;
    [SerializeField] ItemType m_type;
    [SerializeField] int m_stack = 1;

    State m_state = State.Init;
    Vector2 m_dir = Vector2.zero;
    float m_speed = 0;

    GameObject m_picker;

    public override bool CanInteract()
    {
        return m_state == State.Idle;
    }

    public override int GetInteractionTextID()
    {
        return Loc.GetTextID("Pickup");
    }

    public override Vector2 GetOffset()
    {
        return new Vector2(0, 1);
    }

    public override void Interact(GameObject caster)
    {
        m_picker = caster;
    }

    public override bool IsInstantInteraction()
    {
        return m_autoLoot;
    }

    private void Update()
    {
        if (Gamestate.instance.paused)
            return;

        switch(m_state)
        {
            case State.Init:
                OnInit();
                break;
            case State.Dropped:
                OnDrop();
                break;
            case State.Idle:
                OnIdle();
                break;
            case State.Looted:
                OnLoot();
                break;
            default:
                break;
        }
    }

    void Pickup()
    {
        m_state = State.Picked;
        Event<PickupEvent>.Broadcast(new PickupEvent(m_type, m_stack), m_picker, true);
        Destroy(gameObject);
    }

    void OnInit()
    {
        var lootParams = LootType.GetParams();

        var rand = new StaticRandomGenerator<MT19937>();
        var dirDistrib = new UniformVector2CircleSurfaceDistribution();
        var distDistrib = new UniformFloatDistribution(lootParams.m_minMoveSpeed, lootParams.m_maxMoveSpeed);

        m_dir = dirDistrib.Next(rand);
        m_speed = distDistrib.Next(rand);

        m_state = State.Dropped;
    }

    void OnDrop()
    {
        var lootParams = LootType.GetParams();

        float dist = m_speed * Time.deltaTime;
        Vector2 currentPos = transform.position;

        Vector2 newDir = m_dir;
        float newSpeed = m_speed;

        var hits = Physics2D.CircleCastAll(currentPos, lootParams.m_radius, m_dir, dist, lootParams.m_layerLootHit);
        if (hits.Length != 0)
        {
            RaycastHit2D bestHit = hits[0];
            for(int i = 1; i < hits.Length; i++)
            {
                if (hits[i].distance < bestHit.distance)
                    bestHit = hits[i];
            }

            Vector2 wallDir = Vector2.zero;
            wallDir.x = -bestHit.normal.y;
            wallDir.y = bestHit.normal.x;

            float project = Vector2.Dot(m_dir, wallDir);

            if (project < 0)
                wallDir *= -1;
            newSpeed = MathF.Abs(project);
            newDir = wallDir;

            const float minDistFromWall = 0.01f;
            dist = hits.Length;
            if (dist < minDistFromWall)
                dist = 0;
        }

        Vector3 newPos = transform.position;
        newPos.x += newDir.x * dist;
        newPos.y += newDir.y * dist;
        transform.position = newPos;

        newSpeed -= lootParams.m_brakePower * Time.deltaTime;
        if (newSpeed <= 0)
            m_state = State.Idle;

        m_dir = newDir;
        m_speed = newSpeed;
    }

    void OnIdle()
    {
        if (m_picker != null)
            m_state = State.Looted;
    }

    void OnLoot()
    {
        if (m_picker == null)
        {
            m_state = State.Idle;
            return;
        }

        Vector2 dir = m_picker.transform.position - transform.position;
        float dist = dir.magnitude;
        dir /= dist;

        var lootParams = LootType.GetParams();

        float updateDist = lootParams.m_pickupSpeed * Time.deltaTime;
        if (updateDist >= dist)
        {
            Pickup();
            return;
        }

        dir *= updateDist;

        var pos = transform.position;
        pos.x += dir.x;
        pos.y += dir.y;

        transform.position = pos;
    }
}

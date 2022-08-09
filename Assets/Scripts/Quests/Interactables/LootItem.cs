using NLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class LootItem : Interactable
{
    [SerializeField] bool m_autoLoot = false;
    [SerializeField] ItemType m_type;
    [SerializeField] int m_stack = 1;

    GameObject m_picker;

    const float pickSpeed = 10;

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
        if (m_picker == null || Gamestate.instance.paused)
            return;

        Vector2 dir = m_picker.transform.position - transform.position;
        float dist = dir.magnitude;
        dir /= dist;

        float updateDist = pickSpeed * Time.deltaTime;
        if(updateDist >= dist)
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

    void Pickup()
    {
        Event<PickupEvent>.Broadcast(new PickupEvent(m_type, m_stack), m_picker, true);
        Destroy(gameObject);
    }
}

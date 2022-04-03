using System.Collections;
using UnityEngine;


public class Portal : MonoBehaviour
{
    [SerializeField] string m_spawnName;
    [SerializeField] WorldObject m_world;
    [SerializeField] LayerMask m_collideLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == m_collideLayer)
            Event<StartChangeWorldEvent>.Broadcast(new StartChangeWorldEvent(m_world, m_spawnName));
    }
}
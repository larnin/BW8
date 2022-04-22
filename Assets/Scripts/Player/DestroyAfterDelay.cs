using System.Collections;
using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
    [SerializeField] float m_delay = 0;

    void Start()
    {
        Destroy(gameObject, m_delay);
    }
}
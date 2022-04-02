using System.Collections;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] string m_spawnName;

    public string spawnName { get { return m_spawnName; } }
}
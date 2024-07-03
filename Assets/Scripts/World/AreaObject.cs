using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Area", menuName = "Game/New area")]
public class AreaObject : ScriptableObject
{
    [SerializeField] WorldObject m_world;
    [SerializeField] string m_quest;
    [SerializeField] string m_saveID;
}

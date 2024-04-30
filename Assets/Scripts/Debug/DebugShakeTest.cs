using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DebugShakeTest : SerializedMonoBehaviour
{
    [SerializeField] List<ScreenShakeBase> m_shakes = new List<ScreenShakeBase>();

    [Button]
    void Send()
    {
        if (m_shakes == null)
            return;

        foreach(var shake in m_shakes)
        {
            Event<AddScreenShakeEvent>.Broadcast(new AddScreenShakeEvent(shake));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StupidRainbowColor : MonoBehaviour
{
    [SerializeField] float m_speed = 1;

    private void Update()
    {
        Color red = Color.HSVToRGB((Time.time * m_speed) % 1, 1, 1);
        Color yellow = Color.HSVToRGB((Time.time * m_speed + 1f / 6) % 1, 1, 1);
        Color green = Color.HSVToRGB((Time.time * m_speed + 2f / 6) % 1, 1, 1);
        Color cyan = Color.HSVToRGB((Time.time * m_speed + 3f / 6) % 1, 1, 1);
        Color blue = Color.HSVToRGB((Time.time * m_speed + 4f / 6) % 1, 1, 1);
        Color pink = Color.HSVToRGB((Time.time* m_speed + 5f / 6) % 1, 1, 1);

        Event<SetCustomScreenColorEvent>.Broadcast(new SetCustomScreenColorEvent(red, yellow, green, cyan, blue, pink));
    }
}
